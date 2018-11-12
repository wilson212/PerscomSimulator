using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a rank, and it's rules that <see cref="Soldier"/>'s will adhere to.
    /// </summary>
    [Table]
    public class Rank : IEquatable<Rank>
    {
        #region Columns

        /// <summary>
        /// The Unique Rank ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the string name of this Rank
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the string Abbreviation of this Rank
        /// </summary>
        [Column, Required]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets the grade of this rank
        /// </summary>
        [Column, Required]
        public RankType Type { get; set; }

        /// <summary>
        /// Gets or sets the grade of this rank
        /// </summary>
        [Column, Required]
        public int Grade { get; set; }

        /// <summary>
        /// Indicates the order of priority of this Rank. Special Ranks
        /// should have a higher Precendence over entry Ranks of the same grade
        /// to achieve expected results
        /// </summary>
        /// <example>
        /// - Sergeant Major (entry level E-9) would be 1
        /// - Command Sergeant Major (Special Billet based assignment) would be 2
        /// - Command Sergeant Major of the Army (Special billet based assignment) would be 3
        /// </example>
        [Column, Required, Default(0)]
        public int Precedence { get; set; }

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this grade before 
        /// retiring. If the minimum amount is less than the remaining time to live for
        /// the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Default(0)]
        public int MinTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this grade before being
        /// forcefully retired. Retirement date is otherwise un-modified.
        /// </summary>
        [Column, Default(0)]
        public int MaxTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the time in grade (months) to be promotable to the next grade.
        /// </summary>
        [Column, Required, Default(12)]
        public int PromotableAt { get; set; } = 12;

        /// <summary>
        /// Indicates whether the soldiers hodling this Rank will be
        /// promoted to the next rank when they reach the promotableAt value.
        /// </summary>
        [Column, Required, Default(0)]
        public bool AutoPromote { get; set; } = false;

        /// <summary>
        /// Gets or sets the image name of this Rank
        /// </summary>
        [Column, Default("")]
        public string Image { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Soldier"/> entities that hold this 
        /// <see cref="Rank"/>, including retirees. Use with caution as
        /// the result set can get very big.
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Soldier> Soldiers { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Billet"/> entities that require this 
        /// <see cref="Rank"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Billet> Billets { get; set; }

        #endregion

        public bool Equals(Rank other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Rank);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
