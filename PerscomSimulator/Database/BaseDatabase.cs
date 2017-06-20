using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    public abstract class BaseDatabase : CrossLite.SQLiteContext
    {
        /// <summary>
        /// Gets the latest database version
        /// </summary>
        public static Version CurrentVersion { get; protected set; } = new Version(1, 3);

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
        /// Gets a set of <see cref="BilletSpawnSetting"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSpawnSetting> BilletSpawnSettings { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletSpecialty"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSpecialty> BilletSpecialties { get; set; }

        /// <summary>
        /// Gets a set of <see cref="CareerSpawnRate"/> entites stored in the database
        /// </summary>
        public DbSet<CareerSpawnRate> CareerSpawnRates { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Echelon"/> entites stored in the database
        /// </summary>
        public DbSet<Echelon> Echelons { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Rank"/> entites stored in the database
        /// </summary>
        public DbSet<Rank> Ranks { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SoldierGenerator"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierGenerator> SoldierGenerators { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SoldierGeneratorSetting"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierGeneratorSetting> SoldierGeneratorSettings { get; set; }

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
        /// Creates a new instance of BaseDatabase
        /// </summary>
        /// <param name="Builder"></param>
        public BaseDatabase(SQLiteConnectionStringBuilder Builder) : base(Builder)
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
            BilletSpawnSettings = new DbSet<BilletSpawnSetting>(this);
            BilletSpecialties = new DbSet<BilletSpecialty>(this);
            CareerSpawnRates = new DbSet<CareerSpawnRate>(this);
            Echelons = new DbSet<Echelon>(this);
            Ranks = new DbSet<Rank>(this);
            SoldierGenerators = new DbSet<SoldierGenerator>(this);
            SoldierGeneratorSettings = new DbSet<SoldierGeneratorSetting>(this);
            Specialties = new DbSet<Specialty>(this);
            UnitTemplates = new DbSet<UnitTemplate>(this);
            UnitTypeAttachments = new DbSet<UnitTemplateAttachment>(this);

            // Migrations
            MigrationWizard wizard = new MigrationWizard(this);
            wizard.MigrateTables();
        }

        /// <summary>
        /// Fetches the latest database update version from the database
        /// </summary>
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
                CodeFirstSQLite.DropTable<CareerSpawnRate>(this);
                CodeFirstSQLite.DropTable<BilletRequirement>(this);
                CodeFirstSQLite.DropTable<BilletSpawnSetting>(this);
                CodeFirstSQLite.DropTable<BilletSpecialty>(this);
                CodeFirstSQLite.DropTable<Billet>(this);
                CodeFirstSQLite.DropTable<BilletCatagory>(this);
                CodeFirstSQLite.DropTable<Specialty>(this);
                CodeFirstSQLite.DropTable<SoldierGeneratorSetting>(this);
                CodeFirstSQLite.DropTable<SoldierGenerator>(this);
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
                CodeFirstSQLite.CreateTable<SoldierGenerator>(this);
                CodeFirstSQLite.CreateTable<SoldierGeneratorSetting>(this);
                CodeFirstSQLite.CreateTable<Specialty>(this);
                CodeFirstSQLite.CreateTable<BilletCatagory>(this);
                CodeFirstSQLite.CreateTable<Billet>(this);
                CodeFirstSQLite.CreateTable<BilletSpecialty>(this);
                CodeFirstSQLite.CreateTable<BilletSpawnSetting>(this);
                CodeFirstSQLite.CreateTable<BilletRequirement>(this);
                CodeFirstSQLite.CreateTable<CareerSpawnRate>(this);

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

                // Create default Soldier Generator
                SoldierGenerators = new DbSet<SoldierGenerator>(this);
                SoldierGenerators.Add(new SoldierGenerator()
                {
                    Name = "Default",
                    CreatesNewSoldiers = true,
                    NewSoldierProbability = 100
                });

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
