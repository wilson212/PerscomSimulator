using Perscom.Database;
using Perscom.Simulation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Perscom.Collections;

namespace Perscom
{
    /// <summary>
    /// Represents a simulator for running military simulations, managing soldiers,
    /// promotion boards, statistical data, and other simulation-related components.
    /// </summary>
    public class Simulator : IDisposable
    {
        /// <summary>
        /// Represents the current instance of the simulator being executed.
        /// </summary>
        public static Simulator Current { get; private set; }
        
        /// <summary>
        /// Indicates whether this instance is disposed
        /// </summary>
        private bool IsDisposed { get; set; }
        
        /// <summary>
        /// Contains the current Simulation Database instance
        /// </summary>
        private SimDatabase Database { get; set; }

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
        public IdentityList<int, SoldierWrapper> ActiveDutySoldiers { get; protected set; }

        /// <summary>
        /// All promotion board wrappers, keyed by PromotionBoard.Id for iteration.
        /// </summary>
        public KeyedList<int, PromotionBoardWrapper> PromotionBoards { get; private set; }

        /// <summary>
        /// Rank-specific board lookup: (RankId, OccupationId?) => Board
        /// Only populated for boards where RankId is NOT null.
        /// </summary>
        private Dictionary<(int, int?), PromotionBoardWrapper> BoardsByRank { get; set; }

        /// <summary>
        /// Classification-level (pay-grade) board lookup: (RankClassificationId, OccupationId?) => Board
        /// Only populated for boards where RankClassificationId is NOT null.
        /// </summary>
        private Dictionary<(int, int?), PromotionBoardWrapper> BoardsByClassification { get; set; }

        /// <summary>
        /// Gets a list of all the <see cref="Position"/> entities
        /// </summary>
        private List<PositionWrapper> Positions { get; set; }

        /// <summary>
        /// Manages vacant positions within the simulation,
        /// providing functionality to track, mark, and process vacancies
        /// at various organizational echelons.
        /// </summary>
        /// <remarks>
        /// FactionID => PositionManager
        /// </remarks>
        private KeyedList<int, PositionManager> PositionManagers { get; set; }

        /// <summary>
        /// Represents a thread-safe queue used to manage soldiers
        /// who are in the process of retiring from active duty.
        /// </summary>
        private ConcurrentQueue<SoldierWrapper> RetiringSoldiers { get; set; }

        /// <summary>
        /// UnitBlueprintId => [RankType => [Rank.PayGrade => RankGradeStatistics]]
        /// </summary>
        private Dictionary<int, Dictionary<RankType, Dictionary<int, RankGradeStatistics>>> RankStatistics
        {
            get;
            set;
        }

        /// <summary>
        /// UnitBlueprintId => [RankType => [Rank.PayGrade => [OccupationId => SpecialtyGradeStatistics]]]
        /// </summary>
        private Dictionary<int, Dictionary<RankType, Dictionary<int, Dictionary<int, SpecialtyGradeStatistics>>>> SpecialtyStatistics
        {
            get;
            set;
        }

        /// <summary>
        /// [BilletId => BilletStatistics]
        /// </summary>
        private Dictionary<int, PositionBlueprintStats> PosBlueprintStatsMap
        {
            get;
            set;
        }

        /// <summary>
        /// [PositionId => PositionStatistics]
        /// </summary>
        private Dictionary<int, PositionStatistics> PositionStatistics
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the root <see cref="Unit"/> that is processing in this Simulator instance
        /// </summary>
        private UnitWrapper ProcessingUnit { get; set; }

        /// <summary>
        /// The number of years to skip in the simulation before logging
        /// statistical data
        /// </summary>
        private int SkipYears { get; set; }

        /// <summary>
        /// The total number of years the simulation was ran
        /// </summary>
        protected int TotalYearsRan { get; set; } = 0;

        /// <summary>
        /// The total number of Iterations the simulation ran
        /// </summary>
        private int StopIteration { get; set; } = 1;

