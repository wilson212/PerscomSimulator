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
        [Column, Required, Default(1)]
        public bool CreatesNewSoldiers { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Column, Default(100)]
        public int NewSoldierProbability { get; set; } = 100;

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="SoldierGeneratorSetting"/> entities that reference this 
        /// <see cref="SoldierGenerator"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierGeneratorSetting> SpawnSettings { get; set; }

        #endregion

        protected SpawnGenerator<SoldierGeneratorSetting> Generator { get; set; }

        public void Initialize()
        {
            // Create generator instance
            Generator = new SpawnGenerator<SoldierGeneratorSetting>();

            // Add the new soldier spawnable entity
            if (CreatesNewSoldiers)
            {
                Generator.Add(new SoldierGeneratorSetting() { Probability = NewSoldierProbability });
            }

            // Add the rest, if any, spawnable settings
            if (SpawnSettings != null)
                Generator.AddRange(SpawnSettings.ToList());
        }

        public SoldierGeneratorSetting Spawn()
        {
            return Generator?.Spawn();
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
