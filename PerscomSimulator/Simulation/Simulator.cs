using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Perscom.Simulation;

namespace Perscom
{
    public class Simulator
    {
        /// <summary>
        /// The starting date for a simulation
        /// </summary>
        public DateTime StartDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Gets the current Simulation date
        /// </summary>
        public DateTime CurrentDate { get; protected set; } = DateTime.Now;

        /// <summary>
        /// Gets the End Date for the simulation
        /// </summary>
        public DateTime EndDate { get; protected set; }

        /// <summary>
        /// The total number of soldier objects created by this simulation, used
        /// for setting the SpawnID of a soldier
        /// </summary>
        protected int _soldiersSpawned = 0;

        /// <summary>
        /// Gets the <see cref="SimulatorSettings"/> for the last simulation
        /// </summary>
        public SimulatorSettings Settings { get; private set; }

        /// <summary>
        /// Grade => Soldier Array
        /// </summary>
        public Dictionary<RankType, Dictionary<int, List<Soldier>>> Soldiers { get; protected set; }

        /// <summary>
        /// Soldier Generator
        /// </summary>
        protected Dictionary<RankType, SpawnGenerator<Soldier>> Generator { get; set; }

        /// <summary>
        /// RankType => [Grade => PromotionInfo (for grade)]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, RankGradeStatistics>> RankStatistics { get; protected set; }

        /// <summary>
        /// The Unit that is processing in this Simulator instance
        /// </summary>
        public Unit ProcessingUnit { get; protected set; }

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
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        protected RandomNameGenerator NameGenerator { get; set; }

        /// <summary>
        /// Type => [Grade => Stack]
        /// </summary>
        protected Dictionary<RankType, Dictionary<int, Stack<UnitPosition>>> AvailablePositions { get; set; }

        /// <summary>
        /// Contains an enumerable array of RankTypes
        /// </summary>
        protected static RankType[] RankTypes = Enum.GetValues(typeof(RankType)).Cast<RankType>().ToArray();

        /// <summary>
        /// Creates a new Simulator instance
        /// </summary>
        /// <param name="unit">The unit to run the simulation on</param>
        public Simulator(Unit unit, SimulatorSettings settings)
        {
            ProcessingUnit = unit;
            Settings = settings;
            NameGenerator = new RandomNameGenerator();
            Generator = new Dictionary<RankType, SpawnGenerator<Soldier>>();
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
                Generator[type] = new SpawnGenerator<Soldier>();

            // Load probabilities from xml file
            LoadSoldierSettings();
        }

