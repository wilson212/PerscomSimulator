using Perscom.Database;
using System;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a selection procedure that creates a new <see cref="SoldierWrapper"/>
    /// based off of the position parameters
    /// </summary>
    public class SoldierEntryProcedure : AbstractSelectionProcedure
    {
        /// <summary>
        /// The current Simulation Database connection
        /// </summary>
        protected SimDatabase Database { get; set; }

        public SoldierEntryProcedure(SimDatabase db, PositionBlueprint blueprint) : base(db, blueprint)
        {
            // Store database connection
            Database = db;
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        /// <remarks>Creates a new <see cref="Soldier"/> and add's it to the database</remarks>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            // Send him to duty!
            type = SpawnSoldierType.CreateNew;
            
            // Pick a random Persona blueprint
            var persona = SimulationCache.GetRandomPersona();
            var rank = position.BlueprintWrapper.Rank;
            var occupation = position.BlueprintWrapper.Occupation;

            return SoldierWrapper.Spawn(persona, rank, date, occupation, Database);
        }
    }
}
