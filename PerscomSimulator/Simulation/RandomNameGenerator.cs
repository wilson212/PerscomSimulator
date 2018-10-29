using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Perscom.Simulation
{
    /// <summary>
    /// This class is used to generate random first and last names 
    /// </summary>
    public class RandomNameGenerator
    {
        /// <summary>
        /// A list of first names
        /// </summary>
        protected List<string> FirstNames { get; set; }

        /// <summary>
        /// A list of last names
        /// </summary>
        protected List<string> LastNames { get; set; }

        /// <summary>
        /// The RNG class
        /// </summary>
        protected CryptoRandom Rng { get; set; }

        public RandomNameGenerator()
        {
            Rng = new CryptoRandom();
            FirstNames = new List<string>();
            LastNames = new List<string>();

            LoadNames();
        }

        /// <summary>
        /// Generates a random first and last name, and returns
        /// them as a string
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomFirstAndLastName()
        {
            return $"{GenerateRandomFirstName()} {GenerateRandomLastName()}";
        }

        /// <summary>
        /// Generates and returns a random first name
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomFirstName()
        {
            int index = Rng.Next(0, FirstNames.Count - 1);
            return FirstNames[index];
        }

        /// <summary>
        /// Generates and returns a random last name
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomLastName()
        {
            int index = Rng.Next(0, LastNames.Count - 1);
            return LastNames[index];
        }

        /// <summary>
        /// Loads the first and last names into memory from the Config/Names.xml
        /// </summary>
        private void LoadNames()
        {
            // Ensure the file exists
            string filePath = Path.Combine(Program.RootPath, "Config", "Names.xml");
            if (!File.Exists(filePath))
                throw new Exception($"Names.xml file is missing!");

            // Load the document
            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var root = document.DocumentElement;

            // ======================================================================
            // Load first names
            XmlNodeList items = root.SelectNodes("first/name");
            foreach (XmlElement element in items)
                FirstNames.Add(element.InnerText);

            // ======================================================================
            // Load last names
            items = root.SelectNodes("last/name");
            foreach (XmlElement element in items)
                LastNames.Add(element.InnerText);
        }
    }
}
