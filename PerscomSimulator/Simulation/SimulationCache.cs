using Perscom.Database;
using Perscom.Simulation.Procedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    public static class SimulationCache
    {
        /// <summary>
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        public static RandomNameGenerator NameGenerator { get; private set; }

        private static Dictionary<int, BilletWrapper> Billets { get; set; }

        public static Dictionary<int, CareerGenerator> CareerGenerators { get; private set; }

        public static void Load(SimDatabase db)
        {
            // Create name generator
            NameGenerator = new RandomNameGenerator();
            Billets = new Dictionary<int, BilletWrapper>();
            CareerGenerators = new Dictionary<int, CareerGenerator>();

            // Fetch career generators
            foreach (var item in db.CareerGenerators)
            {
                // Initialize generator
                item.Initialize();

                // Add item
                CareerGenerators.Add(item.Id, item);
            }

        }

        public static void Clear()
        {

        }

        public static BilletWrapper FetchBillet(Billet billet, SimDatabase db)
        {
            if (!Billets.ContainsKey(billet.Id))
                Billets.Add(billet.Id, new BilletWrapper(billet, db));

            return Billets[billet.Id];
        }
    }
}
