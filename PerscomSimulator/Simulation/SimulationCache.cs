using Perscom.Database;
using System.Collections.Generic;

namespace Perscom.Simulation
{
    public static class SimulationCache
    {
        /// <summary>
        /// Gets or Sets the <see cref="RandomNameGenerator"/> used to assign
        /// names to <see cref="Soldier"/>s when they spawn
        /// </summary>
        public static RandomNameGenerator NameGenerator { get; private set; }

        /// <summary>
        /// Gets a list of Cached BilletWrappers by ID
        /// </summary>
        private static Dictionary<int, BilletWrapper> Billets { get; set; }

        /// <summary>
        /// Gets a list of Cached CareerGenerator's by ID
        /// </summary>
        public static Dictionary<int, CareerGenerator> CareerGenerators { get; private set; }

        /// <summary>
        /// Loads data into the Cache
        /// </summary>
        /// <param name="db"></param>
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

        /// <summary>
        /// Clears all data from the Cache
        /// </summary>
        public static void Clear()
        {
            Billets?.Clear();
            Billets = null;

            CareerGenerators?.Clear();
            CareerGenerators = null;
        }

        public static BilletWrapper FetchBillet(Billet billet, SimDatabase db)
        {
            if (!Billets.ContainsKey(billet.Id))
                Billets.Add(billet.Id, new BilletWrapper(billet, db));

            return Billets[billet.Id];
        }
    }
}
