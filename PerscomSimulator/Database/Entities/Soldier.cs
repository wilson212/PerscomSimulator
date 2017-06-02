using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class Soldier
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the first name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// Gets or sets the the <see cref="DateTime"/> the soldier was created in
        /// the <see cref="Simulator"/>
        /// </summary>
        [Column, Required]
        public DateTime ServiceEntryDate { get; set; }

        /// <summary>
        /// Gets or sets the assigned end date for this soldier
        /// </summary>
        [Column, Required]
        public DateTime ExitServiceDate { get; set; }

        /// <summary>
        /// Gets the last promotion date for this soldier
        /// </summary>
        [Column, Required]
        public DateTime LastPromotionDate { get; set; }

        /// <summary>
        /// Gets the probability value of this soldier
        /// </summary>
        [Column, Required]
        public int OneInThousandProbability { get; set; }

        /// <summary>
        /// Gets the minimum time to live for this soldier (months)
        /// </summary>
        [Column, Required]
        public int MinTimeToLive { get; set; }

        /// <summary>
        /// Gets the minimum time to live for this soldier (months)
        /// </summary>
        [Column, Required]
        public int MaxTimeToLive { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Rank"/> that 
        /// this position will hold.
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

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="PastAssignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<PastAssignment> Assignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Promotion"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Promotion> Promotions { get; set; }

        #endregion
    }
}
