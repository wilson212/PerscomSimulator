using CrossLite;
using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Perscom
{
    public class Simulator : IDisposable
    {
        protected bool IsDisposed { get; set; }

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
        /// [BilletId => BilletStatistics]
        /// </summary>
        public Dictionary<int, BilletStatistics> BilletStatistics
        {
            get;
            protected set;
        }

        /// <summary>
        /// [PositionId => PositionStatistics]
        /// </summary>
        public Dictionary<int, PositionStatistics> PositionStatistics
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
        /// The total number of Iterations the simulation ran
        /// </summary>
        protected int Iteration { get; set; } = 1;

        /// <summary>
        /// The total number of Iterations the simulation ran
        /// </summary>
        protected int StopIteration { get; set; } = 1;

        /// <summary>
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        protected RandomNameGenerator NameGenerator { get; set; }

        /// <summary>
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes { get; set; } = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        /// <summary>
        /// A Cache to hold Soldier Pool Sorting options.
        /// </summary>
        protected Dictionary<int, List<SoldierPoolSorting>> SoldierPoolOrdering { get; set; }


        /// <summary>
        /// A Cache to hold Soldier Pool Filtering options.
        /// </summary>
        protected Dictionary<int, List<SoldierPoolFilter>> SoldierPoolFiltering { get; set; }

        /// <summary>
        /// A Cache to hold Billet Experience Given.
        /// </summary>
        protected Dictionary<int, List<BilletExperience>> BilletExperience { get; set; }

        /// <summary>
        /// A Cache to hold Soldier Billet Experience Filtering options.
        /// </summary>
        protected Dictionary<int, List<BilletExperienceFilter>> ExperienceFilters { get; set; }

        /// <summary>
        /// A Cache to hold Soldier Billet Experience Group options.
        /// </summary>
        protected Dictionary<int, List<BilletExperienceGroup>> ExperienceGroups { get; set; }

        /// <summary>
        /// A Cache to hold Soldier Billet Experience Sorting options.
        /// </summary>
        protected Dictionary<int, List<BilletExperienceSorting>> ExperienceOrdering { get; set; }

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

            // Load and Cache Soldier Pool Sortings and Filterings!
            SoldierPoolFiltering = new Dictionary<int, List<SoldierPoolFilter>>();
            SoldierPoolOrdering = new Dictionary<int, List<SoldierPoolSorting>>();
            foreach (var item in db.SoldierGeneratorPools)
            {
                var sorts = item.SoldierSorting?.ToArray();
                var filters = item.SoldierFiltering?.ToArray();

                if (sorts != null && sorts.Length > 0)
                {
                    SoldierPoolOrdering.Add(item.Id, sorts.OrderBy(x => x.Precedence).ToList());
                }

                if (filters != null && filters.Length > 0)
                {
                    SoldierPoolFiltering.Add(item.Id, filters.OrderBy(x => x.Precedence).ToList());
                }
            }

            // Load and Cache Billet experience options
            BilletExperience = new Dictionary<int, List<BilletExperience>>();
            ExperienceFilters = new Dictionary<int, List<BilletExperienceFilter>>();
            ExperienceOrdering = new Dictionary<int, List<BilletExperienceSorting>>();
            ExperienceGroups = new Dictionary<int, List<BilletExperienceGroup>>();
            foreach (var item in db.Billets)
            {
                // Check for experience given
                var items = item.Experience.ToList();
                if (items.Count > 0)
                {
                    BilletExperience.Add(item.Id, items);
                }

                // Check for filters
                var filters = item.Filters.OrderBy(x => x.Precedence).ToList();
                if (filters.Count() > 0)
                {
                    ExperienceFilters.Add(item.Id, filters);
                }

                // Check for Grouping
                var groups = item.Grouping.OrderBy(x => x.Precedence).ToList();
                if (groups.Count() > 0)
                {
                    ExperienceGroups.Add(item.Id, groups);
                }

                // Check for Sorting
                var sorting = item.Sorting.OrderBy(x => x.Precedence).ToList();
                if (sorting.Count() > 0)
                {
                    ExperienceOrdering.Add(item.Id, sorting);
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
                throw new ArgumentException("totalYears cannot be less than skipYears", "totalYears");

            // First, set the end date
            SkipYears = skipYears;
            TotalYearsRan = 0;
            StopIteration = totalYears * 12;

            // Set initial dates
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            StartDate = EndDate.AddYears(-totalYears).AddMonths(1);
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
                    .OrderByDescending(x => x.Billet.Rank.Type)
                    .ThenByDescending(x => x.Billet.Rank.Grade)
                    .ThenByDescending(x => x.Billet.Stature)
                    .ToList();

                // Update progress
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

                // Create a stopwatch
                Stopwatch timer = new Stopwatch();
                timer.Start();

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

                        // Get esitmated time left
                        string message = $"Processing {name} of year {TotalYearsRan + 1} of {totalYears}\n\n";
                        if (timer.Elapsed.TotalSeconds >= 120)
                        {
                            message += String.Format(
                                "Time Elapsed: {0:mm\\:ss}; Esitmated Time Remaining: {1:mm\\:ss}",
                                timer.Elapsed,
                                timer.GetEta(Iteration, StopIteration)
                            );
                        }
                        else
                        {
                            message += String.Format("Time Elapsed: {0:mm\\:ss}", timer.Elapsed);
                        }

                        // Update progress window
                        name = CurrentDate.ToString("MMMM");
                        update = new TaskProgressUpdate();
                        update.MessageText = message;
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
                        if (!s.Soldier.Retired)
                        {
                            s.Save(Database, CurrentIterationDate);
                            s.Dispose();
                        }
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

                    // Save billet stats data
                    foreach (var stat in BilletStatistics.Values)
                        Database.BilletStatistics.Add(stat);

                    // Save positional stats data
                    foreach (var stat in PositionStatistics.Values)
                        Database.PositionStatistics.Add(stat);

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
            EntityCache.GetTableMap(typeof(SoldierExperience)).BuildInstanceForeignKeys = enabled;
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

                // Log position data
                LogPositionState(position);

                // Grab current holder
                bool expGiven = false;
                SoldierWrapper soldier = position.Holder;
                RankType type = position.Billet.Rank.Type;
                PromotableStatus status;

                // Label to return to if a soldier retired from this position this month
                DoPositionCheck:
                {
                    // Check if position is empty
                    if (position.IsEmpty)
                    {
                        // Is this an entry level position?
                        if (position.Billet.UsesCustomGenerator)
                        {
                            var generator = SoldierGenerators[position.Billet.SpawnSetting.GeneratorId];
                            soldier = CreateSoldier(generator, position, out SpawnedSoldier setting);

                            // Did we spawn a soldier?
                            if (soldier != null)
                            {
                                // Give past months experience
                                if (setting.Type != SpawnSoldierType.CreateNew && soldier.Position != null)
                                {
                                    GiveSoldierExperience(soldier, soldier.Position);
                                    expGiven = true;
                                }

                                // Assign position
                                AssignPositionToSoldier(soldier, position);

                                // Create entry into statistics
                                if (setting.Type == SpawnSoldierType.CreateNew)
                                    LogSoldierEntry(soldier);
                            }
                        }
                        else
                        {
                            // Grab soldier pool
                            soldier = FindBestSoldierFor(position);
                            if (soldier != null)
                            {
                                // Give past months experience!
                                GiveSoldierExperience(soldier, soldier.Position);
                                expGiven = true;

                                // Assign new position
                                AssignPositionToSoldier(soldier, position);
                            }
                        }
                    }
                    else if (!expGiven)
                    {
                        // soldier could be null if a Lateral movement failed
                        soldier = position.Holder;

                        // Give Experience
                        GiveSoldierExperience(soldier, position);

                        // Process Lateral Movement
                        if (position.Billet.MaxTourLength > 0)
                        {
                            // If we are at max tour length, or past it,
                            // we are a prime candidate for moving!
                            if (soldier.IsPastMaxTourLength(CurrentIterationDate))
                            {
                                TryPerformLateralMovement(soldier, position);
                            }
                        }
                    }

                    // If we could not find someone to fill the position
                    if (position.IsEmpty)
                    {
                        LogDeficit(position);
                        continue;
                    }

                    // Define soldier vars
                    int grade = soldier.Rank.Grade;
                    int specId = soldier.Soldier.SpecialtyId;

                    // Check for retirement (forced by MaxTourLength, MaxTiG, or freewill)
                    if (soldier.IsRetiring(CurrentIterationDate))
                    {
                        // Log retirement data for current rank/grade
                        LogRetiree(soldier);
                        LogPositionEmptied(position);

                        // Remove the retired soldier from the roster
                        ActiveDutySoldiers.Remove(soldier.Soldier.Id);

                        // Say goodbye!
                        soldier.Retire(CurrentIterationDate, Database);

                        // Update soldier record
                        soldier.Save(Database, CurrentIterationDate);

                        // Call dispose!
                        soldier.Dispose();
                        soldier = null;

                        // Go back and fill this position!
                        goto DoPositionCheck;
                    }

                    // Check for auto promotion and under-staff promotion
                    else if (soldier.IsPromotable(CurrentIterationDate, out status))
                    {
                        if (status == PromotableStatus.Automatic || status == PromotableStatus.Position)
                        {
                            // Dont log promotion data if switching rank types!
                            if (soldier.Rank.Type != position.Billet.Rank.Type)
                            {
                                // Log transfer data for current rank/grade
                                LogRankTypeChange(soldier, position);

                                // Promote Soldier
                                soldier.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);
                            }
                            else
                            {
                                // Get the expected soldier rank/grade to promote from
                                var expectedGrade = position.Billet.Rank.Grade - 1;

                                // Promote soldier. Do not skip rank grades!
                                if (soldier.Rank.Grade == expectedGrade)
                                {
                                    // Log promotion data for current rank/grade
                                    LogPromotion(soldier);

                                    // Billet grade is 1 level higher or of a different Type!
                                    soldier.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);
                                }
                                else
                                {
                                    // Log promotion data for current rank/grade
                                    LogPromotion(soldier);

                                    // Billet grade is multiple levels higher, SO we must promote one grade at
                                    // a time!
                                    Rank toRank = RankCache.GetNextGradeRanks(soldier.Rank).FirstOrDefault();
                                    if (toRank != null)
                                        soldier.PromoteTo(CurrentIterationDate, toRank, Database);
                                    else
                                        throw new Exception("Ran out of ranks? wtf");
                                }
                            }
                        }
                        else if (status == PromotableStatus.Lateral)
                        {
                            // Do not log promotion since this is lateral
                            soldier.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);
                        }
                    }
                    else if (soldier.IsStandIn())
                    {
                        LogDeficit(position);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns a soldier to a position, and logs the information
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        private void AssignPositionToSoldier(SoldierWrapper soldier, PositionWrapper position)
        {
            // Log outgoing
            if (soldier.Position != null)
            {
                LogPositionEmptied(soldier.Position);
            }

            // Assign the position
            soldier.AssignPosition(position, CurrentIterationDate, Database);

            // Log incoming
            LogPositionAssigned(position);
        }

        /// <summary>
        /// Gives a soldier experience
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        private void GiveSoldierExperience(SoldierWrapper soldier, PositionWrapper position)
        {
            if (BilletExperience.ContainsKey(position.Billet.Id))
            {
                var items = BilletExperience[position.Billet.Id];
                foreach (var item in items)
                {
                    soldier.GiveExperience(item);
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
        /// <param name="soldier">The position holder</param>
        /// <param name="position">Position the soldier is laterally moving OUT of</param>
        /// <returns>
        /// true if a soldier swaped positions, false otherwise
        /// </returns>
        private bool TryPerformLateralMovement(SoldierWrapper soldier, PositionWrapper position)
        {
            // Define position specific vars
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType type = position.Billet.Rank.Type;
            int grade = position.Billet.Rank.Grade;
            int billetId = position.Billet.Id;
            BilletSelection[] illegalSelections = { BilletSelection.CustomGenerator, BilletSelection.PromotionOnly };

            // Grab soldiers
            IEnumerable<SoldierWrapper> soldiers = topUnit.SoldiersByGrade[type][grade].Values.ToList();

            // Can't lateral into this position if it uses a custom
            // selection generator or is PromotionOnly!
            if (illegalSelections.Contains(position.Billet.Selection))
                return false;

            // If this is lateral ONLY position, than we MUST force
            // people to move from higher stature units, otherwise the position
            // could be empty forver!
            int val = (position.Billet.Selection == BilletSelection.LateralOnly) ? 3 : 2;

            // Get a list of soldiers that CAN do a lateral movement,
            soldiers = soldiers.Where(x => IsCanidateForPosition(x, position) && GetLateralPromotionGroupId(x, position) <= val);

            // Do we have anyone left?
            int count = soldiers.Count();
            if (count == 0)
            {
                return false;
            }

            // Apply grouping and sorting only if we have more than 1 soldier
            if (count > 1)
            {
                //
                // 2. Apply Billet grouping
                //
                IEnumerable<SoldierGroupResult> groups = null;

                // Do we have experience grouping as well?
                if (ExperienceGroups.ContainsKey(billetId))
                {
                    // Apply lateral desire grouping (Need, Want, Dont Want), Then By billet grouping
                    var expGroups = ExperienceGroups[billetId];
                    groups = soldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position), expGroups);
                }
                else
                {
                    groups = soldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position));
                }

                // Get topmost group with at least one soldier in it
                // This list of soldiers will be the most prime canidates,
                // hitting Most if Not All grouping requirements
                soldiers = groups.GetPrimeSoldiers();

                //
                // 3. Apply soldier ordering
                //
                if (ExperienceOrdering.ContainsKey(billetId))
                {
                    // Grab experience sorting
                    var experienceSorts = ExperienceOrdering[billetId];

                    // Apply sorting using just experience
                    soldiers = soldiers.OrderSoldiersBy(experienceSorts);
                }
                else
                {
                    // Apply sorting using just experience
                    soldiers = soldiers.OrderByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
                }
            }

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

                // Log!
                LogPositionEmptied(soldier.Position);
                LogPositionEmptied(otherSoldier.Position);

                // Remove positions first!
                otherSoldier.RemoveFromPosition(CurrentIterationDate, Database);
                soldier.RemoveFromPosition(CurrentIterationDate, Database);

                // Assign new positions
                soldier.AssignPosition(pos, CurrentIterationDate, Database);
                otherSoldier.AssignPosition(position, CurrentIterationDate, Database);

                // Log!
                LogPositionAssigned(soldier.Position);
                LogPositionAssigned(otherSoldier.Position);

                // Return success
                return true;
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
            IEnumerable<SoldierWrapper> primeSoldiers = null;
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType type = position.Billet.Rank.Type;
            var billetId = position.Billet.Billet.Id;

            // If promotion only, reduce the Grade level down one
            // to prevent any lateral movement
            int grade = (position.Billet.Selection == BilletSelection.PromotionOnly)
                ? position.Billet.Rank.Grade - 1
                : position.Billet.Rank.Grade;

            // Keep searching until we either find a soldier to
            // fill the slot, or run out of rank/grades to pull from.
            while (grade > 0)
            {
                // Grab soldier list
                primeSoldiers = topUnit.SoldiersByGrade[type][grade].Values;
                IOrderedEnumerable<SoldierWrapper> soldiers;
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
                    int val = (position.Billet.Selection == BilletSelection.LateralOnly) ? 3 : 2;

                    //
                    // 1. Apply initial filter, ensuring canadiatcy, and any kind of desire
                    //
                    primeSoldiers = primeSoldiers.Where(x => IsCanidateForPosition(x, position) && GetLateralPromotionGroupId(x, position) <= val);

                }
                else
                {
                    //
                    // 1. Apply initial filter, ensuring canadiatcy
                    //
                    primeSoldiers = primeSoldiers.Where(x => IsCanidateForPosition(x, position));
                }

                // Is this position NOT repeatable?
                if (!position.Billet.Billet.Waiverable)
                    primeSoldiers = primeSoldiers.Where(x => !x.BilletsHeld.ContainsKey(billetId));

                // Do we have anyone left?
                int count = primeSoldiers.Count();
                if (count == 0)
                {
                    --grade;
                    continue;
                }

                // Apply grouping and sorting only if we have more than 1 soldier
                if (count > 1)
                {
                    //
                    // 2. Apply Billet grouping
                    //
                    IEnumerable<SoldierGroupResult> groups = null;
                    if (isLateral)
                    {
                        // Do we have experience grouping as well?
                        if (ExperienceGroups.ContainsKey(billetId))
                        {
                            // Apply lateral desire grouping (Need, Want, Dont Want), Then By billet grouping
                            var expGroups = ExperienceGroups[billetId];
                            groups = primeSoldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position), expGroups);
                        }
                        else
                        {
                            groups = primeSoldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position));
                        }

                        // Get topmost group with at least one soldier in it
                        primeSoldiers = groups.GetPrimeSoldiers();
                    }
                    else if (ExperienceGroups.ContainsKey(billetId))
                    {
                        // Apply groups
                        var expGroups = ExperienceGroups[billetId];
                        groups = primeSoldiers.GroupSoldiersBy(expGroups.ToList());

                        // Get topmost group with at least one soldier in it
                        primeSoldiers = groups.GetPrimeSoldiers();
                    }

                    //
                    // 3. Apply soldier ordering
                    //
                    if (ExperienceOrdering.ContainsKey(billetId))
                    {
                        // Grab experience sorting
                        var experienceSorts = ExperienceOrdering[billetId];

                        // Apply sorting using just experience
                        soldiers = primeSoldiers.OrderSoldiersBy(experienceSorts);
                    }
                    else
                    {
                        // Apply sorting using just experience
                        soldiers = primeSoldiers.OrderByDescending(x => x.GetTimeInGrade(CurrentIterationDate));
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
                        continue;
                    }
                }
                else
                {
                    // Check for an un-restricted soldier first
                    var wrapper = primeSoldiers.FirstOrDefault();
                    if (wrapper != null)
                    {
                        return wrapper;
                    }
                    else
                    {
                        --grade;
                        continue;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a group rating for a soldier that is a candidate for a lateral promotion.
        /// A higher returned number indicates a reduced need or desire for a lateral promotion.
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position">The position we are potentially moving into</param>
        /// <returns></returns>
        /// <remarks>
        /// 4 = Soldier Cant really move...
        /// 3 = Downgrade position based on stature, Soldier doesn't want to
        /// 2 = Upgrade to current position, Soldier wants to
        /// 1 = Soldier should To move up very soon (Past MaxTourLength [Waiverable] or Very Near [Not Waiverable])
        /// 0 = Soldier needs to move NOW (Not Waiverable, Will be forced to retire)
        /// </remarks>
        private int GetLateralPromotionGroupId(SoldierWrapper soldier, PositionWrapper position)
        {
            //
            // CAN WE EVEN?
            //
            if (soldier.IsLockedInPosition(CurrentIterationDate))
            {
                if (!soldier.Position.Billet.Billet.CanLateralEarly)
                    return 4;
            }

            //
            // DO WE NEED TO?
            //

            // If we are getting close to our maximum tour length,
            // or we have surpassed our max tour length, return true
            if (soldier.IsNearMaxTourLength(CurrentIterationDate))
            {
                if (soldier.Position.Billet.Billet.Waiverable)
                {
                    // We'll take it just for a change of scenery
                    return (soldier.IsPastMaxTourLength(CurrentIterationDate)) ? 1 : 2;
                }
                else
                {
                    // We are nearing max tour length, and the position
                    // is NOT repeatable... shit! This is high candidacy
                    return (soldier.IsPastMaxTourLength(CurrentIterationDate)) ? 0 : 1;
                }
            }

            //
            // Do We WANT to?
            //

            // If the stature is higher, OF COURSE we want it!
            return (soldier.Position.Billet.Stature < position.Billet.Stature) ? 2 : 3;
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

            // Make sure we are not retiring THIS round!
            if (soldier.IsRetiring(CurrentIterationDate))
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

            // Apply Billet Filtering
            int billetId = position.Billet.Id;
            if (ExperienceFilters.ContainsKey(billetId))
            {
                var filters = ExperienceFilters[billetId];
                foreach (var filter in filters)
                {
                    if (!soldier.MeetsExperienceRequirement(filter))
                        return false;
                }
            }

            // Quit if this is a lateral only position
            if (position.Billet.Selection == BilletSelection.LateralOnly && (position.Billet.Rank.Grade != soldier.Rank.Grade))
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
            PositionStatistics = new Dictionary<int, PositionStatistics>(Positions.Count);
            BilletStatistics = new Dictionary<int, BilletStatistics>();


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
                    var soldier = CreateSoldier(s, pos, out SpawnedSoldier spawned);
                    soldier.AssignPosition(pos, CurrentIterationDate, Database);
                }

                // Create data
                PositionStatistics.Add(pos.Position.Id, new PositionStatistics() { PositionId = pos.Position.Id });
                if (!BilletStatistics.ContainsKey(pos.Billet.Id))
                    BilletStatistics.Add(pos.Billet.Id, new BilletStatistics() { BilletId = pos.Billet.Id });
            }
        }

        /// <summary>
        /// Creates a new soldier based on the parameters provided
        /// </summary>
        /// <param name="grade">The rank level this new soldier will start out as</param>
        /// <param name="type">Indicates the rank classification for this soldier</param>
        /// <returns></returns>
        private SoldierWrapper CreateSoldier(SoldierGenerator generator, PositionWrapper position, out SpawnedSoldier setting)
        {
            // Ensure the postion is empty! IF not, we will have soldiers
            // floating around never ever moving
            if (position.Holder != null)
                throw new ArgumentException("Position holder must be null before creating a new soldier!", "position");

            // Spawn a soldier generator setting
            setting = generator.Spawn();
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
                    FirstName = NameGenerator.GenerateRandomFirstName(),
                    LastName = NameGenerator.GenerateRandomLastName(),
                    EntryIterationId = CurrentIterationDate.Id,
                    LastPromotionIterationId = CurrentIterationDate.Id,
                    LastGradeChangeIterationId = CurrentIterationDate.Id,
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
                {
                    int rankId = setting.Rank.Id;
                    soldiers = soldiers.Where(x => x.Rank.Id == rankId);
                }

                // Find best soldier for lateral movement
                wrapper = FindCrossPoolSoldier(soldiers.ToList(), setting, position);

                // Grab our top filtered soldier
                if (wrapper != null && setting.Career != null)
                {
                    // Assign the soldiers new career length based on Rank Type
                    CareerLengthRange career = setting.Career.Spawn();
                    wrapper.Soldier.SpawnRateId = career.Id;
                    wrapper.Soldier.ExitIterationId = CurrentIterationDate.Id + career.GenerateLength();
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
        private SoldierWrapper FindCrossPoolSoldier(List<SoldierWrapper> soldiers, SpawnedSoldier setting, PositionWrapper position)
        {
            // Quit if we have no soldiers!
            if (soldiers.Count == 0)
                return null;

            // Setup variables
            var billetId = position.Billet.Id;
            IEnumerable<SoldierWrapper> candidates = soldiers;

            //
            // 1. Apply Soldier Pool filtering
            //
            if (SoldierPoolFiltering.ContainsKey(setting.Pool.Id))
            {
                var items = SoldierPoolFiltering[setting.Pool.Id];
                candidates = candidates.FilterSoldierList(items, setting.Pool.FilterLogic, CurrentIterationDate);
            }

            // 
            // 2. Billet Filtering by experience
            //
            if (ExperienceFilters.ContainsKey(billetId))
            {
                // Apply groups
                var filters = ExperienceFilters[billetId];
                candidates = candidates.FilterSoldierList(filters, position.Billet.Billet.ExperienceLogic);
            }

            // Do we have anyone left?
            int count = candidates.Count();
            if (count == 0)
                return null;

            // Apply grouping and sorting only if we have more than 1 soldier
            if (count > 1)
            {
                //
                // 3. Apply Billet grouping
                //
                if (ExperienceGroups.ContainsKey(billetId))
                {
                    // Apply groups
                    var groups = ExperienceGroups[billetId];
                    var result = candidates.GroupSoldiersBy(groups.ToList());

                    // Get topmost group with at least one soldier in it
                    // This list of soldiers will be the most prime canidates,
                    // hitting Most if Not All grouping requirements
                    candidates = result.GetPrimeSoldiers();
                }

                //
                // 4. Apply soldier ordering
                //
                if (ExperienceOrdering.ContainsKey(billetId))
                {
                    // Grab experience sorting
                    var experienceSorts = ExperienceOrdering[billetId];

                    // Check for pool sorting
                    if (SoldierPoolOrdering.ContainsKey(setting.Pool.Id))
                    {
                        // Grab pool sorts
                        var poolSorts = SoldierPoolOrdering[setting.Pool.Id];
                        IOrderedEnumerable<SoldierWrapper> ordered = null;

                        // Who sorts first?
                        if (setting.Pool.OrdersBeforeBilletOrdering)
                        {
                            // Apply time sorting
                            ordered = candidates.OrderSoldiersBy(poolSorts, CurrentIterationDate);

                            // Apply experience sorting
                            ordered = ordered.ThenOrderSoldiersBy(experienceSorts);
                        }
                        else
                        {
                            // Apply experience sorting
                            ordered = candidates.OrderSoldiersBy(experienceSorts);

                            // Apply time sorting
                            ordered = ordered.ThenOrderSoldiersBy(poolSorts, CurrentIterationDate);
                        }

                        // Finally, collapse
                        candidates = ordered.ToList();
                    }
                    else
                    {
                        // Apply sorting using just experience
                        candidates = candidates.OrderSoldiersBy(experienceSorts);
                    }
                }
                else if (SoldierPoolOrdering.ContainsKey(setting.Pool.Id))
                {
                    var poolSorts = SoldierPoolOrdering[setting.Pool.Id];
                    candidates = candidates.OrderSoldiersBy(poolSorts, CurrentIterationDate);
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

        private void LogRankTypeChange(SoldierWrapper soldier, PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int fromGrade = soldier.Rank.Grade;
                var fromType = soldier.Rank.Type;
                int fromSpecId = soldier.Soldier.SpecialtyId;
                int toGrade = position.Billet.Rank.Grade;
                var toType = position.Billet.Rank.Type;
                int toSpecId = position.Billet.Specialty.Id;
                UnitWrapper parentUnit = soldier.Position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitTemplateId;

                    // Outgoing
                    RankStatistics[templateId][fromType][fromGrade].TrackRankTransferFrom(soldier, CurrentIterationDate);
                    SpecialtyStatistics[templateId][fromType][fromGrade][fromSpecId].TrackRankTransferFrom(soldier, CurrentIterationDate);

                    // Incoming
                    RankStatistics[templateId][toType][toGrade].TrackRankTransferInto(soldier);
                    SpecialtyStatistics[templateId][toType][toGrade][toSpecId].TrackRankTransferInto(soldier);

                    // Move up
                    parentUnit = parentUnit.Parent;
                }
            }
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

        private void LogSoldierEntry(SoldierWrapper soldier)
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

                    RankStatistics[templateId][type][grade].TrackPromotionIntoGrade(soldier);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionIntoGrade(soldier);

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

                    // Track Promotion
                    RankStatistics[templateId][type][grade].TrackPromotionToNextGrade(soldier, CurrentDate);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionToNextGrade(soldier, CurrentDate);

                    // Add soldier to incoming on Next grade
                    RankStatistics[templateId][type][grade + 1].TrackPromotionIntoGrade(soldier);
                    SpecialtyStatistics[templateId][type][grade + 1][specId].TrackPromotionIntoGrade(soldier);

                    // Move up
                    parentUnit = parentUnit.Parent;
                }
            }
        }

        private void LogDeficit(PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = position.Billet.Rank.Grade;
                var type = position.Billet.Rank.Type;
                int specId = position.Billet.Specialty?.Id ?? -1;
                UnitWrapper parentUnit = position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitTemplateId;
                    RankStatistics[templateId][type][grade].Deficit += 1;

                    if (specId >= 0)
                        SpecialtyStatistics[templateId][type][grade][specId].Deficit += 1;

                    // Move up
                    parentUnit = parentUnit.Parent;
                }
            }
        }

        private void LogPositionState(PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                if (position.IsEmpty)
                {
                    PositionStatistics[position.Position.Id].EmptyDeficit += 1;
                    BilletStatistics[position.Billet.Id].EmptyDeficit += 1;
                }
                else if (position.Holder.IsStandIn())
                {
                    PositionStatistics[position.Position.Id].StandInDeficit += 1;
                    BilletStatistics[position.Billet.Id].StandInDeficit += 1;
                }
            }
        }

        private void LogPositionAssigned(PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                PositionStatistics[position.Position.Id].TotalSoldiersIncoming += 1;
                BilletStatistics[position.Billet.Id].TotalSoldiersIncoming += 1;
            }
        }

        private void LogPositionEmptied(PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                PositionStatistics[position.Position.Id].TotalSoldiersOutgoing += 1;
                BilletStatistics[position.Billet.Id].TotalSoldiersOutgoing += 1;
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                // Clear positions
                Positions.Clear();
                Positions = null;

                // Clear soldiers
                ActiveDutySoldiers.Clear();
                ActiveDutySoldiers = null;

                // Clear soldier generators
                SoldierGenerators.Clear();
                SoldierGenerators = null;

                // Clear everything!
                RankStatistics.Clear();
                RankStatistics = null;

                SpecialtyStatistics.Clear();
                SpecialtyStatistics = null;

                BilletStatistics.Clear();
                BilletStatistics = null;

                PositionStatistics.Clear();
                PositionStatistics = null;

                BilletExperience.Clear();
                BilletExperience = null;

                ExperienceFilters.Clear();
                ExperienceFilters = null;

                ExperienceGroups.Clear();
                ExperienceGroups = null;

                ExperienceOrdering.Clear();
                ExperienceOrdering = null;

                SoldierPoolFiltering.Clear();
                SoldierPoolFiltering = null;
            }
        }
    }
}