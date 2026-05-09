using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class DbVersion : EntityBase
    {
        /// <summary>
        /// Gets the IsUnique update ID in the table
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public virtual int UpdateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("Version"), Required, Unique]
        protected virtual string VersionString { get; set; }

        /// <summary>
        /// Gets or Sets the Version value for this update entry
        /// </summary>
        public virtual Version Version
        {
            get { return Version.Parse(VersionString); }
            set { VersionString = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        [Column("AppliedOn"), Required]
        [Default("(strftime('%s', 'now'))", Quote = false)]
        protected virtual int AppliedOnEpoch { get; set; }

        public DateTimeOffset AppliedOn
        {
            get { return Epoch.FromEpoch(AppliedOnEpoch); }
            set { AppliedOnEpoch = (int)value.ToUnixTimeSeconds(); }
        }
    }
}
