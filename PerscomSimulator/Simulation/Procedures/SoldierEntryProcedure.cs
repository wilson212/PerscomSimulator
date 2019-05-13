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
        protected SimDatabase Database { get; set; }

        public int CareerGeneratorId { get; protected set; }

        public SoldierEntryProcedure(SimDatabase db, Billet billet) : base(db, billet)
        {
            // Store database connection
            Database = db;

            // Get career generator ID
            CareerGeneratorId = db.ExecuteScalar<int>("SELECT CareerGeneratorId FROM BilletCareer WHERE BilletId=@P0", billet.Id);
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        /// <remarks>Creates a new <see cref="Soldier"/> and add's it to the database</remarks>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            // Create a new soldier object
            Soldier soldier = new Soldier
            {
                FirstName = SimulationCache.NameGenerator.GenerateRandomFirstName(),
                LastName = SimulationCache.NameGenerator.GenerateRandomLastName(),
                EntryIterationId = date.Id,
                LastPromotionIterationId = date.Id,
                LastGradeChangeIterationId = date.Id,
                RankId = position.Billet.Rank.Id,
                SpecialtyId = position.Billet.Specialty.Id
            };

            // Grab career generator
            if (!SimulationCache.CareerGenerators.TryGetValue(CareerGeneratorId, out CareerGenerator career))
                throw new Exception($"Career Generator id {CareerGeneratorId} does not exist for Billet Id {Billet.Id}!");

            // Assign the soldiers new career length based on Rank Type
            CareerLengthRange careerLength = career.Spawn();
            soldier.CareerLengthId = careerLength.Id;
            soldier.ExitIterationId = date.Id + careerLength.GenerateLength();

            // Add soldier to database
            Database.Soldiers.Add(soldier);

            // Create soldier wrapper
            var wrapper = new SoldierWrapper(soldier, position.Billet.Rank, position.Billet.Specialty, date, Database);

            // Send him to duty!
            type = SpawnSoldierType.CreateNew;
            return wrapper;
        }
    }
}
