using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Perscom.Simulation
{
    /// <summary>
    /// This class contains statistical data for unit XML templates
    /// </summary>
    public class UnitTemplate
    {

        #region Statics

        protected static Dictionary<string, UnitTemplate> UnitCache { get; set; }

        static UnitTemplate()
        {
            UnitCache = new Dictionary<string, UnitTemplate>();
        }

        /// <summary>
        /// Recursivly loads the units from the specified fileName.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UnitTemplate Load(string fileName)
        {
            // Add extension
            if (!fileName.EndsWith(".xml"))
                fileName += ".xml";

            // Check Cache
            if (UnitCache.ContainsKey(fileName))
                return UnitCache[fileName];

            // Ensure the unit exists
            string filePath = Path.Combine(Program.RootPath, "Units", fileName);
            if (!File.Exists(filePath))
                throw new Exception($"Unit \"{fileName}\" does not exist");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            // Create the unit
            UnitTemplate unit = new UnitTemplate();
            UnitCache.Add(fileName, unit);

            // ======================================================================
            // Load unit positions
            XmlNodeList positions = root.SelectNodes("info/positions/position/@rank");
            foreach (XmlAttribute attr in positions)
            {
                char code = attr.InnerText[0];
                int grade = Int32.Parse(attr.InnerText.Substring(1));
                RankType type = Ranks.GetRankTypeByCode(code);
                if (unit.SoldierCounts[type].ContainsKey(grade))
                    unit.SoldierCounts[type][grade] += 1;
                else
                    unit.SoldierCounts[type].Add(grade, 1);

                unit.TotalSoldiers += 1;
            }

            // ======================================================================
            // Load sub units
            XmlNodeList subUnits = root.SelectNodes("info/subunits/unit");
            foreach (XmlElement element in subUnits)
            {
                string name = element.Attributes["name"].Value;
                string type = element.Attributes["type"].Value;

                // Add extension
                if (!type.EndsWith(".xml"))
                    type += ".xml";

                // Load unit
                UnitTemplate sub = UnitTemplate.Load(type);
                unit.AddSubUnit(sub);
            }

            return unit;
        }

        #endregion

        /// <summary>
        /// A list of all <see cref="UnitTemplate"/>s that fall under this one
        /// </summary>
        protected List<UnitTemplate> Subunits { get; set; }

        /// <summary>
        /// RankType => [Grade => Soldier Count]
        /// </summary>
        public Dictionary<RankType, Dictionary<int, int>> SoldierCounts { get; protected set; }

        /// <summary>
        /// Gets the total number of soldier positions in the unit
        /// </summary>
        public int TotalSoldiers { get; protected set; } = 0;

        protected UnitTemplate()
        {
            Subunits = new List<UnitTemplate>();
            SoldierCounts = new Dictionary<RankType, Dictionary<int, int>>();
            foreach (RankType type in Enum.GetValues(typeof(RankType)))
            {
                SoldierCounts.Add(type, new Dictionary<int, int>());

                // Add rank grades... all of them!
                foreach (var x in Ranks.GetRankListByType(type).OrderBy(x => x.Value.Grade))
                {
                    int grade = x.Value.Grade;
                    SoldierCounts[type].Add(grade, 0);
                }
            }
        }

        /// <summary>
        /// Adds a child <see cref="UnitTemplate"/> to this unit
        /// </summary>
        /// <param name="sub"></param>
        protected void AddSubUnit(UnitTemplate sub)
        {
            Subunits.Add(sub);

            // Add soldier counts
            foreach (var soldiers in sub.SoldierCounts)
            {
                foreach (var grades in soldiers.Value)
                {
                    SoldierCounts[soldiers.Key][grades.Key] += grades.Value;
                    TotalSoldiers += grades.Value;
                }
            }
        }
    }
}
