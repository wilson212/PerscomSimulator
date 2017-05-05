using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Perscom.Simulation
{
    public class Unit : ICloneable
    {
        /// <summary>
        /// Gets or Sets the name of this Unit instance
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the template XML file name for this unit
        /// </summary>
        public string TemplateName { get; protected set; }

        /// <summary>
        /// The parent unit of this unit instance
        /// </summary>
        public Unit ParentUnit { get; set; }

        /// <summary>
        /// Gets or Sets a list of all soldier positions in this Unit (excluding sub units).
        /// </summary>
        public List<UnitPosition> Positions { get; set; }

        /// <summary>
        /// A list of all <see cref="Unit"/>s that fall under this one
        /// </summary>
        protected List<Unit> Subunits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected static Dictionary<string, Unit> UnitCache { get; set; }

        /// <summary>
        /// Grade => Soldier Array
        /// </summary>
        public static Dictionary<RankType, Dictionary<int, int>> SoldierCounts { get; protected set; }

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Unit()
        {
            UnitCache = new Dictionary<string, Unit>();
        }

        /// <summary>
        /// Creates a new instance of Unit
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="name"></param>
        protected Unit(string templateName, string name)
        {
            Name = name;
            TemplateName = templateName;
            Positions = new List<UnitPosition>();
            Subunits = new List<Unit>();
        }

        /// <summary>
        /// Returns a List of all positions in this unit, and it's sub units.
        /// </summary>
        /// <returns></returns>
        public List<UnitPosition> GetAllPositions()
        {
            var val = new List<UnitPosition>(Positions);
            foreach (Unit sub in Subunits)
            {
                val.AddRange(sub.GetAllPositions());
            }

            return val;
        }

        /// <summary>
        /// Creates a new <see cref="Unit"/> Instance, and all of its sub-units
        /// recursivly.
        /// </summary>
        /// <param name="fileName">The XML Unit Template file name</param>
        /// <param name="unitName">Optionally set the Unit name for this unit</param>
        /// <param name="parentUnit">Optionally set the parent Unit for this Unit</param>
        /// <returns></returns>
        public static Unit CreateType(string fileName, string unitName = null, Unit parentUnit = null)
        {
            // Add extension
            if (!fileName.EndsWith(".xml"))
                fileName += ".xml";

            // Check Cache
            Unit master = (UnitCache.ContainsKey(fileName)) ?  UnitCache[fileName] : LoadUnit(fileName);

            // Set unit variables
            Unit sub = (Unit)master.Clone();
            sub.Name = unitName ?? Path.GetFileNameWithoutExtension(fileName);
            return sub;
        }

        /// <summary>
        /// Creates or returns a cache'd copy of a <see cref="Unit"/> from the XML file
        /// </summary>
        /// <param name="fileName">The XML Unit Template file name</param>
        /// <returns></returns>
        protected static Unit LoadUnit(string fileName)
        {
            // Ensure the unit exists
            string filePath = Path.Combine(Program.RootPath, "Units", fileName);
            if (!File.Exists(filePath))
                throw new Exception($"Unit \"{fileName}\" does not exist");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            // Create the unit
            Unit unit = new Unit(fileName, "master");
            UnitCache.Add(fileName, unit);

            // ======================================================================
            // Load unit positions
            XmlNodeList positions = root.SelectNodes("info/positions/position");
            foreach (XmlElement element in positions)
            {
                // Create new position
                UnitPosition position = new UnitPosition();
                position.ParentUnit = unit;
                position.Name = element.Attributes["name"].Value;
                position.EntryLevel = element.Attributes["entryLevel"]?.InnerText != null;

                // Get rank information
                char code = element.Attributes["rank"].InnerText[0];
                int grade = Int32.Parse(element.Attributes["rank"].InnerText.Substring(1));
                position.RankType = Ranks.GetRankTypeByCode(code);
                position.Grade = grade;

                unit.Positions.Add(position);
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
                var sub = CreateType(type, name);
                unit.Subunits.Add(sub);
            }

            return unit;
        }

        /// <summary>
        /// Recursivly sets the parent unit of all sub-units
        /// </summary>
        public void SetParentsRecursivly()
        {
            // Set positions parent
            for (int i = 0; i < Positions.Count; i++)
                Positions[i].ParentUnit = this;

            // Set positions parent
            for (int i = 0; i < Subunits.Count; i++)
            {
                Subunits[i].ParentUnit = this;
                Subunits[i].SetParentsRecursivly();
            }
        }

        public object Clone()
        {
            var newUnit = new Unit(TemplateName, Name)
            {
                Positions = (List<UnitPosition>)Positions.Clone(),
                Subunits = (List<Unit>)Subunits.Clone()
            };
            newUnit.SetParentsRecursivly();

            return newUnit;
        }

        public override string ToString()
        {
            return (ParentUnit == null) ? Name : String.Concat(Name, ", ", ParentUnit.ToString());
        }
    }
}
