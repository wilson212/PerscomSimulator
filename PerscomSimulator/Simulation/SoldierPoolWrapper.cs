using Perscom.Database;
using System;

namespace Perscom.Simulation
{
    public enum SpawnSoldierType
    {
        TakeFromExistingPool,
        CreateNew
    };

    /// <summary>
    /// Represents a an array spawnable soldier settings that is used by the Simulator
    /// to fill empty positions.
    /// </summary>
    public class SoldierPoolWrapper<T> : ISpawnable, IEquatable<SoldierPoolWrapper<T>>
    {
        /// <summary>
        /// Gets or sets whether the simulator is to create a new soldier, or take
        /// an existing soldier from a unit pool
        /// </summary>
        public SpawnSoldierType Type { get; set; }

        /// <summary>
        /// Used only when the <see cref="Type"/> is set to <see cref="SpawnSoldierType.TakeFromExistingPool"/>.
        /// This indicates the rank of soldiers to spawn from an existing pool.
        /// </summary>
        public Rank Rank { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CareerGenerator"/> to use, if any
        /// </summary>
        public CareerGenerator Career { get; set; }

        /// <summary>
        /// Indicates the spawnable probability of this object
        /// </summary>
        public int Probability { get; set; }

        /// <summary>
        /// Indicates whether we use Rank.Id or Rank.Grade
        /// </summary>
        public bool UseRankGrade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public T Pool { get; set; }

        /// <summary>
        /// Compares a <see cref="SpawnedSoldier"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierPoolWrapper<T> other)
        {
            return (Rank.Id == other.Rank.Id && Type == other.Type);
        }

        public bool Equals(SoldierPoolWrapper<T> other) => IsDuplicateOf(other);

        public override bool Equals(object obj) => Equals(obj as SoldierPoolWrapper<T>);

        public override int GetHashCode() => Rank.Id;
    }
}
