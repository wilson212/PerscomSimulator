using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        protected Dictionary<RankType, SpawnGenerator<CareerSpawnRate>> CareerGenerators { get; set; }

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

            // Load soldier career probabilities
            var list = db.CareerSpawnRates.ToArray();
            CareerGenerators = new Dictionary<RankType, SpawnGenerator<CareerSpawnRate>>();
            foreach (RankType type in RankTypes)
            {
                // Create Spawn Generator
                var generator = new SpawnGenerator<CareerSpawnRate>();
                generator.AddRange(list.Where(x => x.Type == type));

                // Create dictionary record
                CareerGenerators.Add(type, generator);
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
            try
            {
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
            catch(Exception ex)
            {
                ExceptionHandler.GenerateExceptionLog(ex);
                throw;
            }
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
                    if (position.Billet.IsEntryLevel)
                    {
                        soldier = CreateSoldier(SoldierGenerators[position.Billet.SpawnSetting.GeneratorId], position);
                        soldier.AssignPosition(position, CurrentIterationDate, Database);
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

                // If we could not find someone to fill the position
                if (soldier == null)
                    continue;

                // Define soldier vars
                PromotableStatus status;
                int grade = soldier.Rank.Grade;
                int specId = soldier.Soldier.SpecialtyId;

                // Check for retirement
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

                // Check if soldier hit MaxTourLength
            }
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
            int grade = position.Billet.Rank.Grade;

            // Keep searching until we either find a soldier to
            // fill the slot, or run out of rank/grades to pull from.
            while (grade > 0)
            {
                //
                // === Apply Filters === //
                //
                // TODO: Apply filters to force lateral promotions
                //
                var soldiers = topUnit.SoldiersByGrade[type][grade].Values
                    .Where(x => IsCandidateFor(x, position))
                    .OrderBy(x => x.LastPromotionDate.Id);

                // If we have no soldiers, downgrade
                if (soldiers.FirstOrDefault() == null)
                {
                    --grade;
                    continue;
                }

                // Check for repeatable preference
                if (position.Billet.Billet.PreferNonRepeats)
                {
                    soldiers = soldiers.ThenBy(x => x.BilletsHeld.ContainsKey(position.Billet.Billet.Id) ? 1 : 0);
                }

                // Fetch soldier list without checking LockedIn
                var primeSoldiers = soldiers.Where(x => !x.Position.IsLockedIn(CurrentIterationDate));

                // Check for an un-restricted soldier first
                var wrapper = primeSoldiers.FirstOrDefault();
                if (wrapper != null)
                {
                    return wrapper;
                }

                wrapper = soldiers.FirstOrDefault();
                if (wrapper != null && grade < position.Billet.Rank.Grade)
                {
                    // Never laterally move a locked in soldier from the same 
                    // Rank grade as the positon calls for, otherwise we could 
                    // have a game of musical chairs happening EVERY month.
                    return wrapper;
                }

                // Lower grade, and try again
                --grade;
            }

            return null;
        }

        /// <summary>
        /// This method determines if the specified soldier is a good candidate
        /// for the specified position.
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsCandidateFor(SoldierWrapper soldier, PositionWrapper position)
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
                if (!position.Billet.RequiredSpecialties.Contains(soldier.Soldier.SpecialtyId))
                    return false;
            }

            // Check for repeatable position
            if (!position.Billet.Billet.Repeatable)
            {
                if (soldier.BilletsHeld.ContainsKey(position.Billet.Id))
                    return false;
            }

            // If this is a lateral move, only select it IF the stature is higher
            if (soldier.Position.Billet.Rank.Grade == position.Billet.Rank.Grade)
            {
                return (soldier.Position.Billet.Stature < position.Billet.Stature);
            }

            // If this is a promotion, then hell yes!
            else if (position.Billet.Rank.Grade > soldier.Rank.Grade)
            {
                return true;
            }

            // Do not promote downwards on stature
            else if (soldier.Position.Billet.Rank.Grade > position.Billet.Rank.Grade)
            {
                return false;
            }

            // if we are here, then fuck it, why not?
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

            // Build initial roster
            foreach (var pos in Positions)
            {
                // Only fill entry positions!
                if (pos.Billet.IsEntryLevel && Settings.ProcessRankType(pos.Billet.Rank.Type))
                {
                    var soldier = CreateSoldier(SoldierGenerators.Values.First(), pos);
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
            SoldierGeneratorSetting setting = generator.Spawn();
            bool newCareer = setting.NewCareerLength;
            SoldierWrapper wrapper = null;

            // If rankId is 0, then we create new
            if (setting.RankId == 0)
            {
                Soldier soldier = new Soldier();
                soldier.FirstName = NameGenerator.GenerateRandomFirstName();
                soldier.LastName = NameGenerator.GenerateRandomLastName();
                soldier.EntryIterationId = CurrentIterationDate.Id;
                soldier.LastPromotionIterationId = CurrentIterationDate.Id;
                soldier.RankId = position.Billet.Rank.Id;
                soldier.SpecialtyId = position.Billet.Specialty.Id;

                // Assign the soldiers new career length
                var career = CareerGenerators[position.Billet.Rank.Type].Spawn();
                soldier.SpawnRateId = career.Id;

                // Here we figure out just how many months the soldier will stay in service.
                CryptoRandom r = new CryptoRandom();
                int toAdd = r.Next(career.MinCareerLength, career.MaxCareerLength);
                soldier.ExitIterationId = CurrentIterationDate.Id + toAdd;

                // Add soldier to database
                Database.Soldiers.Add(soldier);

                // Create soldier wrapper
                wrapper = new SoldierWrapper(soldier, position.Billet.Rank, CurrentIterationDate, Database);
                ActiveDutySoldiers.Add(soldier.Id, wrapper);

                newCareer = false;
            }
            else
            {
                // Fetch promotion pool
                UnitWrapper topUnit = position.PromotionPoolUnit;
                RankType type = setting.Rank?.Type ?? position.Billet.Rank.Type;
                int grade = setting.Rank?.Grade ?? position.Billet.Rank.Grade;

                var soldiers = topUnit.SoldiersByGrade[type][grade];
                foreach (var s in soldiers)
                {
                    int id = s.Key;
                    wrapper = s.Value;

                    // Only if the soldier has reached their tour requirements
                    if (!wrapper.Position.IsLockedIn(CurrentIterationDate))
                    {
                        Soldier soldier = wrapper.Soldier;
                        wrapper.PromoteTo(CurrentIterationDate, position.Billet.Rank, Database);

                        // Quit looping
                        break;
                    }
                }
            }

            // Does this soldier get assigned a new career?
            if (newCareer)
            {
                Soldier soldier = wrapper.Soldier;

                // Assign the soldiers new career length
                var career = CareerGenerators[wrapper.Rank.Type].Spawn();
                soldier.SpawnRateId = career.Id;

                // Here we figure out just how many months the soldier will stay in service.
                CryptoRandom r = new CryptoRandom();
                int toAdd = r.Next(career.MinCareerLength, career.MaxCareerLength);
                soldier.ExitIterationId = CurrentIterationDate.Id + toAdd;
            }

            // Send him to duty!
            return wrapper;
        }
    }
}
