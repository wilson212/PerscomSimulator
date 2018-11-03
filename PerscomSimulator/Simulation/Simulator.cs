using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CrossLite;
using CrossLite.QueryBuilder;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom
{
    public class Simulator
    {
        protected SimDatabase Database { get; set; }

        /// <summary>
        /// The starting date for a simulation
        /// </summary>
        public DateTime StartDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Gets the current Simulation date
        /// </summary>
        public DateTime CurrentDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the current DateTime in the simulation
        /// </summary>
        public IterationDate CurrentIterationDate { get; set; }

        /// <summary>
        /// Gets the End Date for the simulation
        /// </summary>
        public DateTime EndDate { get; protected set; }

        /// <summary>
        /// Gets the <see cref="SimulatorSettings"/> for the last simulation
        /// </summary>
        public SimulatorSettings Settings { get; private set; }

        /// <summary>
        /// Grade => Soldier Array
        /// </summary>
        public Dictionary<int, SoldierWrapper> ActiveDutySoldiers { get; protected set; }

        /// <summary>
        /// Grade => Soldier Array
        /// </summary>
        public List<PositionWrapper> Positions { get; protected set; }

        /// <summary>
        /// Soldier Generator
        /// </summary>
        protected Dictionary<int, SoldierGenerator> SoldierGenerators { get; set; }

        /// <summary>
        /// UnitTemplateId => [RankType => [Rank.Grade => RankGradeStatistics]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>> RankStatistics
        {
            get;
            protected set;
        }

        /// <summary>
        /// UnitTemplateId => [RankType => [Rank.Grade => [SpecialtyId => SpecialtyGradeStatistics]]]
        /// </summary>
        public Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>> SpecialtyStatistics
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Unit that is processing in this Simulator instance
        /// </summary>
        public UnitWrapper ProcessingUnit { get; protected set; }

        /// <summary>
        /// The number of years to skip in the simulation before logging
        /// statistical data
        /// </summary>
        protected int SkipYears { get; set; }

        /// <summary>
        /// The total number of years the simulation was ran
        /// </summary>
        protected int TotalYearsRan { get; set; } = 0;

        /// <summary>
        /// The total number of years the simulation was ran
        /// </summary>
        protected int Iteration { get; set; } = 1;

        /// <summary>
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        protected RandomNameGenerator NameGenerator { get; set; }

        /// <summary>
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes { get; set; } = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        protected Dictionary<int, IOrderedEnumerable<SoldierPoolSorting>> SoldierPoolSorting
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new Simulator instance
        /// </summary>
        /// <param name="unit">The unit to run the simulation on</param>
        public Simulator(SimDatabase db, UnitWrapper unit, SimulatorSettings settings)
        {
            Database = db;
            ProcessingUnit = unit;
            Settings = settings;

            // Check if new simulation by checking dates and unit id's

            // Create name generator
            NameGenerator = new RandomNameGenerator();

            // Load Generators
            SoldierGenerators = new Dictionary<int, SoldierGenerator>();
            foreach (var generator in db.SoldierGenerators)
            {
                generator.Initialize();
                SoldierGenerators.Add(generator.Id, generator);
            }

            // Load and Cache Soldier Pool Sortings!
            SoldierPoolSorting = new Dictionary<int, IOrderedEnumerable<SoldierPoolSorting>>();
            foreach (var item in db.SoldierGeneratorPools)
            {
                var sorts = item.SoldierSorting;
                if (sorts != null && sorts.Count() > 0)
                {
                    SoldierPoolSorting.Add(item.Id, sorts.OrderBy(x => x.Precedence));
                }
            }
        }

        /// <summary>
        /// Runs the simulation
        /// </summary>
        /// <param name="totalYears">How many years to run the simulation for.</param>
        /// <param name="skipYears">
        /// How many years to skip before logging statistical data. This is done to
        /// alleviate bad data at the start of a simulation due to low soldier movement.
        /// </param>
        public void Run(int totalYears, int skipYears, IProgress<TaskProgressUpdate> progress, CancellationToken token)
        {
            // Check data
            if (totalYears < skipYears)
                throw new ArgumentException("skipYears cannot be less than totalYears", "skipYears");

            // First, set the end date
            SkipYears = skipYears;
            TotalYearsRan = 0;

            // Set initial dates
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            StartDate = EndDate.AddYears(-totalYears);
            CurrentDate = StartDate;

            // Wrap in an exception block
            try
            {
                // Set Iteration ID if we are continuing from a previous sim
                var date = Database.Query<IterationDate>("SELECT * FROM IterationDate ORDER BY Id DESC LIMIT 1").FirstOrDefault();
                if (date != null)
                {
                    StartDate = new DateTime(date.Date.Year, date.Date.Month, 1).AddMonths(1);
                    CurrentDate = StartDate;
                    EndDate = StartDate.AddYears(totalYears);
                    CurrentIterationDate = date;
                }
                else
                {
                    // Create Iteration Date
                    CurrentIterationDate = new IterationDate() { Date = CurrentDate };
                    Database.IterationDates.Add(CurrentIterationDate);
                }

                // Disable foriegn key relationships in instances, to increase performance of the simulation
                SetTableForeignKeyStatus(false);

                // Update progress
                token.ThrowIfCancellationRequested();
                TaskProgressUpdate update = new TaskProgressUpdate();
                update.MessageText = "Ordering soldier positions by grade and stature...";
                progress.Report(update);

                // Get all positions
                Positions = ProcessingUnit.GetAllPositions()
                    .OrderBy(x => x.Billet.Rank.Grade)
                    .ThenByDescending(x => x.Billet.Stature)
                    .ToList();

                token.ThrowIfCancellationRequested();
                update = new TaskProgressUpdate();
                update.MessageText = "Populating soldier seats...";
                progress.Report(update);

                // Populate soldiers if we need to
                using (var trans = Database.BeginTransaction())
                {
                    PopulateSoldiers();
                    trans.Commit();
                }

                // Update progress
                token.ThrowIfCancellationRequested();
                update = new TaskProgressUpdate();
                update.HeaderText = "Running Simulation... Please Wait.";
                update.MessageText = "";
                progress.Report(update);

                // Variable holder for the month name
                string name = String.Empty;

                // Main Loop
                while (EndDate != CurrentDate)
                {
                    // Quit if cancelled
                    token.ThrowIfCancellationRequested();

                    using (var trans = Database.BeginTransaction())
                    {
                        // Update the date
                        if (Iteration > 1)
                        {
                            CurrentDate = CurrentDate.AddMonths(1);

                            // Create Iteration Date
                            CurrentIterationDate = new IterationDate() { Date = CurrentDate };
                            Database.IterationDates.Add(CurrentIterationDate);
                        }

                        // Ensure we are not screwed up here
                        if (Iteration != CurrentIterationDate.Id)
                        {
                            throw new Exception("Date and Iteration dont match!");
                        }

                        // Update progress window
                        name = CurrentDate.ToString("MMMM");
                        update = new TaskProgressUpdate();
                        update.MessageText = $"Processing {name} of year {TotalYearsRan + 1} of {totalYears}";
                        progress.Report(update);

                        // Now do promotions
                        DoMontlyPositionChecks(token);

                        // If an entire years has gone by, subtract a skip year
                        // so we can begin logging again when the time comes.
                        if (CurrentDate.Month == StartDate.Month && Iteration > 2)
                        {
                            TotalYearsRan = CurrentDate.Year - StartDate.Year;
                            if (SkipYears > 0)
                                SkipYears--;

                            GC.Collect();
                        }

                        // Commit changes
                        trans.Commit();

                        // Increment our iteration Id
                        Iteration++;
                    }
                }

                // Update progress
                update = new TaskProgressUpdate();
                update.HeaderText = "Saving Simulation Results... Please Wait.";
                progress.Report(update);

                // Save statistics
                using (var trans = Database.BeginTransaction())
                {
                    // Save soldiers and their relevant data
                    foreach (SoldierWrapper s in ActiveDutySoldiers.Values)
                    {
                        s.Save(Database, CurrentIterationDate);
                        s.Dispose();
                    }

                    // Save rank stats data
                    foreach (var template in RankStatistics.Values)
                        foreach (var grade in template.Values)
                            foreach (var stat in grade.Values)
                            {
                                Database.RankGradeStatistics.Add(stat);
                            }

                    // Save specialty data
                    foreach (var template in SpecialtyStatistics)
                        foreach (var spec in template.Value.Values)
                            foreach (var grade in spec.Values)
                                foreach (var stat in grade.Values)
                                {
                                    Database.SpecialtyGradeStatistics.Add(stat);
                                }

                    // Save
                    trans.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.GenerateExceptionLog(ex);
                throw;
            }
            finally
            {
                // Re-enable foriegn key relationships in instances
                SetTableForeignKeyStatus(true);
            }
        }

        /// <summary>
        /// Sets whether the new entries of certain tables in the database
        /// get their <see cref="CrossLite.CodeFirst.ForeignKey{TEntity}"/>
        /// properties created. When true, there is a massive performance hit
        /// to the simulator, so we disable these when running.
        /// </summary>
        /// <param name="enabled"></param>
        private void SetTableForeignKeyStatus(bool enabled)
        {
            EntityCache.GetTableMap(typeof(PastAssignment)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(SpecialtyAssignment)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(Soldier)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(Promotion)).BuildInstanceForeignKeys = enabled;
        }

        /// <summary>
        /// This method is called after <see cref="PerformStartOfMonthDuties()"/> is 
        /// called, so we can promote soldiers to fill those new empty slots.
        /// </summary>
        private void DoMontlyPositionChecks(CancellationToken token)
        {
            // Loop through each position
            // Sorted by [RankType -> Grade -> Stature]
            foreach (var position in Positions)
            {
                // Quit if we cancelled
                token.ThrowIfCancellationRequested();

                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(position.Billet.Rank.Type))
                    continue;

                // Grab current holder
                SoldierWrapper soldier = position.Holder;
                RankType type = position.Billet.Rank.Type;

                // Check if position is empty
                if (position.IsEmpty)
                {
                    // Is this an entry level position?
                    if (position.Billet.UsesCustomGenerator)
                    {
                        soldier = CreateSoldier(SoldierGenerators[position.Billet.SpawnSetting.GeneratorId], position);
                        soldier?.AssignPosition(position, CurrentIterationDate, Database);
                        continue;
                    }
                    else
                    {
                        // Grab soldier pool
                        soldier = FindBestSoldierFor(position);
                        if (soldier != null)
                        {
                            soldier.AssignPosition(position, CurrentIterationDate, Database);
                        }
                    }
                }
                // Process Lateral Movement
                else if (position.Billet.MaxTourLength > 0)
                {
                    // If we are at max tour length, or past it,
                    // we are a prime candidate for moving!
                    if (position.Holder.IsPastMaxTourLength(CurrentIterationDate))
                    {
                        TryPerformLateralMovement(soldier, position);
                    }
                }

                // If we could not find someone to fill the position
                if (soldier == null)
                    continue;

                // Define soldier vars
                PromotableStatus status;
                int grade = soldier.Rank.Grade;
                int specId = soldier.Soldier.SpecialtyId;

                // Check for retirement (forced by MaxTourLength, MaxTiG, or freewill)
                if (soldier.IsRetiring(CurrentIterationDate))
                {
                    // Log retirement data for current rank/grade
                    LogRetiree(soldier);

                    // Remove the retired soldier from the roster
                    ActiveDutySoldiers.Remove(soldier.Soldier.Id);

                    // Say goodbye!
                    soldier.Retire(CurrentIterationDate, Database);

                    // Update soldier record
                    soldier.Save(Database, CurrentIterationDate);

                    // Call dispose!
                    soldier.Dispose();
                }

                // Check for auto promotion and under-staff promotion
                else if (soldier.IsPromotable(CurrentIterationDate, out status))
                {
                    if (status == PromotableStatus.Automatic || status == PromotableStatus.Position)
                    {
                        // Log promotion data for current rank/grade
                        LogPromotion(soldier);

                        // Get the expected soldier rank/grade to promote from
                        var expectedGrade = position.Billet.Rank.Grade - 1;

                        // Promote soldier. Do not skip rank grades!
                        if (soldier.Rank.Grade == expectedGrade)
                        {
                            // Billet grade is 1 level higher
                            soldier.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);
                        }
                        else
                        {
                            // Billet grade is multiple levels higher
                            Rank toRank = RankCache.GetNextGradeRanks(soldier.Rank).FirstOrDefault();
                            if (toRank != null)
                                soldier.PromoteTo(CurrentIterationDate, toRank, Database);
                            else
                                throw new Exception("Ran out of ranks? wtf");
                        }
                    }
                    else if (status == PromotableStatus.Lateral)
                    {
                        // Do not log promotion since this is lateral
                        soldier.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);
                    }
                }
            }
        }

        /// <summary>
        /// This method will attempt to find another Soldier
        /// to fill the supplied position, and force the soldiers
        /// to swap positions with each other.
        /// </summary>
        /// <remarks>
        /// Similar to FindBestSoldierFor, except we ONLY try to swap with a soldier
        /// of the same grade!
        /// </remarks>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        /// <returns>
        /// true if a soldier swaped positions, false otherwise
        /// </returns>
        private bool TryPerformLateralMovement(SoldierWrapper soldier, PositionWrapper position)
        {
            // Define position specific vars
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType type = position.Billet.Rank.Type;
            int grade = position.Billet.Rank.Grade;
            BilletSelection[] illegalSelections = { BilletSelection.CustomGenerator, BilletSelection.PromotionOnly };

            // Can't lateral into this position if it uses a custom
            // selection generator or is PromotionOnly!
            if (illegalSelections.Contains(position.Billet.Selection))
                return false;

            // If this is lateral ONLY position, than we MUST force
            // people to move from higher stature units, otherwise the position
            // could be empty forver!
            int val = (position.Billet.Selection == BilletSelection.LateralOnly) ? 0 : 1;

            // Get a list of soldiers that CAN do a lateral movement,
            // Order by Desirability and Time in Billet
            var soldiers = topUnit.SoldiersByGrade[type][grade].Values
                .Where(x => IsCanidateForPosition(x, position) && GetLateralPromotionDesire(x, position) > val)
                .OrderByDescending(x => GetLateralPromotionDesire(x, position))
                .ThenByDescending(x => x.GetTimeInBillet(CurrentIterationDate));

            // Any canidates?
            if (soldiers.Count() == 0)
                return false;

            // Now find someone we can switch with!
            foreach (var otherSoldier in soldiers)
            {
                // get soldier position
                var pos = otherSoldier.Position;

                // We cannot lateral INTO a position that uses a SoldierGenerator
                // or is PromotionOnly entry
                if (illegalSelections.Contains(pos.Billet.Billet.Selection))
                {
                    continue;
                }
                else if (pos.Billet.Billet.Selection == BilletSelection.LateralOnly)
                {
                    // We cannot lateral into a position if the soldier
                    // position does not match
                    if (position.Billet.Rank.Grade != pos.Billet.Rank.Grade)
                        continue;
                }

                // Make sure we are a canidate!
                if (IsCanidateForPosition(soldier, pos))
                {
                    // Remove positions first!
                    otherSoldier.RemoveFromPosition(CurrentIterationDate, Database);
                    soldier.RemoveFromPosition(CurrentIterationDate, Database);

                    // Assign new positions
                    soldier.AssignPosition(pos, CurrentIterationDate, Database);
                    otherSoldier.AssignPosition(position, CurrentIterationDate, Database);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method is used to iterate through all of the soldiers in a promotion pool,
        /// and returns the most qualified soldier for the specified <see cref="PositionWrapper"/>.
        /// </summary>
        /// <remarks>
        /// This method will start from the highest ranked soldiers first, and work its way downwards
        /// until it finds someone who can fill the slot. If absolutly no soldiers can fill the slot,
        /// null is returned.
        /// </remarks>
        /// <param name="position">The open position we are trying to fill</param>
        /// <returns></returns>
        private SoldierWrapper FindBestSoldierFor(PositionWrapper position)
        {
            // Define position specific vars
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType type = position.Billet.Rank.Type;

            // If promotion only, reduce the Grade level down one
            // to prevent any lateral movement
            int grade = (position.Billet.Selection == BilletSelection.PromotionOnly)
                ? position.Billet.Rank.Grade - 1
                : position.Billet.Rank.Grade;

            // Keep searching until we either find a soldier to
            // fill the slot, or run out of rank/grades to pull from.
            while (grade > 0)
            {
                //
                // === Apply Filters === //
                //
                // TODO: Apply filters to force lateral promotions
                //
                IOrderedEnumerable<SoldierWrapper> soldiers;
                IEnumerable<SoldierWrapper> primeSoldiers = new List<SoldierWrapper>();
                bool isLateral = (grade == position.Billet.Rank.Grade);

                // Quit if this is a lateral only position
                if (!isLateral && position.Billet.Selection == BilletSelection.LateralOnly)
                    break;

                // Lateral movement or promotion? The filtering is different!
                if (isLateral)
                {
                    // If this is lateral ONLY position, than we MUST force
                    // people to move from higher stature units, otherwise the position
                    // could be empty forver!
                    int val = (position.Billet.Selection == BilletSelection.LateralOnly) ? 0 : 1;

                    // Apply initial filter
                    primeSoldiers = topUnit.SoldiersByGrade[type][grade].Values
                        .Where(x => IsCanidateForPosition(x, position) && GetLateralPromotionDesire(x, position) > val);

                    // Is this position NOT repeatable?
                    if (!position.Billet.Billet.Repeatable)
                    {
                        soldiers = primeSoldiers
                            .Where(x => !x.BilletsHeld.ContainsKey(position.Billet.Billet.Id))
                            .OrderByDescending(x => GetLateralPromotionDesire(x, position))
                            .ThenByDescending(x => x.GetTimeInBillet(CurrentIterationDate));
                    }
                    else if (position.Billet.Billet.PreferNonRepeats)
                    {
                        // Check for repeatable preference
                        soldiers = primeSoldiers
                            .OrderBy(x => x.BilletsHeld.ContainsKey(position.Billet.Billet.Id) ? 1 : 0)
                            .ThenByDescending(x => GetLateralPromotionDesire(x, position))
                            .ThenByDescending(x => x.GetTimeInBillet(CurrentIterationDate));
                    }
                    else
                    {
                        // Final filtering
                        soldiers = primeSoldiers
                            .OrderByDescending(x => GetLateralPromotionDesire(x, position))
                            .ThenByDescending(x => x.GetTimeInBillet(CurrentIterationDate));
                    }
                }
                else
                {
                    // Apply initial filter
                    primeSoldiers = topUnit.SoldiersByGrade[type][grade].Values
                        .Where(x => IsCanidateForPosition(x, position));

                    // Is this position NOT repeatable?
                    if (!position.Billet.Billet.Repeatable)
                    {
                        soldiers = primeSoldiers
                            .Where(x => !x.BilletsHeld.ContainsKey(position.Billet.Billet.Id))
                            .OrderByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
                    }
                    else if (position.Billet.Billet.PreferNonRepeats)
                    {
                        // Check for repeatable preference
                        soldiers = primeSoldiers
                            .OrderBy(x => x.BilletsHeld.ContainsKey(position.Billet.Billet.Id) ? 1 : 0)
                            .ThenByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
                    }
                    else
                    {
                        // Final filtering
                        soldiers = primeSoldiers
                            .OrderByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
                    }
                }

                // Check for an un-restricted soldier first
                var wrapper = soldiers.FirstOrDefault();
                if (wrapper != null)
                {
                    return wrapper;
                }
                else
                {
                    --grade;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a desire rating for a soldier that is a candidate for a lateral promotion.
        /// A higher returned number indicates a greater need or desire for a later promotion.
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position">The position we are potentially moving into</param>
        /// <returns></returns>
        private int GetLateralPromotionDesire(SoldierWrapper soldier, PositionWrapper position)
        {
            //
            // CAN WE EVEN?
            //
            if (soldier.IsLockedInPosition(CurrentIterationDate))
            {
                if (!soldier.Position.Billet.Billet.CanLateralEarly)
                    return 0;
            }

            //
            // DO WE NEED TO?
            //

            // If we are getting close to our maximum tour length,
            // or we have surpassed our max tour length, return true
            if (soldier.IsNearMaxTourLength(CurrentIterationDate))
            {
                if (soldier.Position.Billet.Billet.Repeatable)
                {
                    // If the spot prefers none repeats, then we must
                    // return mid-level desire and Respect that!
                    if (soldier.Position.Billet.Billet.PreferNonRepeats)
                    {
                        return 5;
                    }

                    // The position is repeatable, and does not mind
                    // Repeat soldiers taking the spot...
                    // Is this position more desirable at least?
                    else if (soldier.Position.Billet.Stature < position.Billet.Stature)
                    {
                        return 4;
                    }
                    else
                    {
                        // We'll take it just for a change of scenery
                        return 3;
                    }
                }
                else
                {
                    // We are nearing max tour length, and the position
                    // is NOT repeatable... shit! This is high candidacy
                    return (soldier.IsPastMaxTourLength(CurrentIterationDate)) ? 7 : 6;
                }
            }

            //
            // Do We WANT to?
            //

            // If the stature is higher, OF COURSE we want it!
            return (soldier.Position.Billet.Stature < position.Billet.Stature) ? 2 : 1;
        }

        /// <summary>
        /// This method determines if the specified soldier meets the requirements
        /// to accept the specified position
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected bool IsCanidateForPosition(SoldierWrapper soldier, PositionWrapper position)
        {
            // A soldier can only can move once per iteration!
            // Positions are ordered at the start of the simulation by
            // Grade and Stature anyways, so it works out
            if (soldier.Assignment.AssignedIteration == CurrentIterationDate.Id)
                return false;

            // Don't move to the same billet we already sitting in
            if (position.Billet.Id == soldier.Position.Billet.Id)
                return false;

            // Is there a MOS requirement?
            if (position.Billet.RequiredSpecialties.Length > 0)
            {
                if (position.Billet.RequiredSpecialties.Contains(soldier.Soldier.SpecialtyId))
                {
                    // If requirements are inversed, that means the soldier 
                    // MUST NOT have the required specialty to be a canidate!
                    if (position.Billet.Billet.InverseRequirements)
                    {
                        return false;
                    }
                }
                else if (!position.Billet.Billet.InverseRequirements)
                {
                    // Position required the specialty, but this soldier
                    // does not have it!
                    return false;
                }
            }

            // Check for repeatable position
            if (!position.Billet.Billet.Repeatable)
            {
                if (soldier.BilletsHeld.ContainsKey(position.Billet.Id))
                    return false;
            }

            // Quit if this is a lateral only position
            if (position.Billet.Billet.LateralOnly && (position.Billet.Rank.Grade != soldier.Rank.Grade))
            {
                return false;
            }

            // Check if we are under ranked, and if so, check for position lock
            if (soldier.IsStandIn())
            {
                // Is this position an even higher grade than what we have?
                if (position.Billet.Rank.Grade > soldier.Position.Billet.Rank.Grade)
                {
                    return true;
                }
                else
                {
                    // Never laterally move, when locked into a position, if we are a stand in
                    return (!soldier.IsLockedInPosition(CurrentIterationDate));
                }
            }

            // Are we locked into our current billet?
            if (soldier.IsLockedInPosition(CurrentIterationDate))
            {
                // is this a promotion?
                bool isPromotion = (soldier.Rank.Grade < position.Billet.Rank.Grade);
                bool isLateral = (soldier.Rank.Grade == position.Billet.Rank.Grade);
                if (isPromotion && soldier.Position.Billet.Billet.CanBePromotedEarly)
                    return true;
                else if (isLateral && soldier.Position.Billet.Billet.CanLateralEarly)
                    return true;
                else
                    return false;
            }

            // if we are here, we meet all requirements!
            return true;
        }

        private void LogRetiree(SoldierWrapper soldier)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = soldier.Rank.Grade;
                var type = soldier.Rank.Type;
                int specId = soldier.Soldier.SpecialtyId;
                UnitWrapper parentUnit = soldier.Position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitTemplateId;
                    RankStatistics[templateId][type][grade].TrackRetiree(soldier, CurrentIterationDate);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackRetiree(soldier, CurrentIterationDate);

                    // Move up
                    parentUnit = parentUnit.Parent;
                }
            }
        }

        private void LogPromotion(SoldierWrapper soldier)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = soldier.Rank.Grade;
                var type = soldier.Rank.Type;
                int specId = soldier.Soldier.SpecialtyId;
                UnitWrapper parentUnit = soldier.Position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitTemplateId;
                    RankStatistics[templateId][type][grade].TrackPromotionToNextGrade(soldier, CurrentDate);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionToNextGrade(soldier, CurrentDate);

                    // Move up
                    parentUnit = parentUnit.Parent;
                }
            }
        }

        /// <summary>
        /// Initially populates the soldiers at the start of a simulation
        /// </summary>
        private void PopulateSoldiers()
        {
            // Get all specialties
            var specialties = Database.Specialties.ToArray();

            // Create our soldier dictionaries
            ActiveDutySoldiers = new Dictionary<int, SoldierWrapper>();
            RankStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>>();
            SpecialtyStatistics = new Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>>();

            // Build our Statistics Dictionaries
            foreach (UnitTemplate template in Database.UnitTemplates)
            {
                RankStatistics.Add(template.Id, new Dictionary<RankType, Dictionary<int, RankGradeStatistics>>());
                SpecialtyStatistics.Add(template.Id, new Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>());

                // Soldiers
                foreach (RankType type in RankTypes)
                {
                    // Initiate data
                    RankStatistics[template.Id].Add(type, new Dictionary<int, RankGradeStatistics>());
                    SpecialtyStatistics[template.Id].Add(type, new Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>());

                    // Add all grades
                    var range = RankCache.GetRankGradesByType(type);
                    for (int i = range.Minimum; i <= range.Maximum; i++)
                    {
                        RankStatistics[template.Id][type].Add(i, new RankGradeStatistics()
                        {
                            UnitTemplateId = template.Id,
                            RankGrade = i,
                            RankType = type
                        }
                        );

                        var dict = new Dictionary<int, SpecialtyGradeStatistics>();
                        foreach (Specialty spec in specialties)
                        {
                            dict.Add(spec.Id, new SpecialtyGradeStatistics()
                            {
                                UnitTemplateId = template.Id,
                                SpecialtyId = spec.Id,
                                RankGrade = i,
                                RankType = type
                            }
                            );
                        }

                        SpecialtyStatistics[template.Id][type].Add(i, dict);
                    }
                }
            }

            // Grab the most generic entry level generator!
            var s = SoldierGenerators.Values.Where(x => x.CreatesNewSoldiers && x.NewSoldierProbability == 100).FirstOrDefault();
            if (s == null || s == default(SoldierGenerator))
                throw new Exception("No SoldierGenerator created that has a 100% NEW soldier spawn probabilty");

            // Build initial roster
            foreach (var pos in Positions)
            {
                // Only fill entry positions!
                if (pos.Billet.UsesCustomGenerator && Settings.ProcessRankType(pos.Billet.Rank.Type))
                {
                    var soldier = CreateSoldier(s, pos);
                    soldier.AssignPosition(pos, CurrentIterationDate, Database);
                }
            }
        }

        /// <summary>
        /// Creates a new soldier based on the parameters provided
        /// </summary>
        /// <param name="grade">The rank level this new soldier will start out as</param>
        /// <param name="type">Indicates the rank classification for this soldier</param>
        /// <returns></returns>
        private SoldierWrapper CreateSoldier(SoldierGenerator generator, PositionWrapper position)
        {
            // Ensure the postion is empty! IF not, we will have soldiers
            // floating around never ever moving
            if (position.Holder != null)
                throw new ArgumentException("Position holder must be null before creating a new soldier!", "position");

            // Spawn a soldier generator setting
            SpawnedSoldier setting = generator.Spawn();
            SoldierWrapper wrapper = null;

            // If rankId is 0, then we create new
            if (setting.Type == SpawnSoldierType.CreateNew)
            {
                // Create a new soldier object
                Soldier soldier = new Soldier
                {
                    FirstName = NameGenerator.GenerateRandomFirstName(),
                    LastName = NameGenerator.GenerateRandomLastName(),
                    EntryIterationId = CurrentIterationDate.Id,
                    LastPromotionIterationId = CurrentIterationDate.Id,
                    RankId = position.Billet.Rank.Id,
                    SpecialtyId = position.Billet.Specialty.Id
                };

                // Ensure we initialized
                if (!setting.Career.IsInitialized)
                    setting.Career.Initialize();

                // Assign the soldiers new career length based on Rank Type
                CareerLengthRange career = setting.Career.Spawn();
                soldier.SpawnRateId = career.Id;
                soldier.ExitIterationId = CurrentIterationDate.Id + career.GenerateLength();

                // Add soldier to database
                Database.Soldiers.Add(soldier);

                // Create soldier wrapper
                wrapper = new SoldierWrapper(soldier, position.Billet.Rank, CurrentIterationDate, Database);
                ActiveDutySoldiers.Add(soldier.Id, wrapper);
            }
            else
            {
                // Fetch promotion pool
                UnitWrapper topUnit = position.PromotionPoolUnit;

                // ---------------------------
                // Apply Filters
                // ---------------------------
                IEnumerable<SoldierWrapper> soldiers = topUnit.SoldiersByGrade[setting.Rank.Type][setting.Rank.Grade].Values.ToList();
                if (!setting.UseRankGrade)
                    soldiers = soldiers.Where(x => x.Rank.Id == setting.Rank.Id);

                // Find best soldier for lateral movement
                wrapper = FindCrossPoolSoldier(soldiers, setting, position);

                // Grab our top filtered soldier
                if (wrapper != null)
                {
                    // Promote soldier
                    wrapper.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);

                    // Change career length?
                    if (setting.Career != null)
                    {
                        // Assign the soldiers new career length based on Rank Type
                        CareerLengthRange career = setting.Career.Spawn();
                        wrapper.Soldier.SpawnRateId = career.Id;
                        wrapper.Soldier.ExitIterationId = CurrentIterationDate.Id + career.GenerateLength();
                    }
                }
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
            IEnumerable<SoldierWrapper> soldiers, 
            SpawnedSoldier setting, 
            PositionWrapper position)
        {
            // Apply soldier ordering
            var list = new List<SoldierWrapper>(soldiers);
            if (SoldierPoolSorting.ContainsKey(setting.Pool.Id))
            {
                var oList = list.OrderBy(x => 1);
                var items = SoldierPoolSorting[setting.Pool.Id];
                foreach (var sort in items)
                {
                    oList = OrderSoldierList(oList, sort.SortBy, sort.Direction);
                }

                list = oList.ToList();
            }

            PromotableStatus status;
            foreach (var soldier in list)
            {
                // Must be promotable?
                if (setting.Pool.MustBePromotable)
                {
                    if (!soldier.IsPromotable(CurrentIterationDate, out status) || status == PromotableStatus.Lateral)
                        continue;
                }

                // Soldier locked into position?
                if (soldier.Position != null && setting.Pool.NotLockedInBillet && soldier.IsLockedInPosition(CurrentIterationDate))
                {
                    continue;
                }

                // Make sure the soldier can even sit in this position!
                if (IsCanidateForPosition(soldier, position))
                    return soldier;
            }

            return null;
        }

        /// <summary>
        /// This method is used to sort soldiers using the given SoldierSorting and direction
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private IOrderedEnumerable<SoldierWrapper> OrderSoldierList(
            IOrderedEnumerable<SoldierWrapper> list,
            SoldierSorting sortBy,
            Sorting direction)
        {
            switch (sortBy)
            {
                default:
                    return list;
                case SoldierSorting.TimeInGrade:
                    return (direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInGrade(CurrentIterationDate))
                        : list.ThenByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
                case SoldierSorting.TimeInService:
                    return (direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInService(CurrentIterationDate))
                        : list.ThenByDescending(x => x.GetTimeInService(CurrentIterationDate));
                case SoldierSorting.TimeInBillet:
                    return (direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeInBillet(CurrentIterationDate))
                        : list.ThenByDescending(x => x.GetTimeInBillet(CurrentIterationDate));
                case SoldierSorting.TimeToRetirement:
                    return (direction == Sorting.Ascending)
                        ? list.ThenBy(x => x.GetTimeUntilRetirement(CurrentIterationDate))
                        : list.ThenByDescending(x => x.GetTimeUntilRetirement(CurrentIterationDate));
            }
        }
    }
}
