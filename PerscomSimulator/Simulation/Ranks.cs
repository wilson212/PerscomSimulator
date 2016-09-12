using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Perscom.Simulation
{
    public static class Ranks
    {
        public static Dictionary<RankType, Dictionary<int, Rank>> RankList { get; set; }

        /// <summary>
        /// Loads the Ranks.xml file
        /// </summary>
        public static void Load()
        {
            RankList = new Dictionary<RankType, Dictionary<int, Rank>>()
            {
                { RankType.Enlisted, new Dictionary<int, Rank>() },
                { RankType.Officer, new Dictionary<int, Rank>() },
                { RankType.Warrant, new Dictionary<int, Rank>() }
            };

            // Ensure the unit exists
            string filePath = Path.Combine(Program.RootPath, "Config", "Ranks.xml");
            if (!File.Exists(filePath))
                throw new Exception($"Ranks.xml file is missing!");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            // ======================================================================
            // Load probabilities
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                string name = Enum.GetName(typeof(RankType), type).ToLower();
                XmlNodeList items = root.SelectNodes($"{name}/rank");
                foreach (XmlElement element in items)
                {
                    XmlAttribute attr = element.Attributes["grade"];
                    int rank = (int)Int32.Parse(attr.InnerText);
                    int t2r = Int32.Parse(element.Attributes["minTimeForRet"]?.Value ?? "0");
                    int tig = Int32.Parse(element.Attributes["promotableAt"]?.Value ?? "12");
                    int max = Int32.Parse(element.Attributes["maxTimeInGrade"]?.Value ?? "0");
                    Range<int> stature = (element.Attributes["stature"]?.Value != null)
                        ? ParseStature(element.Attributes["stature"].Value)
                        : null;

                    // Add the rank to the list
                    RankList[type].Add(rank, new Rank()
                    {
                        Type = type,
                        Name = element.Attributes["name"].Value,
                        Grade = rank,
                        MinTimeForConsideration = t2r,
                        PromotableAt = tig,
                        MaxTimeInGrade = max,
                        Stature = stature
                    });
                }
            }
        }

        private static Range<int> ParseStature(string value)
        {
            var stature = new Range<int>();
            if (value.Contains(","))
            {
                string[] parts = value.Split(',');
                stature.Minimum = Int32.Parse(parts[0].Trim());
                stature.Maximum = Int32.Parse(parts[1].Trim());
            }
            else
            {
                stature.Minimum = Int32.Parse(value);
                stature.Maximum = 0;
            }
            return stature;
        }

        /// <summary>
        /// Gets the list of Grade => <see cref="Rank"/> for the
        /// specified <see cref="RankList"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<int, Rank> GetRankListByType(RankType type) => RankList[type];

        public static Rank GetRank(RankType type, int grade) => RankList[type][grade];

        public static Rank GetEntryLevelRank(RankType type) => RankList[type].First().Value;

        public static bool GradeExists(RankType type, int grade) => RankList[type].ContainsKey(grade);

        /// <summary>
        /// Gets the <see cref="RankType"/> based on the type <see cref="char"/> code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RankType GetRankTypeByCode(char code)
        {
            switch (Char.ToLower(code))
            {
                case 'e': return RankType.Enlisted;
                case 'o': return RankType.Officer;
                default: return RankType.Warrant;
            }
        }
    }
}
