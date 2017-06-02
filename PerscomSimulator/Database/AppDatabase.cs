using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using CrossLite;
using CrossLite.CodeFirst;
using System.IO;

namespace Perscom.Database
{
    public class AppDatabase : CrossLite.SQLiteContext
    {
        /// <summary>
        /// Contains the Connection string needed to create and connect
        /// to the application's SQLite database
        /// </summary>
        protected static SQLiteConnectionStringBuilder Builder;

        /// <summary>
        /// Gets the latest database version
        /// </summary>
        public static Version CurrentVersion { get; protected set; } = new Version(1, 0);

        /// <summary>
        /// Gets the current database tables version
        /// </summary>
        public static Version DatabaseVersion { get; protected set; }

        #region Database Entity Sets

        protected DbSet<DbVersion> DbVersions { get; set; }

        #endregion

        /// <summary>
        /// Static Constructor
        /// </summary>
        static AppDatabase()
        {
            // Define folder path to the AppData.db
            string source = Path.Combine(Program.RootPath, "Data");
            if (!Directory.Exists(source))
                Directory.CreateDirectory(source);

            // Create the connection builder
            Builder = new SQLiteConnectionStringBuilder();
            Builder.DataSource = Path.Combine(source, "AppData.db");
            Builder.ForeignKeys = true;
            Builder.JournalMode = SQLiteJournalModeEnum.Wal;
        }

        public AppDatabase() : base(Builder)
        {
            // Open connection first
            base.Connect();

            // Grab the current tables version
            if (DatabaseVersion == null)
            {
                try
                {
                    GetVersion();
                }
                catch (SQLiteException e) when (e.Message.Contains("no such table"))
                {
                    // Rebuild database tables
                    BuildTables();

                    // Try 1 last time to get the Version
                    GetVersion();
                }
            }

            // Create Database Sets
            DbVersions = new DbSet<DbVersion>(this);

            // Migrations
            MigrationWizard wizard = new MigrationWizard(this);
            wizard.MigrateTables();
        }

        internal void GetVersion()
        {
            // Grab version. Plain SQL query here for performance
            string query = "SELECT * FROM DbVersion ORDER BY UpdateId DESC LIMIT 1";
            DbVersion row = Query<DbVersion>(query).FirstOrDefault();

            // If row is null, then the table exists, but was truncated
            if (row == null)
                throw new Exception("DbVersion table is empty");

            // Set instance database version
            DatabaseVersion = row.Version;
        }

        /// <summary>
        /// Drops all tables from the database, and the creates new
        /// tables.
        /// </summary>
        protected void BuildTables()
        {
            // Wrap in a transaction
            using (SQLiteTransaction tr = base.BeginTransaction())
            {
                // Delete old table rementants
                CodeFirstSQLite.DropTable<Billet>(this);
                CodeFirstSQLite.DropTable<UnitTypeAttachment>(this);
                CodeFirstSQLite.DropTable<Unit>(this);
                CodeFirstSQLite.DropTable<UnitType>(this);
                CodeFirstSQLite.DropTable<Echelon>(this);
                CodeFirstSQLite.DropTable<Rank>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<Rank>(this);
                CodeFirstSQLite.CreateTable<Echelon>(this);
                CodeFirstSQLite.CreateTable<UnitType>(this);
                CodeFirstSQLite.CreateTable<Unit>(this);
                CodeFirstSQLite.CreateTable<UnitTypeAttachment>(this);
                CodeFirstSQLite.CreateTable<Billet>(this);

                // Create version record
                DbVersion version = new DbVersion();
                version.Version = CurrentVersion;
                version.AppliedOn = DateTime.Now;

                DbVersions = new DbSet<DbVersion>(this);
                DbVersions.Add(version);

                // Commit the transaction
                tr.Commit();
            }
        }
    }
}
