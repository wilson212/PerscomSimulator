using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    [Table]
    [CompositeUnique(nameof(GeneratorId), nameof(RankId))]
    public class SoldierGeneratorSetting : ISpawnable, IEquatable<SoldierGeneratorSetting>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGeneratorSetting ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoldierGenerator.Id"/> that this entity references
        /// </summary>
        [Column, Required]
        public int GeneratorId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required]
        public int Probability { get; set; }

        /// <summary>
        /// Indicates whether the soldier being selected is to recieve
        /// a <see cref="CareerSpawnRate"/> for their new <see cref="Database.Rank"/>
        /// </summary>
        [Column, Required]
        public bool NewCareerLength { get; set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("GeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGenerator> FK_Generator { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.SoldierGenerator"/> that 
        /// this entity references.
        /// </summary>
        public SoldierGenerator Generator
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                GeneratorId = value.Id;
                FK_Generator?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Rank"/> that 
        /// this entity references.
        /// </summary>
        public Rank Rank
        {
            get
            {
                return FK_Rank?.Fetch();
            }
            set
            {
                RankId = value.Id;
                FK_Rank?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="SoldierGeneratorSetting"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <remarks>Used in the <see cref="SoldierGeneratorEditorForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierGeneratorSetting other)
        {
            return (RankId == other.RankId && GeneratorId == other.GeneratorId);
        }

        public bool Equals(SoldierGeneratorSetting other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SoldierGeneratorSetting);
        }

        public override int GetHashCode() => Id;
    }
}
