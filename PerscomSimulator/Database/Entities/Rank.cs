using CrossLite;
using CrossLite.CodeFirst;
using System;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a rank, and it's rules that <see cref="Soldier"/>'s will adhere to.
    /// </summary>
    [Table]
    public class Rank : EntityBase, IEquatable<Rank>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Rank ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="RankClassification.Id"/> this entity references
        /// </summary>
        [Column, Required]
        public virtual int RankClassificationId { get; set; }
        
        /// <summary>
        /// Gets or sets the string name of this Rank
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the string Abbreviation of this Rank
        /// </summary>
        [Column, Required, Unique]
        public virtual  string Abbreviation { get; set; }

        /// <summary>
        /// If true, the only way this rank can be achieved is through a special assignment.
        /// </summary>
        [Column, Required, Default(false)]
        public virtual bool IsPositional { get; set; }
        
        /// <summary>
        /// The identifier of the next rank in the hierarchy, used to establish a progression path. This should be
        /// null unless this <see cref="RankClassification.HasSplitRankLanes"/> is true.
        /// </summary>
        /// <remarks>
        /// THE TRACK ENFORCER:
        /// If this is populated, ONLY people currently holding this Previous Rank can apply for this Rank.
        /// e.g., ThisRankId = First Sergeant (E-8) => NextRankId = Sergeant Major (E-9)
        /// Master Gunnery Sergeant is completely blocked off from them.
        /// </remarks>
        [Column, Default(null)]
        public virtual int? NextRankId { get; set; } 

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
        public virtual int Precedence { get; set; }

        /// <summary>
        /// Gets or sets the image name of this Rank
        /// </summary>
        [Column, Default("")]
        public virtual string Image { get; set; }

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.RankClassification"/> that 
        /// this <see cref="Rank"/> references.
        /// </summary>
        [ForeignKey(nameof(RankClassificationId))]
        [References(nameof(Database.RankClassification.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual RankClassification Classification { get; set; }

        /// <summary>
        /// Represents the rank that follows the current rank in the hierarchy.
        /// This property establishes a foreign key relationship to the Rank table,
        /// enabling the chaining of ranks.
        /// </summary>
        [ForeignKey(nameof(NextRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank NextRank { get; set; }

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
        public virtual EntitySet<Soldier> Soldiers { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PositionBlueprint"/> entities that require this 
        /// <see cref="Rank"/>
        /// </summary>
        [InverseForeignKey(nameof(Database.PositionBlueprint.TargetRankId))]
        public virtual EntitySet<PositionBlueprint> PositionBlueprintsByRankId { get; set; }

        #endregion
        
        public RankType Type => Classification?.Type ?? default;
        
        public int PayGrade => Classification?.PayGrade ?? 0;
        
        /// <summary>
        /// Returns a composite sort value where RankType is the primary key
        /// and PayGrade is the secondary key. Officer > Warrant > Enlisted.
        /// </summary>
        private int SortValue
        {
            get
            {
                int typeOrder = Type switch
                {
                    RankType.Officer  => 2,
                    RankType.Warrant  => 1,
                    _ => 0
                };

                // Multiply by 100 to leave room for pay grades (max ~10-15)
                return (typeOrder * 100) + PayGrade;
            }
        }
        
        #region Operators

        public static bool operator ==(Rank left, Rank right)
        {
            if (left is null) 
                return right is null;
            
            return left.Equals(right);
        }

        public static bool operator !=(Rank left, Rank right)
        {
            return !(left == right);
        }

        public static bool operator <(Rank left, Rank right)
        {
            if (left is null || right is null) 
                return false;
            
            return left.SortValue < right.SortValue;
        }

        public static bool operator >(Rank left, Rank right)
        {
            if (left is null || right is null) 
                return false;
            
            return left.SortValue > right.SortValue;
        }

        public static bool operator <=(Rank left, Rank right)
        {
            if (left is null || right is null) 
                return left is null && right is null;
            
            return left.SortValue <= right.SortValue;
        }

        public static bool operator >=(Rank left, Rank right)
        {
            if (left is null || right is null) 
                return left is null && right is null;
            
            return left.SortValue >= right.SortValue;
        }
        
        #endregion
        
        public bool Equals(Rank other)
        {
            if (other is null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Rank);
        }
        
        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