        /// <summary>
        /// Loads the Probabilities.xml file to establish the soldier probability values
        /// </summary>
        private void LoadSoldierSettings()
        {
            // Ensure the unit exists
            string filePath = Path.Combine(Program.RootPath, "Config", "Soldiers.xml");
            if (!File.Exists(filePath))
                throw new Exception($"Soldiers.xml file is missing!");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;
            
            foreach (RankType type in RankTypes)
            {
                string name = Enum.GetName(typeof(RankType), type).ToLower();
                XmlNodeList items = root.SelectNodes($"{name}/soldier");
                foreach (XmlElement element in items)
                {
                    int prob = Int32.Parse(element.Attributes["probability"].Value);
                    int min = Int32.Parse(element.Attributes["minTime"].Value);
                    int max = Int32.Parse(element.Attributes["maxTime"].Value);
                    Generator[type].Add(new Soldier(prob, new Range<int>(min, max)));
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
            int startMonth = DateTime.Now.Month;
            EndDate = new DateTime(DateTime.Now.Year, startMonth, 1);
            CurrentDate = StartDate = EndDate.AddYears(-totalYears);
            SkipYears = skipYears;
            TotalYearsRan = 0;

            // Populate soldiers if we need to
            PopulateSoldiers();

            // Update progress
            TaskProgressUpdate update = new TaskProgressUpdate();
            update.HeaderText = "Running Simulation... Please Wait.";
            progress.Report(update);

            // Main Loop
            while (EndDate != CurrentDate)
            {
                // Quit if cancelled
                if (token.IsCancellationRequested)
                    return;

                // Update the date
                CurrentDate = CurrentDate.AddMonths(1);

                // Update progress window every 1 year of simulation
                int yearsDone = CurrentDate.Year - StartDate.Year;
                if (CurrentDate.Month == startMonth && yearsDone > 0)
                {
                    TotalYearsRan = yearsDone;
                    update = new TaskProgressUpdate();
                    update.MessageText = $"Processing year {yearsDone} of {totalYears}";
                    progress.Report(update);
                }

                // Process Retirements first!
                PerformStartOfMonthDuties();

                // Now do promotions
                ProcessPromotions();

                // If an entire years has gone by, subtract a skip year
                // so we can begin logging again when the time comes.
                if (SkipYears > 0 && CurrentDate.Month == startMonth)
                {
                    SkipYears -= 1;
                }
            }
        }

        /// <summary>
        /// Returns the total percentage of soldiers who made it to the specified
        /// rank and grade in the simulation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public double TotalPromotionRate(RankType type, int grade)
        {
            // Make sure the rank exists in the simulation
            if (_soldiersSpawned == 0 || !RankStatistics[type].ContainsKey(grade))
                return 0;

            // Total number of soldiers who did NOT make this rank/grade
            int totalPriorGradeRetirements = 0;

            // Total number of soldiers who DID make this rank/grade
            int totalAtThisRank = RankStatistics[type][grade].TotalSoldiers;

            // Add up the total number of soldiers who did NOT make this
            // grade in the specified rank type
            foreach (Rank r in Ranks.GetPrevousGrades(type, grade))
            {
                // If no soldiers ever made this rank/grade, skip
                if (!RankStatistics[type].ContainsKey(r.Grade))
                    continue;

                // Add total retirements at this grade to the total cumulative retirements
                var info = RankStatistics[type][r.Grade];
                totalPriorGradeRetirements += info.TotalRetirements;
            }

            // Get the total number of soldiers up to this rank and grade point
            double totalSoldiers = totalAtThisRank + totalPriorGradeRetirements;

            // Prevent div by zero exception
            return (totalSoldiers == 0) ? 0 : Math.Round((totalAtThisRank / totalSoldiers) * 100, 2);
        }

        /// <summary>
        /// Gets the percentage of positions at the specified rank / grade that
        /// are understaffed on average
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public double GetAverageDeficitRate(RankType type, int grade)
        {
            UnitTemplate template = UnitTemplate.Load(ProcessingUnit.TemplateName);
            int positions = template.SoldierCounts[type][grade];
            int cumulative = positions * (TotalYearsRan * 12);

            // Total number of soldiers who DID make this rank/grade
            int totalDeficit = RankStatistics[type][grade].Deficit;

            if (cumulative == 0 || totalDeficit == 0) return 0;

            return Math.Round(((double)totalDeficit / cumulative) * 100, 2);
        }

        /// <summary>
        /// Each soldier is given a TimeToLive when created, this method will clear them
        /// out to make room for new soldiers and also process automatic promotions
        /// </summary>
        /// <param name="soldiers"></param>
        private void PerformStartOfMonthDuties()
        {
            foreach (RankType type in RankTypes)
            {
                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                // Itterate through each Grade, except the last
                foreach (KeyValuePair<int, List<Soldier>> data in Soldiers[type])
                {
                    // Create a shallow copy of the soldiers at this grade, so we
                    // can enumerate and still modify the soldier list
                    Soldier[] soldiers = new Soldier[Soldiers[type][data.Key].Count];
                    Soldiers[type][data.Key].CopyTo(soldiers);

                    // Now we promote the selected soldiers for promotion
                    foreach (Soldier soldier in soldiers)
                    {
                        // Check for retirement
                        if (soldier.IsRetiring(CurrentDate))
                        {
                            // Log retirement data for current rank/grade
                            if (SkipYears == 0)
                                RankStatistics[type][soldier.RankInfo.Grade].TrackRetiree(soldier, CurrentDate);

                            // Remove the retired soldier from the roster
                            Soldiers[type][soldier.RankInfo.Grade].Remove(soldier);

                            // Forfeit the soldiers position
                            AvailablePositions[type][soldier.Position.Grade].Push(soldier.Position);

                            // Say goodbye!
                            soldier.Retire();
                            continue;
                        }

                        // Check for rank deficit before processing auto promotions
                        if (soldier.Position.Grade > soldier.RankInfo.Grade)
                        {
                            // Log retirement data for current rank/grade
                            if (SkipYears == 0)
                                RankStatistics[type][soldier.Position.Grade].Deficit += 1;
                        }

                        // Check for auto promotion and under-staff promotion
                        PromotableStatus status;
                        if (soldier.IsPromotable(CurrentDate, out status) && (status == PromotableStatus.Automatic || status == PromotableStatus.Position))
                        {
                            // Log promotion data for current rank/grade
                            if (SkipYears == 0)
                                RankStatistics[type][soldier.RankInfo.Grade].TrackPromotionToNextGrade(soldier, CurrentDate);

                            // Move soldier to the new rank roster
                            Rank toRank = Ranks.GetRank(type, soldier.RankInfo.Grade + 1);
                            Soldiers[type][soldier.RankInfo.Grade].Remove(soldier);
                            Soldiers[type][toRank.Grade].Add(soldier);

                            // Promotion soldier
                            soldier.Promote(CurrentDate, toRank);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is called after <see cref="PerformStartOfMonthDuties()"/> is 
        /// called, so we can promote soldiers to fill those new empty slots.
        /// </summary>
        private void ProcessPromotions()
        {
            foreach (RankType type in RankTypes)
            {
                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                // Get the grade of the entry level rank
                Rank entryRank = Ranks.GetEntryLevelRank(type);
                Queue<UnitPosition> positions = new Queue<UnitPosition>();

                // Itterate through each Grade in Reverse
                foreach (KeyValuePair<int, List<Soldier>> data in Soldiers[type].OrderByDescending(x => x.Key))
                {
                    // Grab number of open positions for this grade
                    Rank toRank = Ranks.GetRank(type, data.Key);
                    
                    // Add positions from this grade to the Queue
                    while (AvailablePositions[type][toRank.Grade].Count > 0)
                    {
                        UnitPosition pos = AvailablePositions[type][toRank.Grade].Pop();
                        positions.Enqueue(pos);
                    }

                    // If this is an entry level rank, we handle things differently
                    if (!Ranks.GradeExists(type, data.Key - 1))
                    {
                        // throw error if this is not an entry level rank!
                        if (toRank.Grade != entryRank.Grade)
                        {
                            string name = Enum.GetName(typeof(RankType), type);
                            throw new Exception(String.Format("There seems to be a Grade gap in the RankType {0}", name));
                        }

                        // Create new soldiers!
                        while (positions.Count > 0)
                        {
                            Soldier soldier = CreateSoldier(toRank.Grade, type);
                            soldier.AssignPosition(positions.Dequeue(), CurrentDate);
                            Soldiers[type][toRank.Grade].Add(soldier);
                        }
                    }
                    else if (positions.Count > 0)
                    {
                        // Grab previous grade
                        Rank fromRank = Ranks.GetPreviousGrade(type, toRank.Grade);
                        int prevGrade = fromRank.Grade;
                        int currGrade = toRank.Grade;

                        // In order to fill higher level positions, we may need to look
                        // further down than 1 grade if the deficit is high.
                        while (positions.Count > 0 && Ranks.GradeExists(type, prevGrade))
                        {
                            // Update to Rank
                            toRank = Ranks.GetRank(type, currGrade);

                            // Next we select which soldiers are getting promoted
                            IEnumerable<Soldier> selected;
                            if (toRank.MinTimeForConsideration > 0)
                            {
                                // Soldiers who do not meet the Min Time till Retirement are selected after
                                // those who do, so we order promotions by those who are staying awhile, 
                                // then by seniority (Time in grade)
                                selected = (
                                    from soldier in Soldiers[type][prevGrade]
                                    let t2r = soldier.ExitServiceDate.MonthDifference(CurrentDate)
                                    let qualified = (t2r >= toRank.MinTimeForConsideration) ? 1 : 2
                                    orderby qualified, soldier.LastPromotionDate
                                    select soldier
                                ).Take(positions.Count);
                            }
                            else
                            {
                                selected = (
                                    from soldier in Soldiers[type][prevGrade]
                                    orderby soldier.LastPromotionDate
                                    select soldier
                                ).Take(positions.Count);
                            }

                            // Break if we could not find anyone!
                            if (selected.Count() > 0)
                            {
                                // Now we promote the selected soldiers for promotion
                                foreach (Soldier soldier in selected)
                                {
                                    // Grab new job
                                    UnitPosition newPosition = positions.Dequeue();
                                    UnitPosition oldPosition = soldier.Position;

                                    // Assign soldiers new position
                                    soldier.AssignPosition(newPosition, CurrentDate);
                                    AvailablePositions[type][oldPosition.Grade].Push(oldPosition);

                                    // Promote soldier, and move him to the new ranking list
                                    if (soldier.IsPromotable(CurrentDate))
                                    {
                                        // Log Promotion?
                                        if (SkipYears == 0)
                                            RankStatistics[type][prevGrade].TrackPromotionToNextGrade(soldier, CurrentDate);

                                        soldier.Promote(CurrentDate, toRank);
                                        Soldiers[type][prevGrade].Remove(soldier);
                                        Soldiers[type][currGrade].Add(soldier);
                                    }
                                }
                            }

                            // Decrement values
                            currGrade--;
                            prevGrade--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initially populates the soldiers at the start of a simulation
        /// </summary>
        private void PopulateSoldiers()
        {
            // Get all positions
            var positions = ProcessingUnit.GetAllPositions();

            // Clear out old data
            Soldiers = new Dictionary<RankType, Dictionary<int, List<Soldier>>>();
            RankStatistics = new Dictionary<RankType, Dictionary<int, RankGradeStatistics>>();
            AvailablePositions = new Dictionary<RankType, Dictionary<int, Stack<UnitPosition>>>();

            // Soldiers
            foreach (RankType type in RankTypes)
            {
                // Initiate data
                Soldiers.Add(type, new Dictionary<int, List<Soldier>>());
                RankStatistics.Add(type, new Dictionary<int, RankGradeStatistics>());
                AvailablePositions.Add(type, new Dictionary<int, Stack<UnitPosition>>());

                // Add all grades
                foreach (Rank rank in Ranks.GetRankListByType(type).Select(x => x.Value).OrderBy(x => x.Grade))
                {
                    Soldiers[type].Add(rank.Grade, new List<Soldier>());
                    RankStatistics[type].Add(rank.Grade, new RankGradeStatistics());
                    AvailablePositions[type].Add(rank.Grade, new Stack<UnitPosition>());
                }

                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                for (int i = 0; i < positions.Count; i++)
                {
                    var pos = positions[i];
                    if (pos.RankType == type)
                    {
                        var soldier = CreateSoldier(pos.Grade, type);
                        soldier.AssignPosition(pos, CurrentDate);
                        Soldiers[type][pos.Grade].Add(soldier);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new soldier based on the parameters provided
        /// </summary>
        /// <param name="grade">The rank level this new soldier will start out as</param>
        /// <param name="type">Indicates the rank classification for this soldier</param>
        /// <returns></returns>
        private Soldier CreateSoldier(int grade, RankType type)
        {
            Soldier soldier = Generator[type].Spawn();
            soldier.SpawnId = Interlocked.Increment(ref _soldiersSpawned) + 1;
            soldier.FirstName = NameGenerator.GenerateRandomFirstName();
            soldier.LastName = NameGenerator.GenerateRandomLastName();
            soldier.ServiceEntryDate = CurrentDate;
            soldier.LastPromotionDate = CurrentDate;
            soldier.RankInfo = Ranks.GetRank(type, grade);

            // Here we figure out just how many months the soldier will stay in service.
            //  - Soldier.TimeToLive is set by the the Soldier.xml and the SpawnGenerator 
            //    class when initialized.
            CryptoRandom r = new CryptoRandom();
            int toAdd = r.Next(soldier.TimeToLive.Minimum, soldier.TimeToLive.Maximum);
            soldier.ExitServiceDate = CurrentDate.AddMonths(toAdd);

            // Send him to duty!
            return soldier;
        }
    }
}
