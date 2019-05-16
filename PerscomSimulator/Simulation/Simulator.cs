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
        /// <summary>
        /// Indicates whether this instance is disposed
        /// </summary>
        protected bool IsDisposed { get; set; }
        
        /// <summary>
        /// Contains the current Simulation Database instance
        /// </summary>
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
        /// Soldier Id => Soldier
        /// </summary>
        public Dictionary<int, SoldierWrapper> ActiveDutySoldiers { get; protected set; }

        /// <summary>
        /// Gets a list of all the <see cref="Position"/> entities
        /// </summary>
        public List<PositionWrapper> Positions { get; protected set; }

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
        /// Gets the root <see cref="Unit"/> that is processing in this Simulator instance
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
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes { get; set; } = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        /// <summary>
        /// A Cache to hold Billet Experience Given.
        /// </summary>
        protected Dictionary<int, List<BilletExperience>> BilletExperience { get; set; }

        /// <summary>
        /// Creates a new Simulator instance
        /// </summary>
        /// <param name="unit">The unit to run the simulation on</param>
        public Simulator(SimDatabase db, UnitWrapper unit, SimulatorSettings settings)
        {
            // Set instance properties
            Database = db;
            ProcessingUnit = unit;
            Settings = settings;

            // Load and Cache Billet experience options
            BilletExperience = new Dictionary<int, List<BilletExperience>>();
            foreach (var item in db.Billets)
            {
                // Check for experience given
                var items = item.Experience.ToList();
                if (items.Count > 0)
                {
                    BilletExperience.Add(item.Id, items);
                }
            }

            // Attach Events
            SoldierWrapper.OnLateralPositionExchange += SoldierWrapper_OnLateralPositionExchange;
            SoldierWrapper.OnPositionChange += SoldierWrapper_OnPositionChange;
            SoldierWrapper.OnPositionAndRankChange += SoldierWrapper_OnPositionAndRankChange;
            SoldierWrapper.OnRankGradeChange += SoldierWrapper_OnRankGradeChange;
            SoldierWrapper.OnRetire += SoldierWrapper_OnRetire;
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

                    // Wrap in a transaction to vastly speed up database interations this month!
                    using (var trans = Database.BeginTransaction())
                    {
                        // Update the date
                        if (Iteration > 1)
                        {
                            CurrentDate = CurrentDate.AddMonths(1);

                            // Create Iteration Date
                            CurrentIterationDate = new IterationDate() { Date = CurrentDate, Logged = (SkipYears == 0) };
                            Database.IterationDates.Add(CurrentIterationDate);
                        }

                        // Ensure we are not screwed up here
                        if (Iteration != CurrentIterationDate.Id)
                        {
                            throw new Exception("Date and Iteration dont match!");
                        }

                        // Get esitmated time left
                        string message = $"Processing {name} of year {TotalYearsRan + 1} of {totalYears}\n\n";
                        if (timer.Elapsed.TotalSeconds >= 60)
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
            EntityCache.GetTableMap(typeof(RankGradeStatistics)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(SpecialtyGradeStatistics)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(PastAssignment)).BuildInstanceForeignKeys = enabled;
            EntityCache.GetTableMap(typeof(Assignment)).BuildInstanceForeignKeys = enabled;
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

                // Label to return to if a soldier retired from this position this month
                DoPositionCheck:
                {
                    // Check if position is empty
                    if (position.IsEmpty)
                    {
                        // Create a board using the billet procedure
                        var board = position.Billet.Procedure;

                        // Use billet procedure to select new soldier
                        soldier = board.SelectCandidate(position, CurrentIterationDate, out SpawnSoldierType spawned);
                        if (soldier != null)
                        {
                            if (spawned == SpawnSoldierType.CreateNew)
                            {
                                // Assign position, do NOT fire event!
                                soldier.AssignPosition(position, CurrentIterationDate, Database, false);

                                // Add soldier to active duty roster
                                ActiveDutySoldiers.Add(soldier.Soldier.Id, soldier);

                                // Custom entry log
                                LogSoldierEntry(soldier);
                            }
                            else
                            {
                                // Give past months experience
                                if (soldier.Position != null)
                                {
                                    GiveSoldierExperience(soldier, soldier.Position);
                                    expGiven = true;
                                }

                                // Assign new position
                                soldier.AssignPosition(position, CurrentIterationDate, Database);
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
                                // Create a board using the billet procedure
                                var board = position.Billet.Procedure;

                                // Use billet procedure to select new soldier
                                var otherSoldier = board.FindLateralCandidate(position, CurrentIterationDate);
                                if (otherSoldier != null)
                                {
                                    soldier.ExchangePositionsWith(otherSoldier, CurrentIterationDate, Database);
                                }
                            }
                        }
                    }

                    // If we could not find someone to fill the position
                    if (position.IsEmpty)
                    {
                        LogDeficit(position);
                        continue;
                    }

                    // Check for retirement (forced by MaxTourLength, MaxTiG, or freewill)
                    if (soldier.IsRetiring(CurrentIterationDate))
                    {
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

                    // Check for auto promotion and under-staff promotions. Signal true for event firing
                    soldier.DoPromotionIfEligable(CurrentIterationDate, Database, true, out Promotion p);

                    // Log deficit if soldier is stand in
                    if (soldier.IsStandIn())
                    {
                        LogDeficit(position);
                    }
                }
            }
        }

        /// <summary>
        /// Gives a soldier the <see cref="Experience"/> items provided by the position
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
                        });

                        var dict = new Dictionary<int, SpecialtyGradeStatistics>();
                        foreach (Specialty spec in specialties)
                        {
                            dict.Add(spec.Id, new SpecialtyGradeStatistics()
                            {
                                UnitTemplateId = template.Id,
                                SpecialtyId = spec.Id,
                                RankGrade = i,
                                RankType = type
                            });
                        }

                        SpecialtyStatistics[template.Id][type].Add(i, dict);
                    }
                }
            }

            // Build initial roster
            foreach (var pos in Positions)
            {
                // Only fill entry positions!
                if (pos.Billet.CreatesNewSoldiers && Settings.ProcessRankType(pos.Billet.Rank.Type))
                {
                    var soldier = pos.Billet.Procedure.SelectCandidate(pos, CurrentIterationDate, out SpawnSoldierType type);
                    soldier.AssignPosition(pos, CurrentIterationDate, Database, false);

                    // Add to active duty list
                    ActiveDutySoldiers.Add(soldier.Soldier.Id, soldier);
                }

                // Create data
                PositionStatistics.Add(pos.Position.Id, new PositionStatistics() { PositionId = pos.Position.Id });
                if (!BilletStatistics.ContainsKey(pos.Position.BilletId))
                    BilletStatistics.Add(pos.Position.BilletId, new BilletStatistics() { BilletId = pos.Position.BilletId });
            }
        }

        #region Events and Logging

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

                // Update position statistics
                var pos = PositionStatistics[soldier.Position.Position.Id];
                pos.TotalSoldiersIncoming += 1;

                var bill = BilletStatistics[soldier.Position.Position.BilletId];
                bill.TotalSoldiersIncoming += 1;
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
                    BilletStatistics[position.Position.BilletId].EmptyDeficit += 1;
                }
                else if (position.Holder.IsStandIn())
                {
                    PositionStatistics[position.Position.Id].StandInDeficit += 1;
                    BilletStatistics[position.Position.BilletId].StandInDeficit += 1;
                }
            }
        }

        private void SoldierWrapper_OnLateralPositionExchange(object sender, LateralPositionExchangeEventArgs e)
        {
            if (SkipYears == 0)
            {
                // Soldier 1
                if (e.Soldier1EventArgs is PositionAndRankChangeEventArgs)
                {
                    SoldierWrapper_OnPositionAndRankChange(
                        e.Soldier1EventArgs.Soldier,
                        e.Soldier1EventArgs as PositionAndRankChangeEventArgs
                    );
                }
                else
                {
                    SoldierWrapper_OnPositionChange(e.Soldier1EventArgs.Soldier, e.Soldier1EventArgs);
                }

                // Soldier 2
                if (e.Soldier2EventArgs is PositionAndRankChangeEventArgs)
                {
                    SoldierWrapper_OnPositionAndRankChange(
                        e.Soldier2EventArgs.Soldier,
                        e.Soldier2EventArgs as PositionAndRankChangeEventArgs
                    );
                }
                else
                {
                    SoldierWrapper_OnPositionChange(e.Soldier2EventArgs.Soldier, e.Soldier2EventArgs);
                }
            }
        }

        private void SoldierWrapper_OnPositionAndRankChange(object sender, PositionAndRankChangeEventArgs e)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                // Grab ranks. Remember Foriegn keys are disabled
                Rank fromRank = RankCache.RanksById[e.Promotion.FromRankId];
                Rank toRank = RankCache.RanksById[e.Promotion.ToRankId];

                // Determine what happened here, and log accordingly
                bool isRankTypeChange = (fromRank.Type != toRank.Type);
                bool isLateral = (fromRank.Grade == toRank.Grade);
                bool isGradePromotion = (fromRank.Grade < toRank.Grade);

                // Grab soldier vars
                var soldier = e.Soldier;
                int specId = e.FromSpecialty.Id;
                UnitWrapper fromParentUnit = e.FromPosition.ParentUnit;
                UnitWrapper toParentUnit = e.ToPosition.ParentUnit;

                // Log
                if (isRankTypeChange)
                {
                    int fromGrade = fromRank.Grade;
                    var fromType = fromRank.Type;
                    int toGrade = toRank.Grade;
                    var toType = toRank.Type;
                    int toSpecId = e.ToPosition.Billet.Specialty.Id;

                    // Transfer out statistics
                    while (fromParentUnit != null)
                    {
                        var templateId = fromParentUnit.Unit.UnitTemplateId;

                        // Outgoing
                        RankStatistics[templateId][fromType][fromGrade].TrackRankTransferFrom(e, CurrentIterationDate);
                        SpecialtyStatistics[templateId][fromType][fromGrade][specId].TrackRankTransferFrom(e, CurrentIterationDate);

                        // Move up
                        fromParentUnit = fromParentUnit.Parent;
                    }

                    // Transfer into statistics
                    while (toParentUnit != null)
                    {
                        var templateId = toParentUnit.Unit.UnitTemplateId;

                        // Incoming
                        RankStatistics[templateId][toType][toGrade].TrackRankTransferInto(soldier);
                        SpecialtyStatistics[templateId][toType][toGrade][toSpecId].TrackRankTransferInto(soldier);

                        // Move up
                        toParentUnit = toParentUnit.Parent;
                    }

                    // In Position
                    var pos = PositionStatistics[e.ToPosition.Position.Id];
                    pos.TotalSoldiersIncoming += 1;
                    pos.TotalSoldiersTransferredIn += 1;
                    pos.TotalMonthsInGradeIncoming += 0;
                    pos.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    var bill = BilletStatistics[e.ToPosition.Position.BilletId];
                    bill.TotalSoldiersIncoming += 1;
                    bill.TotalSoldiersTransferredIn += 1;
                    bill.TotalMonthsInGradeIncoming += 0;
                    bill.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    // Out Position
                    pos = PositionStatistics[e.FromPosition.Position.Id];
                    pos.TotalSoldiersOutgoing += 1;
                    pos.TotalSoldiersTransferredOut += 1;
                    pos.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    bill = BilletStatistics[e.FromPosition.Position.BilletId];
                    bill.TotalSoldiersOutgoing += 1;
                    bill.TotalSoldiersTransferredOut += 1;
                    bill.TotalMonthsInPosition += e.FromPositionTimeInBillet;
                }
                else if (isLateral)
                {
                    // Just fire off the OnPositionChange Event
                    SoldierWrapper_OnPositionChange(sender, e);
                }
                else if (isGradePromotion)
                {
                    // Log grade promotion for Rank and Specialty Data
                    while (fromParentUnit != null)
                    {
                        var templateId = fromParentUnit.Unit.UnitTemplateId;
                        var type = fromRank.Type;

                        // Track Promotion
                        RankStatistics[templateId][type][fromRank.Grade].TrackPromotionToNextGrade(e, CurrentDate);
                        SpecialtyStatistics[templateId][type][fromRank.Grade][specId].TrackPromotionToNextGrade(e, CurrentDate);

                        // Add soldier to incoming on Next grade
                        RankStatistics[templateId][type][toRank.Grade].TrackPromotionIntoGrade(soldier);
                        SpecialtyStatistics[templateId][type][toRank.Grade][specId].TrackPromotionIntoGrade(soldier);

                        // Move up
                        fromParentUnit = fromParentUnit.Parent;
                    }

                    // In Position
                    var pos = PositionStatistics[e.ToPosition.Position.Id];
                    pos.TotalSoldiersIncoming += 1;
                    pos.TotalSoldiersPromotedIn += 1;
                    pos.TotalMonthsInGradeIncoming += 0;
                    pos.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    var bill = BilletStatistics[e.ToPosition.Position.BilletId];
                    bill.TotalSoldiersIncoming += 1;
                    bill.TotalSoldiersPromotedIn += 1;
                    bill.TotalMonthsInGradeIncoming += 0;
                    bill.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    // Out Position
                    pos = PositionStatistics[e.FromPosition.Position.Id];
                    pos.TotalSoldiersOutgoing += 1;
                    pos.TotalSoldiersPromotedOut += 1;
                    pos.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    bill = BilletStatistics[e.FromPosition.Position.BilletId];
                    bill.TotalSoldiersOutgoing += 1;
                    bill.TotalSoldiersPromotedOut += 1;
                    bill.TotalMonthsInPosition += e.FromPositionTimeInBillet;
                }
            }
        }

        private void SoldierWrapper_OnPositionChange(object sender, PositionChangeEventArgs e)
        {
            if (SkipYears == 0)
            {
                // Log promotion data for current rank/grade
                int tig = e.Soldier.GetTimeInGrade(CurrentIterationDate);
                int tis = e.Soldier.GetTimeInService(CurrentIterationDate);

                // Grab into position
                var pos = PositionStatistics[e.ToPosition.Position.Id];
                pos.TotalSoldiersIncoming += 1;
                pos.TotalMonthsInGradeIncoming += tig;
                pos.TotalMonthsInServiceIncoming += tis;

                var bill = BilletStatistics[e.ToPosition.Position.BilletId];
                bill.TotalSoldiersIncoming += 1;
                bill.TotalMonthsInGradeIncoming += tig;
                bill.TotalMonthsInServiceIncoming += tis;

                // Is this technically a promotion?
                if (e.FromPosition.Billet.Rank.Grade < e.ToPosition.Billet.Rank.Grade)
                {
                    pos.TotalSoldiersPromotedIn += 1;
                    bill.TotalSoldiersPromotedIn += 1;
                }
                else
                {
                    pos.TotalSoldiersLateralIn += 1;
                    bill.TotalSoldiersLateralIn += 1;
                }

                // New soldiers!
                if (e.FromPosition != null)
                {
                    // Out Of
                    pos = PositionStatistics[e.FromPosition.Position.Id];
                    pos.TotalSoldiersOutgoing += 1;
                    pos.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    bill = BilletStatistics[e.FromPosition.Position.BilletId];
                    bill.TotalSoldiersOutgoing += 1;
                    bill.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    // Is this technically a promotion?
                    if (e.FromPosition.Billet.Rank.Grade < e.ToPosition.Billet.Rank.Grade)
                    {
                        pos.TotalSoldiersPromotedOut += 1;
                        bill.TotalSoldiersPromotedOut += 1;
                    }
                    else
                    {
                        pos.TotalSoldiersLateralOut += 1;
                        bill.TotalSoldiersLateralOut += 1;
                    }
                }
            }
        }

        private void SoldierWrapper_OnRetire(object sender, SoldierWrapper soldier)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = soldier.Rank.Grade;
                var type = soldier.Rank.Type;
                int specId = soldier.Soldier.SpecialtyId;

                // Update position statistics
                var pos = PositionStatistics[soldier.Position.Position.Id];
                pos.TotalSoldiersOutgoing += 1;
                pos.TotalSoldiersRetireOut += 1;
                pos.TotalMonthsInPosition += soldier.GetTimeInBillet(CurrentIterationDate);

                var bill = BilletStatistics[soldier.Position.Position.BilletId];
                bill.TotalSoldiersOutgoing += 1;
                bill.TotalSoldiersRetireOut += 1;
                bill.TotalMonthsInPosition += soldier.GetTimeInBillet(CurrentIterationDate);

                // Log rank stats
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

        private void SoldierWrapper_OnRankGradeChange(object sender, RankChangeEventArgs e)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                // Grab ranks. Remember Foriegn keys are disabled
                Rank fromRank = RankCache.RanksById[e.Promotion.FromRankId];
                Rank toRank = RankCache.RanksById[e.Promotion.ToRankId];

                // Determine what happened here, and log accordinly
                bool isGradePromotion = (fromRank.Grade < toRank.Grade);

                // Grab soldier vars
                int specId = e.Soldier.Soldier.SpecialtyId;
                UnitWrapper parentUnit = e.Soldier.Position.ParentUnit;

                // Log
                if (isGradePromotion)
                {
                    while (parentUnit != null)
                    {
                        var templateId = parentUnit.Unit.UnitTemplateId;
                        int grade = fromRank.Grade;
                        var type = fromRank.Type;

                        // Track Promotion
                        RankStatistics[templateId][type][grade].TrackPromotionToNextGrade(e, CurrentDate);
                        SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionToNextGrade(e, CurrentDate);

                        // Add soldier to incoming on Next grade
                        RankStatistics[templateId][type][grade + 1].TrackPromotionIntoGrade(e.Soldier);
                        SpecialtyStatistics[templateId][type][grade + 1][specId].TrackPromotionIntoGrade(e.Soldier);

                        // Move up
                        parentUnit = parentUnit.Parent;
                    }
                }
                else // Is demotion
                {

                }
            }
        }

        #endregion Events and Logging

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                // Clear positions
                Positions?.Clear();
                Positions = null;

                // Clear soldiers
                ActiveDutySoldiers?.Clear();
                ActiveDutySoldiers = null;

                // Clear everything!
                RankStatistics?.Clear();
                RankStatistics = null;

                SpecialtyStatistics?.Clear();
                SpecialtyStatistics = null;

                BilletStatistics?.Clear();
                BilletStatistics = null;

                PositionStatistics?.Clear();
                PositionStatistics = null;

                BilletExperience?.Clear();
                BilletExperience = null;
            }
        }
    }
}