using Perscom.Database;
using System.Collections.Generic;

namespace Perscom.Simulation
{
    public static class SimulationCache
    {
        /// <summary>
        /// Gets a list of Cached BilletWrappers by ID
        /// </summary>
        private static Dictionary<int, PositionBlueprintWrapper> PosBlueprintWrappers { get; set; }
        
        private static ProbabilityGenerator<Persona> PersonaGenerator { get; set; }

        /// <summary>
        /// Loads data into the Cache
        /// </summary>
        /// <param name="db"></param>
        public static void Load(SimDatabase db)
        {
            // Create name generator
            PosBlueprintWrappers = new Dictionary<int, PositionBlueprintWrapper>();
            PersonaGenerator = new ProbabilityGenerator<Persona>(db.Personas);
        }

        public static Persona GetRandomPersona()
        {
            return PersonaGenerator.Spawn();
        }

        /// <summary>
        /// Clears all data from the Cache
        /// </summary>
        public static void Clear()
        {
            PosBlueprintWrappers?.Clear();
            PosBlueprintWrappers = null;

            PersonaGenerator?.Clear();
           PersonaGenerator = null;
        }

        public static PositionBlueprintWrapper FetchBillet(PositionBlueprint billet, SimDatabase db)
        {
            if (!PosBlueprintWrappers.ContainsKey(billet.Id))
                PosBlueprintWrappers.Add(billet.Id, new PositionBlueprintWrapper(billet, db));

            return PosBlueprintWrappers[billet.Id];
        }
    }
}
