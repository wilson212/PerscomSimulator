﻿using System;
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
        private int totalSoldiers = 0;

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
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        protected RandomNameGenerator NameGenerator { get; set; }

        /// <summary>
        /// Creates a new Simulator instance
        /// </summary>
        /// <param name="unit">The unit to run the simulation on</param>
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
        public void Run(int totalYears, int skipYears, IProgress<TaskProgressUpdate> progress, SimulatorSettings settings, CancellationToken token)
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
                // Quit if cancelled
                if (token.IsCancellationRequested)
                    return;

                // Update the date
                CurrentDate = CurrentDate.AddMonths(1);

                // Update progress window every 1 year of simulation
                int yearsDone = CurrentDate.Year - StartDate.Year;
                if (CurrentDate.Month == startMonth && yearsDone > 0)
                {
                    TaskProgressUpdate update = new TaskProgressUpdate();
                    update.MessageText = $"Processing year {yearsDone} of {totalYears}";
                    progress.Report(update);
                }

                // Process Retirements first!
                ProcessRetirements();

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
        /// Returns the total percentage of players who made it to the specified
        /// rank and grade in the simulation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public double TotalSelectionRate(RankType type, int grade)
        {
            // Make sure the rank exists in the simulation
            if (totalSoldiers == 0 || !RankStatistics[type].ContainsKey(grade))
                return 0;

            // Set math variables
            int cumulative = 0;
            Rank current = Ranks.GetRank(type, grade);
            double total = RankStatistics[type][grade].TotalSoldiers;

            foreach (Rank r in Ranks.GetPrevousGrades(type, grade))
            {
                // Add grade statistics if missing
                if (!RankStatistics[type].ContainsKey(r.Grade))
                    continue;

                // Log retirement data for current rank/grade
                var info = RankStatistics[type][r.Grade];
                cumulative += info.TotalRetirements;
            }

            return (cumulative == 0) ? 0 : Math.Round(total / (total + cumulative), 5) * 100;
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

                // Itterate through each Soldier in each Grade
                foreach (KeyValuePair<int, List<Soldier>> data in Soldiers[type])
                {
                    // Use a FOR loop here so we can remove soldiers as we itterate
                    for (int i = data.Value.Count; i > 0; i--)
                    {
                        // Extract rank grade and soldier object
                        int grade = data.Key;
                        Soldier soldier = Soldiers[type][grade][i - 1];

                        // check retirement
                        if (soldier.IsRetiring(CurrentDate))
                        {
                            // Do not log skiped months
                            if (SkipYears == 0)
                            {
                                // Add grade statistics if missing
                                if (!RankStatistics[type].ContainsKey(grade))
                                    RankStatistics[type].Add(grade, new RankGradeStatistics());

                                // Log retirement data for current rank/grade
                                RankStatistics[type][grade].AddRetiree(soldier, CurrentDate);
                            }

                            // Finally, remove the retired soldier
                            Soldiers[type][grade].RemoveAt(i - 1);
                        }
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
                            // Soldiers who do not meet the Min Time till Retirement are selected after
                            // those who do, so we order promotions by those who are staying awhile, 
                            // then by seniority (Time in grade)
                            selected.AddRange(
                                (from soldier in Soldiers[type][fromRank.Grade]
                                 let t2r = soldier.ExitServiceDate.MonthDifference(CurrentDate)
                                 let qualified = (t2r >= toRank.MinTimeForConsideration) ? 0 : 1 // Reversed
                                 orderby qualified, soldier.LastPromotionDate
                                 select soldier
                                ).Take(openings)
                            );
                        }
                        else
                        {
                            selected.AddRange(
                                (from soldier in Soldiers[type][fromRank.Grade]
                                 orderby soldier.LastPromotionDate
                                 select soldier
                                ).Take(openings)
                            );
                        }

                        // Now we promote the selected soldiers for promotion
                        foreach (Soldier soldier in selected)
                        {
                            // Log statistic data?
                            if (SkipYears == 0)
                            {
                                // Add grade statistics if missing
                                if (!RankStatistics[type].ContainsKey(fromRank.Grade))
                                    RankStatistics[type].Add(fromRank.Grade, new RankGradeStatistics());

                                // Log retirement data for current rank/grade
                                RankStatistics[type][fromRank.Grade].TrackPromotionToNextGrade(soldier, CurrentDate);
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
            RankStatistics = new Dictionary<RankType, Dictionary<int, RankGradeStatistics>>();

            // Soldiers
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                // Initiate data
                Soldiers.Add(type, new Dictionary<int, List<Soldier>>());
                RankStatistics.Add(type, new Dictionary<int, RankGradeStatistics>());

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
        /// <param name="grade">The rank level this new soldier will start out as</param>
        /// <param name="type">Indicates the rank classification for this soldier</param>
        /// <returns></returns>
        private Soldier CreateSoldier(int grade, RankType type)
        {
            Soldier soldier = Generator[type].Spawn();
            soldier.SpawnId = Interlocked.Increment(ref totalSoldiers) + 1;
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
