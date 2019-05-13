using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that uses soldier pools
    /// in a specific order.
    /// </summary>
    public class OrderedSelectionProcedure : AbstractSelectionProcedure
    {
        /// <summary>
        /// The open Simulation database connection
        /// </summary>
        protected SimDatabase Database { get; set; }

        /// <summary>
        /// The underlying Procedure Wrapper
        /// </summary>
        public OrderedProcedureWrapper ProcedureWrapper { get; protected set; }

        /// <summary>
        /// Our number randomizer
        /// </summary>
        private CryptoRandom Randomizer = new CryptoRandom();

        /// <summary>
        /// Gets the selection filtering options for each <see cref="OrderedPool"/>.
        /// </summary>
        /// <remarks>Includes the <see cref="Billet"/> filters as well</remarks>
        protected Dictionary<int, List<AbstractFilter>> PoolFilters { get; set; }

        /// <summary>
        /// Gets the selection grouping options for each <see cref="OrderedPool"/>.
        /// </summary>
        /// <remarks>Includes the <see cref="Billet"/> grouping as well</remarks>
        protected Dictionary<int, List<AbstractFilter>> PoolGroups { get; set; }

        /// <summary>
        /// Gets the selection sorting options for each <see cref="OrderedPool"/>.
        /// </summary>
        /// <remarks>Includes the <see cref="Billet"/> sorting as well</remarks>
        protected Dictionary<int, List<AbstractSort>> PoolSorting { get; set; }

        protected List<SoldierPoolWrapper<OrderedPool>> SoldierPools { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="billet"></param>
        public OrderedSelectionProcedure(SimDatabase db, Billet billet) : base(db, billet)
        {
            // Store database connection
            Database = db;

            // Setup class variables
            Randomizer = new CryptoRandom();
            SoldierPools = new List<SoldierPoolWrapper<OrderedPool>>();
            PoolFilters = new Dictionary<int, List<AbstractFilter>>();
            PoolGroups = new Dictionary<int, List<AbstractFilter>>();
            PoolSorting = new Dictionary<int, List<AbstractSort>>();

            // Create procedure
            var proc = billet.OrderedProcedures.FirstOrDefault();
            ProcedureWrapper = new OrderedProcedureWrapper(proc.Procedure, db);

            // Add each poll into the list of pools
            foreach (var item in ProcedureWrapper.ProcedurePools.OrderBy(x => x.Precedence))
            {
                var setting = new SoldierPoolWrapper<OrderedPool>()
                {
                    Type = SpawnSoldierType.TakeFromExistingPool,
                    Probability = item.Probability,
                    Rank = item.Rank,
                    Pool = item
                };

                // Check for a career
                int id = item.CareerGenerator?.Id ?? 0;
                if (SimulationCache.CareerGenerators.TryGetValue(id, out CareerGenerator career))
                {
                    setting.Career = career;
                }

                SoldierPools.Add(setting);

                // Cache the filters
                var filters = new List<AbstractFilter>(item.SoldierFilters.OrderBy(x => x.Precedence));
                filters.AddRange(Filters);
                PoolFilters.Add(item.Id, filters);

                // Cache the grouping
                var groups = new List<AbstractFilter>(item.SoldierGroups.OrderBy(x => x.Precedence));
                groups.AddRange(Grouping);
                PoolGroups.Add(item.Id, groups);

                // Cache sorting
                if (!item.SelectRandom)
                {
                    var sorting = new List<AbstractSort>(item.SoldierSorting.OrderBy(x => x.Precedence));
                    sorting.AddRange(Sorting);
                    PoolSorting.Add(item.Id, sorting);
                }
            }

            if (ProcedureWrapper.CreatesNewSoldiers)
            {
                // Add spawn setting to list!
                SoldierPools.Add(new SoldierPoolWrapper<OrderedPool>()
                {
                    Type = SpawnSoldierType.CreateNew,
                    Probability = ProcedureWrapper.NewSoldierProbability,
                    Career = ProcedureWrapper.NewSoldierCareer
                });
            }
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            foreach (var pool in SoldierPools)
            {
                // Roll the number!
                int apathy = Randomizer.Next(1, 100);

                // If there isn't enough intrest, skip this pool
                if (apathy > pool.Probability)
                    continue;

                // Fetch promotion pool
                UnitWrapper topUnit = position.PromotionPoolUnit;

                // ---------------------------
                // Apply Rank Filters
                // ---------------------------
                IEnumerable<SoldierWrapper> soldiers = topUnit.SoldiersByGrade[pool.Rank.Type][pool.Rank.Grade].Values.ToList();
                if (!pool.UseRankGrade)
                {
                    int rankId = pool.Rank.Id;
                    soldiers = soldiers.Where(x => x.Rank.Id == rankId);
                }

                // Find best soldier for lateral movement
                var wrapper = FindCrossPoolSoldier(soldiers.ToList(), pool, position, date);

                // Grab our top filtered soldier
                if (wrapper == null)
                {
                    // Move to the next pool
                    continue;
                }
                else if (pool.Career != null)
                {
                    // Assign the soldiers new career length based on Rank Type
                    CareerLengthRange career = pool.Career.Spawn();
                    wrapper.Soldier.CareerLengthId = career.Id;
                    wrapper.Soldier.ExitIterationId = date.Id + career.GenerateLength();
                }

                // Set type
                type = SpawnSoldierType.TakeFromExistingPool;
                return wrapper;
            }

            // No soldiers were pulled from the promotion pools...
            // Can we create a new soldier?
            type = SpawnSoldierType.CreateNew;
            if (ProcedureWrapper.CreatesNewSoldiers)
            {
                // Create a new soldier object
                Soldier soldier = new Soldier
                {
                    FirstName = SimulationCache.NameGenerator.GenerateRandomFirstName(),
                    LastName = SimulationCache.NameGenerator.GenerateRandomLastName(),
                    EntryIterationId = date.Id,
                    LastPromotionIterationId = date.Id,
                    LastGradeChangeIterationId = date.Id,
                    RankId = position.Billet.Rank.Id,
                    SpecialtyId = position.Billet.Specialty.Id
                };

                // Assign the soldiers new career length based on Rank Type
                CareerLengthRange careerLength = ProcedureWrapper.NewSoldierCareer.Spawn();
                soldier.CareerLengthId = careerLength.Id;
                soldier.ExitIterationId = date.Id + careerLength.GenerateLength();

                // Add soldier to database
                Database.Soldiers.Add(soldier);

                // Create soldier wrapper
                return new SoldierWrapper(soldier, position.Billet.Rank, position.Billet.Specialty, date, Database);
            }

            return null;
        }

        /// <summary>
        /// This method is used to pull a soldier from within a unit somewhere, and return
        /// the best soldier for the position
        /// </summary>
        /// <param name="soldiers"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private SoldierWrapper FindCrossPoolSoldier(
            List<SoldierWrapper> soldiers,
            SoldierPoolWrapper<OrderedPool> setting,
            PositionWrapper position,
            IterationDate date)
        {
            // Quit if we have no soldiers!
            if (soldiers.Count == 0)
                return null;

            // Setup variables
            IEnumerable<SoldierWrapper> candidates = soldiers;

            //
            // 1. Apply Soldier Pool filtering
            //
            if (PoolFilters.ContainsKey(setting.Pool.Id))
            {
                var filters = PoolFilters[setting.Pool.Id];
                candidates = candidates.FilterSoldierList(filters, setting.Pool.FilterLogic, date);
            }

            // Do we have anyone left?
            int count = candidates.Count();
            if (count == 0)
            {
                return null;
            }
            // Apply grouping and sorting only if we have more than 1 soldier
            else if (count > 1)
            {
                //
                // 3. Apply Billet grouping
                //
                if (PoolGroups.ContainsKey(setting.Pool.Id))
                {
                    IEnumerable<SoldierGroupResult> result = null;
                    if (setting.Pool.GroupByDesire)
                    {
                        // Apply groups
                        var groups = PoolGroups[setting.Pool.Id];

                        // Apply lateral desire grouping (Need, Want, Dont Want), Then By billet grouping
                        result = candidates.GroupSoldiersBy(
                            x => GetLateralPromotionGroupId(x, position, date),
                            groups,
                            date
                        );
                    }
                    else
                    {
                        // Apply groups
                        var groups = PoolGroups[setting.Pool.Id];
                        result = candidates.GroupSoldiersBy(groups, date);
                    }

                    // Get topmost group with at least one soldier in it
                    // This list of soldiers will be the most prime canidates,
                    // hitting Most if Not All grouping requirements
                    candidates = result.GetPrimeSoldiers();
                }

                // Do we have any soldiers?
                if (candidates.Count() == 0)
                    throw new Exception("Group has no prime soldiers, but there was a soldier count");

                //
                // 4. Apply soldier ordering
                //
                if (PoolSorting.ContainsKey(setting.Pool.Id))
                {
                    var poolSorts = PoolSorting[setting.Pool.Id];
                    if (poolSorts.Count > 0)
                        candidates = candidates.OrderSoldiersBy(poolSorts, date);
                }
            }

            //
            // 5. Grab a list of eligable soldiers
            //
            PromotableStatus status;
            var eligable = new List<SoldierWrapper>();
            foreach (var soldier in candidates)
            {
                // Must be promotable?
                if (setting.Pool.MustBePromotable)
                {
                    if (!soldier.IsPromotable(date, out status) || status == PromotableStatus.Lateral)
                        continue;
                }

                // Soldier locked into position?
                if (soldier.Position != null && setting.Pool.NotLockedInBillet && soldier.IsLockedInPosition(date))
                {
                    continue;
                }

                // Make sure the soldier can even sit in this position!
                if (IsCanidateForPosition(soldier, position, date))
                {
                    eligable.Add(soldier);
                }
            }

            // Grab a soldier if we have any
            if (eligable.Count > 0)
            {
                if (setting.Pool.SelectRandom && eligable.Count > 1)
                {
                    // Select random soldier
                    int index = Randomizer.Next(0, eligable.Count - 1);
                    return eligable[index];
                }
                else
                {
                    return eligable.FirstOrDefault();
                }
            }

            // No soldier was eligable
            return null;
        }
    }
}