        /// <summary>
        /// Creates a new Simulator instance
        /// </summary>
        /// <param name="db"></param>
        /// <param name="unit">The unit to run the simulation on</param>
        /// <param name="settings"></param>
        public Simulator(SimDatabase db, UnitWrapper unit, SimulatorSettings settings)
        {
            // Set instance properties
            Database = db;
            ProcessingUnit = unit;
            Settings = settings;
            
            // Fill Position Manager Dictionary
            PositionManagers = new();
            RetiringSoldiers = new();
            
            // Grab managers
            foreach (var faction in Database.Factions)
            {
                PositionManagers[faction.Id] = new PositionManager(Database.Echelons.Count - 1);
            }
            
            // ToDo: Need to calculate the total number of Positions
            var stats = UnitBuilder.GetUnitStatistics(unit.Unit.Blueprint);
            ActiveDutySoldiers = new IdentityList<int, SoldierWrapper>(stats.TotalSoldiers);
            
            // Attach Events
            SoldierWrapper.OnLateralPositionExchange += SoldierWrapper_OnLateralPositionExchange;
            SoldierWrapper.OnPositionChange += SoldierWrapper_OnPositionChange;
            SoldierWrapper.OnPositionAndRankChange += SoldierWrapper_OnPositionAndRankChange;
            SoldierWrapper.OnRankGradeChange += SoldierWrapper_OnRankGradeChange;
            SoldierWrapper.OnRetire += SoldierWrapper_OnRetire;
            
            // Set this instance as current
            Current = this;
        }

        /// <summary>
        /// Sets up the simulation
        /// </summary>
        /// <param name="totalYears">How many years to run the simulation for.</param>
        /// <param name="skipYears">
        /// How many years to skip before logging statistical data. This is done to
        /// alleviate bad data at the start of a simulation due to low soldier movement.
        /// </param>
        private void Setup(int totalYears, int skipYears)
        {
            // Check data
            if (totalYears < skipYears)
                throw new ArgumentException(@"totalYears cannot be less than skipYears", "totalYears");

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
                var date = Database.IterationDates.LastOrDefault();
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

                // Get all positions
                var stats = UnitBuilder.GetUnitStatistics(ProcessingUnit.Unit.Blueprint);
                Positions = new List<PositionWrapper>(stats.PositionCount);
                ProcessingUnit.GetAllPositions(Positions);

                // Populate soldiers if we need to
                using (var trans = Database.BeginTransaction())
                {
                    PopulateSoldiers();
                    trans.Commit();
                }
                
                // Initialize promotion boards
                InitializePromotionBoards();
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
        }

        /// <summary>
        /// Executes the simulation for a specified number of years, optionally skipping an initial set of years.
        /// </summary>
        /// <param name="totalYears">The total number of years to run the simulation.</param>
        /// <param name="skipYears">The number of years at the beginning of the simulation to skip for data collection purposes.</param>
        public void RunSimulation(int totalYears, int skipYears, IProgress<TaskProgressUpdate> progress, CancellationToken token)
        {
            // Wrap in a transaction block
            using var trans = Database.BeginTransaction();
            
            // Setup dates, populate initial soldiers...
            Setup(totalYears, skipYears);

            for (int month = 0; month < StopIteration; month++)
            {
                token.ThrowIfCancellationRequested();

                // Create or advance IterationDate
                var date = new IterationDate { Date = StartDate.AddMonths(month) };
                Database.IterationDates.Add(date);
                CurrentIterationDate = date;

                // Run one month
                ProcessMonth(date);

                // Commit DB changes periodically
                if (month >= skipYears * 12)
                {
                    // Log statistics
                }

                progress?.Report(new TaskProgressUpdate
                {
                    ProgressPercent = (int)((month / (double)StopIteration) * 100)
                });
            }

            trans.Commit();
        }

