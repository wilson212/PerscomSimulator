using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Perscom.Simulation
{
    /// <summary>
    /// This class is used to generate random first and last names,
    /// filtered by gender and race.
    /// </summary>
    public static class RandomNameGenerator
    {
        /// <summary>
        /// Male first names keyed by Race
        /// </summary>
        private static Dictionary<Race, ProbabilityGenerator<WeightedName>> MaleFirstNames { get; set; }

        /// <summary>
        /// Female first names keyed by Race
        /// </summary>
        private static Dictionary<Race, ProbabilityGenerator<WeightedName>> FemaleFirstNames { get; set; }

        /// <summary>
        /// Last names keyed by Race
        /// </summary>
        private static Dictionary<Race, ProbabilityGenerator<WeightedName>> LastNames { get; set; }

        /// <summary>
        /// A probability-based generator used to determine race distributions
        /// during name generation.
        /// </summary>
        private static ProbabilityGenerator<Prospect<Race>> RaceGenerator { get; set; }

        /// <summary>
        /// All available races that have been loaded
        /// </summary>
        public static Race[] AvailableRaces { get; private set; }

        /// <summary>
        /// Static initializer — loads names on first access
        /// </summary>
        static RandomNameGenerator()
        {
            MaleFirstNames = new Dictionary<Race, ProbabilityGenerator<WeightedName>>();
            FemaleFirstNames = new Dictionary<Race, ProbabilityGenerator<WeightedName>>();
            LastNames = new Dictionary<Race, ProbabilityGenerator<WeightedName>>();
            RaceGenerator = new ProbabilityGenerator<Prospect<Race>>();

            LoadNames();
            AvailableRaces = MaleFirstNames.Keys.ToArray();
        }

        /// <summary>
        /// Generates a random first name for the given gender and race.
        /// </summary>
        public static string GetFirstName(bool isMale, Race race)
        {
            var gen = isMale ? MaleFirstNames[race] : FemaleFirstNames[race];
            return gen.Spawn().Name;
        }

        /// <summary>
        /// Generates a random last name for the given race.
        /// </summary>
        public static string GetLastName(Race race)
        {
            return LastNames[race].Spawn().Name;
        }

        /// <summary>
        /// Convenience overload: returns "FirstName LastName"
        /// </summary>
        public static string GetFullName(bool isMale, Race race)
        {
            return $"{GetFirstName(isMale, race)} {GetLastName(race)}";
        }

        /// <summary>
        /// Returns a random Race from the available loaded races.
        /// </summary>
        public static Race GetRandomRace()
        {
            return RaceGenerator.Spawn().Value;
        }

        /// <summary>
        /// Loads and initializes name data from an XML configuration file.
        /// Names are categorized by race, gender, and type (first or last).
        /// This method populates internal dictionaries used for random name generation.
        /// </summary>
        /// <exception cref="Exception">
        /// Thrown if the required "Names.xml" configuration file is missing or cannot be loaded.
        /// </exception>
        private static void LoadNames()
        {
            string filePath = Path.Combine(Program.RootPath, "Config", "Names.xml");
            if (!File.Exists(filePath))
                throw new Exception("Names.xml file is missing!");

            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            foreach (XmlElement raceElement in root.SelectNodes("race"))
            {
                string raceName = raceElement.GetAttribute("name");
                if (!Enum.TryParse<Race>(raceName, true, out var race))
                    continue;

                // Parse race weight (default 1)
                int raceWeight = 1;
                string raceWeightAttr = raceElement.GetAttribute("weight");
                if (!string.IsNullOrEmpty(raceWeightAttr))
                    int.TryParse(raceWeightAttr, out raceWeight);

                // Add race to generator
                RaceGenerator.Add(new Prospect<Race>(raceWeight, race));

                // Add male first names
                MaleFirstNames[race] = new ProbabilityGenerator<WeightedName>(
                    LoadWeightedNames(raceElement, "first/male/name")
                );

                // Add female first names
                FemaleFirstNames[race] = new ProbabilityGenerator<WeightedName>(
                    LoadWeightedNames(raceElement, "first/female/name")
                );

                // Add last names
                LastNames[race] = new ProbabilityGenerator<WeightedName>(
                    LoadWeightedNames(raceElement, "last/name")
                );
            }
        }

        /// <summary>
        /// Loads a list of weighted names from the specified XML parent element using the given XPath query.
        /// </summary>
        /// <param name="parent">The parent XML element from which the names will be extracted.</param>
        /// <param name="xpath">The XPath query used to locate name elements within the parent XML element.</param>
        /// <returns>A list of <see cref="WeightedName"/> objects extracted from the XML.</returns>
        private static List<WeightedName> LoadWeightedNames(XmlElement parent, string xpath)
        {
            var list = new List<WeightedName>();
            foreach (XmlElement el in parent.SelectNodes(xpath))
            {
                int weight = 1;
                string weightAttr = el.GetAttribute("weight");
                if (!string.IsNullOrEmpty(weightAttr))
                    int.TryParse(weightAttr, out weight);

                list.Add(new WeightedName(el.InnerText, weight));
            }
            return list;
        }
    }
}
