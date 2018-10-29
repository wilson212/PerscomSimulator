using System;
using System.Collections.Generic;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    [Table]
    public class SoldierGenerator : IEquatable<SoldierGenerator>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGenerator ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Generator
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, Default(0)]
        public bool CreatesNewSoldiers { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        [Column, Default(0)]
        public int NewSoldierProbability { get; set; } = 0;

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="SoldierGeneratorPool"/> entities that reference this 
        /// <see cref="SoldierGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierGeneratorPool> SpawnPools { get; set; }

        // <summary>
        /// Gets a list of <see cref="SoldierGeneratorCareer"/> entities that reference this 
        /// <see cref="SoldierGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierGeneratorCareer> NewSoldierCareer { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletSpawnSetting"/> entities that reference this 
        /// <see cref="SoldierGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSpawnSetting> BilletSpawns { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="SpawnedSoldier"/> <see cref="SpawnGenerator{T}"/>
        /// </summary>
        protected SpawnGenerator<SpawnedSoldier> Generator { get; set; }

        /// <summary>
        /// Indicates whether this generator has been initialized
        /// </summary>
        public bool IsInitialized => (Generator != null);

        /// <summary>
        /// Initializes this <see cref="SoldierGenerator"/>. This method must be called
        /// before an <see cref="SpawnedSoldier"/>'s can be spawned
        /// </summary>
        public void Initialize()
        {
            // Create generator instance
            Generator = new SpawnGenerator<SpawnedSoldier>();

            // Add the new soldier spawnable entity
            if (CreatesNewSoldiers)
            {
                // Grab generator
                var item = NewSoldierCareer.FirstOrDefault();
                if (item != null && item != default(SoldierGeneratorCareer))
                {
                    // Add spawn setting to list!
                    Generator.Add(new SpawnedSoldier()
                    {
                        Type = SpawnSoldierType.CreateNew,
                        Probability = NewSoldierProbability,
                        Career = item.CareerGenerator
                    });
                }
            }

            // Add the rest, if any, spawnable settings
            if (SpawnPools != null)
            {
                foreach (var item in SpawnPools)
                {
                    // Add spawn setting to list!
                    Generator.Add(new SpawnedSoldier()
                    {
                        Type = SpawnSoldierType.TakeFromExistingPool,
                        Probability = item.Probability,
                        Rank = item.Rank,
                        Career = item.CareerGenerator,
                        Pool = item
                    });
                }
            }
        }

        /// <summary>
        /// Spawns a random <see cref="SpawnedSoldier"/> based
        /// on the set probability
        /// </summary>
        public SpawnedSoldier Spawn()
        {
            if (Generator == null)
                Initialize();

            return Generator.Spawn();
        }

        public bool Equals(SoldierGenerator other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SoldierGenerator);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