        /// <summary>
        /// Executes the simulation logic for the specified iteration date.
        /// </summary>
        /// <param name="date">The date representing the current iteration of the simulation.</param>
        private void ProcessMonth(IterationDate date)
        {
            // ---------------------------------------------
            // Mutli-threaded processing
            
            // Create a loop state object that will be passed to each loop iteration. This prevents closures
            var loopState = (
                Date: date, 
                DB: Database, 
                Queue: RetiringSoldiers, 
                Positions:  PositionManagers,
                Sim: this
            );
            
            // PHASE ONE: Process Promotion Boards, updating the active promotion lists
            PromotionBoards.ParallelEach(loopState, (board, state) => board.ConveneBoard(state.Date, state.Sim));
            
            // PHASE TWO: The Global Soldier update.
            // Pack everything the loop needs into a single Tuple to avoid closure allocations
            // from being created and hopefully prevent GC Stuttering.
            ActiveDutySoldiers.ParallelEach(loopState, (soldier, state) => 
            {
                // Update Morale, Burnout, Time-in-Grade, Promotion applications, etc.
                soldier.ProcessMonthlyUpdate(state.Date, state.DB);

                // Did this math cause them to quit or retire?
                if (soldier.IsRetiring(state.Date))
                {
                    // Grab position
                    var pos = soldier.Position;
                    
                    // Safely throw their now-empty position into the Cascade buckets
                    state.Positions[pos.ParentUnit.FactionId].MarkPositionVacant(pos);
                    
                    // Enqueue the soldier for retirement processing in a single thread
                    state.Queue.Enqueue(soldier);
                }
                else
                {
                    // Attempt promotion board if eligible
                    soldier.AttemptPromotionBoard(state.Date, state.DB);
                }
            });
            
            // ---------------------------------------------
            // -- Switch to single-threaded processing
            
            // PHASE THREE: Process retirements. This is not thread-safe
            while (RetiringSoldiers.TryDequeue(out var soldier))
            {
                // Process the retirement...
                soldier.Retire(date, Database);
                
                // Remove from tracking
                ActiveDutySoldiers.Remove(soldier.Entity.Id);
            }
            
            // PHASE FOUR: Process empty Positions. This is not thread-safe
            foreach (var manager in PositionManagers)
            {
                // When a soldier retires, we need to process vacancies to fill them back up
                manager.ProcessAllVacancies(date);
            };
            
            // PHASE FIVE: Close Promotion Boards. This is a locking operation on each Insert/Update.
            // Therefore, it's not exactly built for Parallel.ForEach
            foreach (var board in PromotionBoards)
            {
                board.SaveChanges(Database, this);
            }
        }

        /// <summary>
        /// Initially populates the soldiers at the start of a simulation
        /// </summary>
        private void PopulateSoldiers()
        {
            foreach (var position in Positions)
            {
                if (!position.IsEmpty) continue;

                // Pick a random Persona blueprint
                var persona = SimulationCache.GetRandomPersona();
                var rank = position.BlueprintWrapper.Rank;
                var occupation = position.BlueprintWrapper.Occupation;

                var soldier = SoldierWrapper.Spawn(persona, rank, CurrentIterationDate, occupation, Database);
                position.AssignSoldier(soldier);
                ActiveDutySoldiers.Add(soldier);
                position.ParentUnit.AddSoldier(soldier);
            }
        }
        
