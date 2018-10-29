using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// This object represents a named <see cref="CareerLengthRange"/> container.
    /// </summary>
    [Table]
    public class CareerGenerator : IEquatable<CareerGenerator>
    {
        #region Columns

        /// <summary>
        /// The Unique Generator ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the string name of this generator
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="CareerSpawnRate"/> entities that reference this 
        /// <see cref="CareerGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<CareerLengthRange> CareerLengths { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SoldierGeneratorCareer"/> entities that reference this 
        /// <see cref="CareerGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierGeneratorCareer> GeneratorCareers { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SoldierCareerAdjustment"/> entities that reference this 
        /// <see cref="CareerGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierCareerAdjustment> CareerAdjustments { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="CareerLengthRange"/> <see cref="SpawnGenerator{T}"/>
        /// </summary>
        protected SpawnGenerator<CareerLengthRange> Generator { get; set; }

        /// <summary>
        /// Indicates whether this generator has been initialized
        /// </summary>
        public bool IsInitialized => (Generator != null);

        /// <summary>
        /// Initializes this <see cref="CareerGenerator"/>. This method must be called
        /// before an <see cref="CareerLengthRange"/>'s can be spawned
        /// </summary>
        public void Initialize()
        {
            // Create generator instance
            Generator = new SpawnGenerator<CareerLengthRange>();

            // Add the rest, if any, spawnable settings
            if (CareerLengths != null)
            {
                // Add spawn setting to list!
                Generator.AddRange(CareerLengths);
            }
        }

        /// <summary>
        /// Spawns a random <see cref="CareerLengthRange"/>
        /// </summary>
        /// <returns></returns>
        public CareerLengthRange Spawn()
        {
            if (Generator == null)
                Initialize();

            return Generator.Spawn();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CareerGenerator);
        }

        public bool Equals(CareerGenerator other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
