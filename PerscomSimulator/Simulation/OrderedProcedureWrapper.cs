using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    public class OrderedProcedureWrapper
    {
        protected OrderedProcedure Procedure { get; set; }

        /// <summary>
        /// The Unique OrderedProcedure ID (Row ID)
        /// </summary>
        public int Id => Procedure.Id;

        /// <summary>
        /// Gets the string name of this Generator
        /// </summary>
        public string Name => Procedure.Name;

        /// <summary>
        /// Gets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        public int RankId => Procedure.RankId;

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
        public List<OrderedPool> ProcedurePools { get; set; }

        public OrderedProcedureWrapper(OrderedProcedure procedure, SimDatabase db)
        {
            Procedure = procedure;
            ProcedurePools = new List<OrderedPool>(
                db.Query<OrderedPool>(
                    "SELECT * FROM OrderedPool WHERE OrderedProcedureId=@P0 ORDER BY Precedence", 
                    procedure.Id
                )
            );

            // Fetch Career Generator

            if (CreatesNewSoldiers)
            {
                int id = db.ExecuteScalar<int>(
                    "SELECT CareerGeneratorId FROM OrderedProcedureCareer WHERE OrderedProcedureId=@P0", procedure.Id
                );

                if (!SimulationCache.CareerGenerators.TryGetValue(id, out CareerGenerator career))
                    throw new Exception($"Career Generator id {id} does not exist for OrderedProcedure Id {procedure.Id}!");

                NewSoldierCareer = career;
            }
        }
    }
}