        /// <summary>
        /// Loads promotion boards from the database and assigns them to the appropriate units
        /// </summary>
        private void InitializePromotionBoards()
        {
            var allBoards = Database.PromotionBoards.ToList();
            PromotionBoards = new KeyedList<int, PromotionBoardWrapper>(allBoards.Count);
            BoardsByRank = new Dictionary<(int, int?), PromotionBoardWrapper>(allBoards.Count);
            BoardsByClassification = new Dictionary<(int, int?), PromotionBoardWrapper>(allBoards.Count);

            foreach (var board in allBoards)
            {
                var wrapper = new PromotionBoardWrapper(board);
                PromotionBoards.Add(board.Id, wrapper);

                // Index into the rank-specific lookup
                if (board.RankId != null)
                {
                    BoardsByRank.TryAdd((board.RankId.Value, board.OccupationId), wrapper);
                }

                // Index into the classification-level lookup
                if (board.RankClassificationId != null)
                {
                    BoardsByClassification.TryAdd((board.RankClassificationId.Value, board.OccupationId), wrapper);
                }
            }

            // Mid-sim load: hydrate existing candidates into lazy pools
            var existingCandidates = Database.PromotableCandidates.ToList();
            if (existingCandidates.Count > 0)
            {
                foreach (var candidate in existingCandidates)
                {
                    if (ActiveDutySoldiers.TryGetValue(candidate.SoldierId, out var soldier))
                    {
                        var board = FindPromotionBoard(soldier.Entity.TargetRankId.Value, soldier.Occupation?.Id);
                        if (board != null)
                        {
                            soldier.Position?.ParentUnit?.RegisterPromotableUpChain(
                                candidate, soldier.Entity.TargetRankId.Value, soldier.Occupation?.Id, board);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Resolves the correct PromotionBoardWrapper for a given target rank and optional occupation,
        /// using the fallback chain: RankId → RankClassificationId, occupation-specific → generic.
        /// </summary>
        public PromotionBoardWrapper FindPromotionBoard(int targetRankId, int? occupationId)
        {
            // Try rank-specific + occupation
            if (occupationId != null && BoardsByRank.TryGetValue((targetRankId, occupationId), out var board))
                return board;

            // Try rank-specific + generic
            if (BoardsByRank.TryGetValue((targetRankId, null), out board))
                return board;

            // Fall back to classification-level
            var rank = RankCache.RanksById[targetRankId];
            int classId = rank.RankClassificationId;

            // Try classification + occupation
            if (occupationId != null && BoardsByClassification.TryGetValue((classId, occupationId), out board))
                return board;

            // Try classification + generic
            if (BoardsByClassification.TryGetValue((classId, null), out board))
                return board;

            return null;
        }

        #region Events and Logging

        private void LogSoldierEntry(SoldierWrapper soldier)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = soldier.Rank.PayGrade;
                var type = soldier.Rank.Type;
                int specId = soldier.Entity.OccupationId;
                UnitWrapper parentUnit = soldier.Position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitBlueprintId;

                    RankStatistics[templateId][type][grade].TrackPromotionIntoGrade(soldier);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionIntoGrade(soldier);

                    // MoveTo up
                    parentUnit = parentUnit.Parent;
                }

                // Update position statistics
                var pos = PositionStatistics[soldier.Position.Position.Id];
                pos.TotalSoldiersIncoming += 1;

                var bill = PosBlueprintStatsMap[soldier.Position.Position.BlueprintId];
                bill.TotalSoldiersIncoming += 1;
            }
        }

        private void LogDeficit(PositionWrapper position)
        {
            // Log promotion data for current rank/grade
            if (SkipYears == 0)
            {
                int grade = position.BlueprintWrapper.Rank.PayGrade;
                var type = position.BlueprintWrapper.Rank.Type;
                int specId = position.BlueprintWrapper.Occupation?.Id ?? -1;
                UnitWrapper parentUnit = position.ParentUnit;

                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitBlueprintId;
                    RankStatistics[templateId][type][grade].Deficit += 1;

                    if (specId >= 0)
                        SpecialtyStatistics[templateId][type][grade][specId].Deficit += 1;

                    // MoveTo up
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
                    PosBlueprintStatsMap[position.Position.BlueprintId].EmptyDeficit += 1;
                }
                else if (position.Holder.IsStandIn())
                {
                    PositionStatistics[position.Position.Id].StandInDeficit += 1;
                    PosBlueprintStatsMap[position.Position.BlueprintId].StandInDeficit += 1;
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
                bool isLateral = (fromRank.PayGrade == toRank.PayGrade);
                bool isGradePromotion = (fromRank.PayGrade < toRank.PayGrade);

                // Grab soldier vars
                var soldier = e.Soldier;
                int specId = e.FromSpecialty.Id;
                UnitWrapper fromParentUnit = e.FromPosition.ParentUnit;
                UnitWrapper toParentUnit = e.ToPosition.ParentUnit;

                // Log
                if (isRankTypeChange)
                {
                    int fromGrade = fromRank.PayGrade;
                    var fromType = fromRank.Type;
                    int toGrade = toRank.PayGrade;
                    var toType = toRank.Type;
                    int toSpecId = e.ToPosition.BlueprintWrapper.Occupation.Id;

                    // Transfer out statistics
                    while (fromParentUnit != null)
                    {
                        var templateId = fromParentUnit.Unit.UnitBlueprintId;

                        // Outgoing
                        RankStatistics[templateId][fromType][fromGrade].TrackRankTransferFrom(e, CurrentIterationDate);
                        SpecialtyStatistics[templateId][fromType][fromGrade][specId].TrackRankTransferFrom(e, CurrentIterationDate);

                        // MoveTo up
                        fromParentUnit = fromParentUnit.Parent;
                    }

                    // Transfer into statistics
                    while (toParentUnit != null)
                    {
                        var templateId = toParentUnit.Unit.UnitBlueprintId;

                        // Incoming
                        RankStatistics[templateId][toType][toGrade].TrackRankTransferInto(soldier);
                        SpecialtyStatistics[templateId][toType][toGrade][toSpecId].TrackRankTransferInto(soldier);

                        // MoveTo up
                        toParentUnit = toParentUnit.Parent;
                    }

                    // In Position
                    var pos = PositionStatistics[e.ToPosition.Position.Id];
                    pos.TotalSoldiersIncoming += 1;
                    pos.TotalSoldiersTransferredIn += 1;
                    pos.TotalMonthsInGradeIncoming += 0;
                    pos.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    var bill = PosBlueprintStatsMap[e.ToPosition.Position.BlueprintId];
                    bill.TotalSoldiersIncoming += 1;
                    bill.TotalSoldiersTransferredIn += 1;
                    bill.TotalMonthsInGradeIncoming += 0;
                    bill.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    // Out Position
                    pos = PositionStatistics[e.FromPosition.Position.Id];
                    pos.TotalSoldiersOutgoing += 1;
                    pos.TotalSoldiersTransferredOut += 1;
                    pos.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    bill = PosBlueprintStatsMap[e.FromPosition.Position.BlueprintId];
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
                    // Log grade promotion for Rank and Occupation Data
                    while (fromParentUnit != null)
                    {
                        var templateId = fromParentUnit.Unit.UnitBlueprintId;
                        var type = fromRank.Type;

                        // Track Promotion
                        RankStatistics[templateId][type][fromRank.PayGrade].TrackPromotionToNextGrade(e, CurrentDate);
                        SpecialtyStatistics[templateId][type][fromRank.PayGrade][specId].TrackPromotionToNextGrade(e, CurrentDate);

                        // Add soldier to incoming on Next grade
                        RankStatistics[templateId][type][toRank.PayGrade].TrackPromotionIntoGrade(soldier);
                        SpecialtyStatistics[templateId][type][toRank.PayGrade][specId].TrackPromotionIntoGrade(soldier);

                        // MoveTo up
                        fromParentUnit = fromParentUnit.Parent;
                    }

                    // In Position
                    var pos = PositionStatistics[e.ToPosition.Position.Id];
                    pos.TotalSoldiersIncoming += 1;
                    pos.TotalSoldiersPromotedIn += 1;
                    pos.TotalMonthsInGradeIncoming += 0;
                    pos.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    var bill = PosBlueprintStatsMap[e.ToPosition.Position.BlueprintId];
                    bill.TotalSoldiersIncoming += 1;
                    bill.TotalSoldiersPromotedIn += 1;
                    bill.TotalMonthsInGradeIncoming += 0;
                    bill.TotalMonthsInServiceIncoming += soldier.GetTimeInService(CurrentIterationDate);

                    // Out Position
                    pos = PositionStatistics[e.FromPosition.Position.Id];
                    pos.TotalSoldiersOutgoing += 1;
                    pos.TotalSoldiersPromotedOut += 1;
                    pos.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    bill = PosBlueprintStatsMap[e.FromPosition.Position.BlueprintId];
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

                var bill = PosBlueprintStatsMap[e.ToPosition.Position.BlueprintId];
                bill.TotalSoldiersIncoming += 1;
                bill.TotalMonthsInGradeIncoming += tig;
                bill.TotalMonthsInServiceIncoming += tis;

                // Is this technically a promotion?
                if (e.FromPosition.BlueprintWrapper.Rank.PayGrade < e.ToPosition.BlueprintWrapper.Rank.PayGrade)
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

                    bill = PosBlueprintStatsMap[e.FromPosition.Position.BlueprintId];
                    bill.TotalSoldiersOutgoing += 1;
                    bill.TotalMonthsInPosition += e.FromPositionTimeInBillet;

                    // Is this technically a promotion?
                    if (e.FromPosition.BlueprintWrapper.Rank.PayGrade < e.ToPosition.BlueprintWrapper.Rank.PayGrade)
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
                int grade = soldier.Rank.PayGrade;
                var type = soldier.Rank.Type;
                int specId = soldier.Entity.OccupationId;

                // Update position statistics
                var pos = PositionStatistics[soldier.Position.Position.Id];
                pos.TotalSoldiersOutgoing += 1;
                pos.TotalSoldiersRetireOut += 1;
                pos.TotalMonthsInPosition += soldier.GetTimeInBillet(CurrentIterationDate);

                var bill = PosBlueprintStatsMap[soldier.Position.Position.BlueprintId];
                bill.TotalSoldiersOutgoing += 1;
                bill.TotalSoldiersRetireOut += 1;
                bill.TotalMonthsInPosition += soldier.GetTimeInBillet(CurrentIterationDate);

                // Log rank stats
                UnitWrapper parentUnit = soldier.Position.ParentUnit;
                while (parentUnit != null)
                {
                    var templateId = parentUnit.Unit.UnitBlueprintId;
                    RankStatistics[templateId][type][grade].TrackRetiree(soldier, CurrentIterationDate);
                    SpecialtyStatistics[templateId][type][grade][specId].TrackRetiree(soldier, CurrentIterationDate);

                    // MoveTo up
                    parentUnit = parentUnit.Parent;
                }
            }
            
            soldier.Position?.ParentUnit?.DeregisterPromotableUpChain(soldier);
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
                bool isGradePromotion = (fromRank.PayGrade < toRank.PayGrade);

                // Grab soldier vars
                int specId = e.Soldier.Entity.OccupationId;
                UnitWrapper parentUnit = e.Soldier.Position.ParentUnit;

                // Log
                if (isGradePromotion)
                {
                    while (parentUnit != null)
                    {
                        var templateId = parentUnit.Unit.UnitBlueprintId;
                        int grade = fromRank.PayGrade;
                        var type = fromRank.Type;

                        // Track Promotion
                        RankStatistics[templateId][type][grade].TrackPromotionToNextGrade(e, CurrentDate);
                        SpecialtyStatistics[templateId][type][grade][specId].TrackPromotionToNextGrade(e, CurrentDate);

                        // Add soldier to incoming on Next grade
                        RankStatistics[templateId][type][grade + 1].TrackPromotionIntoGrade(e.Soldier);
                        SpecialtyStatistics[templateId][type][grade + 1][specId].TrackPromotionIntoGrade(e.Soldier);

                        // MoveTo up
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

                PosBlueprintStatsMap?.Clear();
                PosBlueprintStatsMap = null;

                PositionStatistics?.Clear();
                PositionStatistics = null;
            }
        }
    }
}