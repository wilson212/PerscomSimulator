using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that is randomly chosen
    /// based off of RNG with the specified promotion pools.
    /// </summary>
    public class RandomizedSelectionProcedure : AbstractSelectionProcedure
    {
        /// <summary>
        /// The open Simulation database connection
        /// </summary>
        protected SimDatabase Database { get; set; }

        /// <summary>
        /// The underlying Procedure Wrapper
        /// </summary>
        public RandomizedProcedureWrapper ProcedureWrapper { get; protected set; }

        /// <summary>
        /// Gets our RNG soldier pool spawner
        /// </summary>
        protected SpawnGenerator<SoldierPoolWrapper<RandomizedPool>> Generator { get; set; }

        /// <summary>
        /// Gets the selection filtering options for each <see cref="RandomizedPool"/>.
        /// </summary>
        /// <remarks>Includes the <see cref="Billet"/> filters as well</remarks>
        protected Dictionary<int, List<AbstractFilter>> PoolFilters { get; set; }

        /// <summary>
        /// Gets the selection sorting options for each <see cref="RandomizedPool"/>.
        /// </summary>
        /// <remarks>Includes the <see cref="Billet"/> sorting as well</remarks>
        protected Dictionary<int, List<AbstractSort>> PoolSorting { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="RandomizedSelectionProcedure"/>
        /// </summary>
        /// <param name="db">The open Simulation database connection</param>
        /// <param name="billet">The billet this procedure is created for</param>
        public RandomizedSelectionProcedure(SimDatabase db, Billet billet) : base(db, billet)
        {
            // Store database connection
            Database = db;

            // Create generator instance
            Generator = new SpawnGenerator<SoldierPoolWrapper<RandomizedPool>>();
            PoolFilters = new Dictionary<int, List<AbstractFilter>>();
            PoolSorting = new Dictionary<int, List<AbstractSort>>();

            // Create procedure
            var proc = billet.RandomizedProcedures.FirstOrDefault();
            ProcedureWrapper = new RandomizedProcedureWrapper(proc.Procedure, db);

            // Add the new soldier spawnable entity
            if (ProcedureWrapper.CreatesNewSoldiers)
            {
                // Add spawn setting to list!
                Generator.Add(new SoldierPoolWrapper<RandomizedPool>()
                {
                    Type = SpawnSoldierType.CreateNew,
                    Probability = ProcedureWrapper.NewSoldierProbability,
                    Career = ProcedureWrapper.NewSoldierCareer
                });
            }

            // Add each pool
            foreach (var item in ProcedureWrapper.ProcedurePools)
            {
                var setting = new SoldierPoolWrapper<RandomizedPool>()
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

                // Add spawn setting to list!
                Generator.Add(setting);

                // Cache the filters
                var filters = new List<AbstractFilter>(item.SoldierFiltering.OrderBy(x => x.Precedence));
                filters.AddRange(Filters);
                PoolFilters.Add(item.Id, filters);

                // Cache sorting
                if (item.OrdersBeforeBilletOrdering)
                {
                    var sorting = new List<AbstractSort>(item.SoldierSorting.OrderBy(x => x.Precedence));
                    sorting.AddRange(Sorting);
                    PoolSorting.Add(item.Id, sorting);
                }
                else
                {
                    var sorting = new List<AbstractSort>(Sorting);
                    sorting.AddRange(item.SoldierSorting.OrderBy(x => x.Precedence));
                    PoolSorting.Add(item.Id, sorting);
                }
            }
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            // Spawn a soldier generator setting
            var setting = Generator.Spawn();
            if (setting == null)
                throw new Exception("Unable to spawn soldier setting from soldier Generator!");

            // Define vars
            SoldierWrapper wrapper = null;

            // If rankId is 0, then we create new
            if (setting.Type == SpawnSoldierType.CreateNew)
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
                wrapper = new SoldierWrapper(soldier, position.Billet.Rank, position.Billet.Specialty, date, Database);

                // Set type
                type = SpawnSoldierType.CreateNew;
            }
            else
            {
                // Fetch promotion pool
                UnitWrapper topUnit = position.PromotionPoolUnit;

                // ---------------------------
                // Apply Rank Filters
                // ---------------------------
                IEnumerable<SoldierWrapper> soldiers = topUnit.SoldiersByGrade[setting.Rank.Type][setting.Rank.Grade].Values.ToList();
                if (!setting.UseRankGrade)
                {
                    int rankId = setting.Rank.Id;
                    soldiers = soldiers.Where(x => x.Rank.Id == rankId);
                }

                // Find best soldier for lateral movement
                wrapper = FindCrossPoolSoldier(soldiers.ToList(), setting, position, date);

                // Grab our top filtered soldier
                if (wrapper != null && setting.Career != null)
                {
                    // Assign the soldiers new career length based on Rank Type
                    CareerLengthRange career = setting.Career.Spawn();
                    wrapper.Soldier.CareerLengthId = career.Id;
                    wrapper.Soldier.ExitIterationId = date.Id + career.GenerateLength();
                }

                // Set type
                type = SpawnSoldierType.TakeFromExistingPool;
            }

            // Send him to duty!
            return wrapper;
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
            SoldierPoolWrapper<RandomizedPool> setting, 
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
                if (Grouping.Count > 0)
                {
                    // Apply groups
                    var result = candidates.GroupSoldiersBy(Grouping, date);

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
            // 5. Select the first elegible solder
            //
            PromotableStatus status;
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
                    return soldier;
            }

            return null;
        }
    }
}
