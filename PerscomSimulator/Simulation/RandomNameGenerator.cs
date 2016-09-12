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
    /// This class 
    /// </summary>
    public class RandomNameGenerator
    {
        protected List<string> FirstNames { get; set; }

        protected List<string> LastNames { get; set; }

        protected CryptoRandom Rng { get; set; }

        public RandomNameGenerator()
        {
            Rng = new CryptoRandom();
            FirstNames = new List<string>();
            LastNames = new List<string>();

            LoadNames();
        }

        public string GenerateRandomFirstAndLastName()
        {
            return $"{GenerateRandomFirstName()} {GenerateRandomLastName()}";
        }

        public string GenerateRandomFirstName()
        {
            int index = Rng.Next(0, FirstNames.Count - 1);
            return FirstNames[index];
        }

        public string GenerateRandomLastName()
        {
            int index = Rng.Next(0, LastNames.Count - 1);
            return LastNames[index];
        }

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
