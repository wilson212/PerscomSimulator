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

        private int totalSoldiers = 0;
        private int totalMaxMonths = 0;

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

        ///<summary>
        /// Gets the promotion info going FROM this grade to the next
        /// RankType => [Grade => PromotionInfo (for grade)]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, PromotionInfo>> Promotions { get; protected set; }

        ///<summary>
        /// RankType => [Grade => PromotionInfo (for grade)]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, PromotionInfo>> Retirements { get; protected set; }

        /// <summary>
        /// Gets the all personel related statistics
        /// </summary>
        public RetirementInfo GlobalRetirements { get; protected set; } = new RetirementInfo();

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
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        protected RandomNameGenerator NameGenerator { get; set; }

        public Simulator(Unit unit)
        {
            ProcessingUnit = unit;
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
            
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                XmlNodeList items = root.SelectNodes($"{Enum.GetName(typeof(RankType), type).ToLower()}/soldier");
                foreach (XmlElement element in items)
                {
                    int prob = Int32.Parse(element.Attributes["probability"].Value);
                    int min = Int32.Parse(element.Attributes["minTime"].Value);
                    int max = Int32.Parse(element.Attributes["maxTime"].Value);
                    Generator[type].Add(new Soldier(prob, new Range<int>(min, max)));
                    if (max > totalMaxMonths) totalMaxMonths = max;
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
        public void Run(int totalYears, int skipYears, IProgress<TaskProgressUpdate> progress, SimulatorSettings settings)
        {
            // Check data
            if (totalYears < skipYears)
                throw new ArgumentException("skipYears cannot be less than totalYears", "skipYears");

            // First, set the end date
            int startMonth = DateTime.Now.Month;
            EndDate = new DateTime(DateTime.Now.Year, startMonth, 1);
            CurrentDate = StartDate = EndDate.AddYears(-totalYears);
            SkipYears = skipYears;
            Settings = settings;

            // Populate soldiers if we need to
            PopulateSoldiers();

            // Main Loop
            while (EndDate != CurrentDate)
            {
                // Update the date
                CurrentDate = CurrentDate.AddMonths(1);

                // Update progress window every 1 year of simulation
                double yearsDone = CurrentDate.Year - StartDate.Year;
                if (progress != null && CurrentDate.Month == 1 && yearsDone > 0)
                {
                    TaskProgressUpdate update = new TaskProgressUpdate();
                    update.MessageText = $"Processing year {yearsDone} of {totalYears}";
                    progress.Report(update);
                }

                // Process Soldiers
                ProcessRetirements();

                // Now do promotions
                ProcessPromotions();

                // If an entire years has gone by, subtract a skip year
                // so we can begin logging again when the time comes.
                if (SkipYears > 0 && CurrentDate.Month % startMonth == 0)
                {
                    SkipYears -= 1;
                }
            }
        }

        /// <summary>
        /// Each soldier is given a TimeToLive when created. This method will clear them
        /// out to make room for new promotions
        /// </summary>
        /// <param name="soldiers"></param>
        private void ProcessRetirements()
        {
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                // Itterate through each Grade
                foreach (KeyValuePair<int, List<Soldier>> data in Soldiers[type])
                for (int i = data.Value.Count; i > 0; i--)
                {
                    // check retirement
                    if (data.Value[i - 1].IsRetiring(CurrentDate))
                    {
                        int rank = data.Key;

                        // Do not log skiped months
                        if (SkipYears == 0)
                        {
                            Soldier s = Soldiers[type][rank][i - 1];
                            var list = Retirements[s.RankInfo.Type];
                            if (!list.ContainsKey(s.RankInfo.Grade))
                                list[s.RankInfo.Grade] = new RetirementInfo();

                            // local vars
                            int grade = s.RankInfo.Grade;

                            // Log retirement data
                            list[grade].TotalPersonel += 1;
                            list[grade].TotalMonthsInService += s.ServiceEntryDate.MonthDifference(CurrentDate);
                            list[grade].TotalMonthsInGrade += s.LastPromotionDate.MonthDifference(CurrentDate);
                            if (s.IsPromotable(CurrentDate))
                                list[grade].TotalPromotable += 1;

                            GlobalRetirements.TotalPersonel += 1;
                            GlobalRetirements.TotalMonthsInGrade += s.LastPromotionDate.MonthDifference(CurrentDate);
                            GlobalRetirements.TotalMonthsInService += s.ServiceEntryDate.MonthDifference(CurrentDate);
                        }

                        Soldiers[type][rank].RemoveAt(i - 1);
                    }
                }
            }
        }

        /// <summary>
        /// This method is called after <see cref="ProcessRetirements(RankType)"/> is 
        /// called, so we can promote soldiers to fill those new empty slots.
        /// </summary>
        private void ProcessPromotions()
        {
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                // Itterate through each Grade
                foreach (KeyValuePair<int, List<Soldier>> data in Soldiers[type].OrderByDescending(x => x.Key))
                {
                    // Grade next this grade
                    Rank toRank = Ranks.GetRank(type, data.Key);
                    int openings = ProcessingUnit.SoldierCounts[type][toRank.Grade] - Soldiers[type][toRank.Grade].Count;
                    if (openings > 0)
                    {
                        // Make sure this is not an entry rank!
                        if (!Ranks.GradeExists(type, data.Key - 1) || !Soldiers[type].ContainsKey(data.Key - 1))
                        {
                            for (int i = 0; i < openings; i++)
                                Soldiers[type][toRank.Grade].Add(CreateSoldier(toRank.Grade, type));

                            continue;
                        }

                        // Grade the previous promotion grade
                        Rank fromRank = Ranks.GetRank(type, data.Key - 1);

                        // Next we select which soldiers are getting promoted
                        List<Soldier> selected = new List<Soldier>(openings);
                        if (toRank.MinTimeForConsideration > 0)
                        {
                            selected.AddRange(
                                (from x in Soldiers[type][fromRank.Grade]
                                 let t2r = x.ExitServiceDate.MonthDifference(CurrentDate)
                                 where t2r >= toRank.MinTimeForConsideration
                                 select x
                                ).Take(openings)
                            );
                        }

                        // If we didn't have enough qualified soldiers for promotion via time considerations,
                        // than we must select people anyways
                        openings = openings - selected.Count;
                        if (openings > 0)
                        {
                            selected.AddRange(
                                Soldiers[type][fromRank.Grade]
                                .OrderBy(x => x.LastPromotionDate)
                                .Take(openings)
                            );
                        }

                        foreach (Soldier soldier in selected.OrderBy(x => x.LastPromotionDate))
                        {
                            // Log statistic data?
                            if (SkipYears == 0)
                            {
                                var list = Promotions[type];
                                if (!list.ContainsKey(soldier.RankInfo.Grade))
                                    list[soldier.RankInfo.Grade] = new PromotionInfo();

                                // Log data
                                list[soldier.RankInfo.Grade].TotalPersonel += 1;
                                list[soldier.RankInfo.Grade].TotalMonthsInService += soldier.ServiceEntryDate.MonthDifference(CurrentDate);
                                list[soldier.RankInfo.Grade].TotalMonthsInGrade += soldier.LastPromotionDate.MonthDifference(CurrentDate);
                            }

                            // Promote soldier, and move him to the new ranking list
                            soldier.Promote(CurrentDate, toRank);
                            Soldiers[type][fromRank.Grade].Remove(soldier);
                            Soldiers[type][toRank.Grade].Add(soldier);
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
            // Clear out old data
            Soldiers = new Dictionary<RankType, Dictionary<int, List<Soldier>>>();
            Promotions = new Dictionary<RankType, Dictionary<int, PromotionInfo>>();
            Retirements = new Dictionary<RankType, Dictionary<int, PromotionInfo>>();

            // Soldiers
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                // Initiate data
                Soldiers.Add(type, new Dictionary<int, List<Soldier>>());
                Promotions.Add(type, new Dictionary<int, PromotionInfo>());
                Retirements.Add(type, new Dictionary<int, PromotionInfo>());

                // Ensure we are setup to run this rank type
                if (!Settings.ProcessRankType(type))
                    continue;

                // Itterate through each Grade
                foreach (KeyValuePair<int, int> data in ProcessingUnit.SoldierCounts[type])
                {
                    Soldiers[type].Add(data.Key, new List<Soldier>());
                    for (int i = 0; i < data.Value; i++)
                        Soldiers[type][data.Key].Add(CreateSoldier(data.Key, type));
                }
            }
        }

        /// <summary>
        /// Creates a new soldier based on the parameters provided
        /// </summary>
        /// <param name="rank">The rank this new soldier will start out as</param>
        /// <param name="officer">True of this is an officer, false otherwise</param>
        /// <returns></returns>
        private Soldier CreateSoldier(int rank, RankType type)
        {
            Soldier soldier = Generator[type].Spawn();
            soldier.SpawnId = Interlocked.Increment(ref totalSoldiers) + 1;
            soldier.FirstName = NameGenerator.GenerateRandomFirstName();
            soldier.LastName = NameGenerator.GenerateRandomLastName();
            soldier.ServiceEntryDate = CurrentDate;
            soldier.LastPromotionDate = CurrentDate;
            soldier.RankInfo = Ranks.GetRank(type, rank);

            // Here we figure out just how many months the soldier will stay in service.
            //  - Soldier.TimeToLive is set by the the Probability.xml and the SpawnGenerator 
            // class when initialized.
            CryptoRandom r = new CryptoRandom();
            int toAdd = r.Next(soldier.TimeToLive.Minimum, soldier.TimeToLive.Maximum);
            soldier.ExitServiceDate = CurrentDate.AddMonths(toAdd);

            // Send him to duty!
            return soldier;
        }
    }
}
