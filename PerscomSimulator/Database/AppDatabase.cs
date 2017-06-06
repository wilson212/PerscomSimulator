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

        /// <summary>
        /// Gets a set of <see cref="Billet"/> entites stored in the database
        /// </summary>
        public DbSet<Billet> Billets { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletCatagory"/> entites stored in the database
        /// </summary>
        public DbSet<BilletCatagory> BilletCatagories { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletRequirement"/> entites stored in the database
        /// </summary>
        public DbSet<BilletRequirement> BilletRequirements { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletSpecialty"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSpecialty> BilletSpecialties { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Echelon"/> entites stored in the database
        /// </summary>
        public DbSet<Echelon> Echelons { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Rank"/> entites stored in the database
        /// </summary>
        public DbSet<Rank> Ranks { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SoldierSpawnRate"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierSpawnRate> SoldierSpawnRates { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Specialty"/> entites stored in the database
        /// </summary>
        public DbSet<Specialty> Specialties { get; set; }

        /// <summary>
        /// Gets a set of <see cref="UnitTemplate"/> entites stored in the database
        /// </summary>
        public DbSet<UnitTemplate> UnitTemplates { get; set; }

        /// <summary>
        /// Gets a set of <see cref="UnitTemplateAttachment"/> entites stored in the database
        /// </summary>
        public DbSet<UnitTemplateAttachment> UnitTypeAttachments { get; set; }

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

        /// <summary>
        /// Creates a new connection to the AppData.db database
        /// </summary>
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
            Billets = new DbSet<Billet>(this);
            BilletCatagories = new DbSet<BilletCatagory>(this);
            BilletRequirements = new DbSet<BilletRequirement>(this);
            BilletSpecialties = new DbSet<BilletSpecialty>(this);
            Echelons = new DbSet<Echelon>(this);
            Ranks = new DbSet<Rank>(this);
            SoldierSpawnRates = new DbSet<SoldierSpawnRate>(this);
            Specialties = new DbSet<Specialty>(this);
            UnitTemplates = new DbSet<UnitTemplate>(this);
            UnitTypeAttachments = new DbSet<UnitTemplateAttachment>(this);

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
                CodeFirstSQLite.DropTable<SoldierSpawnRate>(this);
                CodeFirstSQLite.DropTable<BilletRequirement>(this);
                CodeFirstSQLite.DropTable<BilletSpecialty>(this);
                CodeFirstSQLite.DropTable<Billet>(this);
                CodeFirstSQLite.DropTable<BilletCatagory>(this);
                CodeFirstSQLite.DropTable<Specialty>(this);
                CodeFirstSQLite.DropTable<UnitTemplateAttachment>(this);
                CodeFirstSQLite.DropTable<UnitTemplate>(this);
                CodeFirstSQLite.DropTable<Echelon>(this);
                CodeFirstSQLite.DropTable<Rank>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<Rank>(this);
                CodeFirstSQLite.CreateTable<Echelon>(this);
                CodeFirstSQLite.CreateTable<UnitTemplate>(this);
                CodeFirstSQLite.CreateTable<UnitTemplateAttachment>(this);
                CodeFirstSQLite.CreateTable<Specialty>(this);
                CodeFirstSQLite.CreateTable<BilletCatagory>(this);
                CodeFirstSQLite.CreateTable<Billet>(this);
                CodeFirstSQLite.CreateTable<BilletSpecialty>(this);
                CodeFirstSQLite.CreateTable<BilletRequirement>(this);
                CodeFirstSQLite.CreateTable<SoldierSpawnRate>(this);

                // Add Echelons
                Echelons = new DbSet<Echelon>(this);
                Echelons.Add(new Echelon() { Name = "<<Inherit From Parent>>", HierarchyLevel = 99 });
                var echelons = new String[] {
                    "Fire Team", "Squad", "Platoon", "Company", "Battalion", "Regiment", "Brigade",
                    "Division", "Corp", "Field Army", "Army Group", "Army Region", "Command"
                };

                int level = 1;
                foreach (string name in echelons)
                {
                    Echelon e = new Echelon();
                    e.Name = name;
                    e.HierarchyLevel = level++;
                    Echelons.Add(e);
                }

                // Add Billet Catagories
                BilletCatagories = new DbSet<BilletCatagory>(this);
                var catagories = new String[] {
                    "General", "Special Staff Group", "S6 Staff", "S5 Staff", "S4 Staff",
                    "S3 Staff", "S2 Staff", "S1 Staff", "Personal Staff Group",
                    "Chief of Staff", "Leadership", "Command Group"
                };

                level = 1;
                foreach (string name in catagories)
                {
                    var cat = new BilletCatagory();
                    cat.Name = name;
                    cat.ZIndex = level++;
                    BilletCatagories.Add(cat);
                }

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
