using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    public class RandomizedProcedureWrapper
    {
        protected RandomizedProcedure Procedure { get; set; }

        /// <summary>
        /// The Unique OrderedProcedure ID (Row ID)
        /// </summary>
        public int Id => Procedure.Id;

        /// <summary>
        /// Gets the string name of this Generator
        /// </summary>
        public string Name => Procedure.Name;

        /// <summary>
        /// 
        /// </summary>
        public bool CreatesNewSoldiers => Procedure.CreatesNewSoldiers;

        /// <summary>
        /// 
        /// </summary>
        public int NewSoldierProbability => Procedure.NewSoldierProbability;

        // <summary>
        /// 
        /// </summary>
        public CareerGenerator NewSoldierCareer { get; set; } = null;

        /// <summary>
        /// Gets a list or <see cref="OrderedPool"/> entities
        /// </summary>
        public List<RandomizedPool> ProcedurePools { get; set; }

        public RandomizedProcedureWrapper(RandomizedProcedure procedure, SimDatabase db)
        {
            Procedure = procedure;
            ProcedurePools = new List<RandomizedPool>(
                db.Query<RandomizedPool>(
                    "SELECT * FROM RandomizedPool WHERE RandomizedProcedureId=@P0",
                    procedure.Id
                )
            );

            // Fetch Career Generator

            if (CreatesNewSoldiers)
            {
                int id = db.ExecuteScalar<int>(
                    "SELECT CareerGeneratorId FROM RandomizedProcedureCareer WHERE RandomizedProcedureId=@P0", procedure.Id
                );

                if (!SimulationCache.CareerGenerators.TryGetValue(id, out CareerGenerator career))
                    throw new Exception($"Career Generator id {id} does not exist for RandomizedProcedure Id {procedure.Id}!");

                NewSoldierCareer = career;
            }
        }
    }
}
