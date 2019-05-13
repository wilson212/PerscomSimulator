using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Data.SQLite;
using System.Linq;

namespace Perscom.Database
{
    public abstract class BaseDatabase : CrossLite.SQLiteContext
    {
        /// <summary>
        /// Gets the latest database version
        /// </summary>
        public static Version CurrentVersion { get; protected set; } = new Version(2, 0);

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
        /// Gets a set of <see cref="BilletCareer"/> entites stored in the database
        /// </summary>
        public DbSet<BilletCareer> BilletCareers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletCatagory"/> entites stored in the database
        /// </summary>
        public DbSet<BilletCatagory> BilletCatagories { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.BilletExperience"/> entites stored in the database
        /// </summary>
        public DbSet<BilletExperience> BilletExperience { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletOrderedProcedure"/> entites stored in the database
        /// </summary>
        public DbSet<BilletOrderedProcedure> BilletOrderedProcedures { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletCustomProcedure"/> entites stored in the database
        /// </summary>
        public DbSet<BilletRandomizedProcedure> BilletRandomProcedures { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.BilletSelectionFilter"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSelectionFilter> BilletSelectionFilters { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.BilletSelectionGroup"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSelectionGroup> BilletSelectionGroups { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.BilletSelectionSorting"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSelectionSorting> BilletSelectionSorting { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletSpecialtyRequirement"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSpecialtyRequirement> BilletSpecialtyRequirements { get; set; }

        /// <summary>
        /// Gets a set of <see cref="BilletSpecialty"/> entites stored in the database
        /// </summary>
        public DbSet<BilletSpecialty> BilletSpecialties { get; set; }

        /// <summary>
        /// Gets a set of <see cref="CareerGenerator"/> entites stored in the database
        /// </summary>
        public DbSet<CareerGenerator> CareerGenerators { get; set; }

        /// <summary>
        /// Gets a set of <see cref="CareerLengthRange"/> entites stored in the database
        /// </summary>
        public DbSet<CareerLengthRange> CareerLengthRange { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Echelon"/> entites stored in the database
        /// </summary>
        public DbSet<Echelon> Echelons { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.Experience"/> entites stored in the database
        /// </summary>
        public DbSet<Experience> Experience { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Rank"/> entites stored in the database
        /// </summary>
        public DbSet<Rank> Ranks { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedProcedure"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedProcedure> OrderedProcedures { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPoolCareer"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedProcedureCareer> OrderedProcedureCareers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPool"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPool> OrderedPools { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPoolCareer"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPoolCareer> OrderedPoolCareers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPoolFilter"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPoolFilter> OrderedPoolFilters { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPoolGroup"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPoolGroup> OrderedPoolGroups { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.OrderedPoolSorting"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPoolSorting> OrderedPoolSorting { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OrderedPoolSpecialty"/> entites stored in the database
        /// </summary>
        public DbSet<OrderedPoolSpecialty> OrderedPoolSpecialties { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedProcedure"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedProcedure> RandomizedProcedures { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedProcedureCareer"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedProcedureCareer> RandomizedProcedureCareers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedPool"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedPool> RandomizedPools { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedPoolCareer"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedPoolCareer> RandomizedPoolCareers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedPoolFilter"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedPoolFilter> RandomizedPoolFilters { get; set; }

        /// <summary>
        /// Gets a set of <see cref="RandomizedPool"/> entites stored in the database
        /// </summary>
        public DbSet<RandomizedPoolSorting> RandomizedPoolSorting { get; set; }

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
            BilletCareers = new DbSet<BilletCareer>(this);
            BilletCatagories = new DbSet<BilletCatagory>(this);
            BilletExperience = new DbSet<BilletExperience>(this);
            BilletRandomProcedures = new DbSet<BilletRandomizedProcedure>(this);
            BilletOrderedProcedures = new DbSet<BilletOrderedProcedure>(this);
            BilletSelectionFilters = new DbSet<BilletSelectionFilter>(this);
            BilletSelectionGroups = new DbSet<BilletSelectionGroup>(this);
            BilletSelectionSorting = new DbSet<BilletSelectionSorting>(this);
            BilletSpecialtyRequirements = new DbSet<BilletSpecialtyRequirement>(this);
            BilletSpecialties = new DbSet<BilletSpecialty>(this);
            CareerGenerators = new DbSet<CareerGenerator>(this);
            CareerLengthRange = new DbSet<CareerLengthRange>(this);
            Echelons = new DbSet<Echelon>(this);
            Experience = new DbSet<Experience>(this);
            OrderedProcedures = new DbSet<OrderedProcedure>(this);
            OrderedProcedureCareers = new DbSet<OrderedProcedureCareer>(this);
            OrderedPools = new DbSet<OrderedPool>(this);
            OrderedPoolCareers = new DbSet<OrderedPoolCareer>(this);
            OrderedPoolFilters = new DbSet<OrderedPoolFilter>(this);
            OrderedPoolGroups = new DbSet<OrderedPoolGroup>(this);
            OrderedPoolSorting = new DbSet<OrderedPoolSorting>(this);
            OrderedPoolSpecialties = new DbSet<OrderedPoolSpecialty>(this);
            RandomizedProcedures = new DbSet<RandomizedProcedure>(this);
            RandomizedPools = new DbSet<RandomizedPool>(this);
            RandomizedPoolFilters = new DbSet<RandomizedPoolFilter>(this);
            RandomizedPoolSorting = new DbSet<RandomizedPoolSorting>(this);
            RandomizedPoolCareers = new DbSet<RandomizedPoolCareer>(this);
            RandomizedProcedureCareers = new DbSet<RandomizedProcedureCareer>(this);
            Ranks = new DbSet<Rank>(this);
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
                CodeFirstSQLite.DropTable<BilletSpecialtyRequirement>(this);
                CodeFirstSQLite.DropTable<BilletCustomProcedure>(this);
                CodeFirstSQLite.DropTable<BilletSpecialty>(this);
                CodeFirstSQLite.DropTable<BilletSelectionFilter>(this);
                CodeFirstSQLite.DropTable<BilletSelectionGroup>(this);
                CodeFirstSQLite.DropTable<BilletSelectionSorting>(this);
                CodeFirstSQLite.DropTable<BilletExperience>(this);
                CodeFirstSQLite.DropTable<BilletCareer>(this);
                CodeFirstSQLite.DropTable<Billet>(this);
                CodeFirstSQLite.DropTable<BilletCatagory>(this);
                CodeFirstSQLite.DropTable<Specialty>(this);
                CodeFirstSQLite.DropTable<RandomizedPoolCareer>(this);
                CodeFirstSQLite.DropTable<RandomizedProcedureCareer>(this);
                CodeFirstSQLite.DropTable<RandomizedPoolFilter>(this);
                CodeFirstSQLite.DropTable<RandomizedPoolSorting>(this);
                CodeFirstSQLite.DropTable<RandomizedPool>(this);
                CodeFirstSQLite.DropTable<RandomizedProcedure>(this);
                CodeFirstSQLite.DropTable<OrderedPoolSpecialty>(this);
                CodeFirstSQLite.DropTable<OrderedPoolCareer>(this);
                CodeFirstSQLite.DropTable<OrderedPoolGroup>(this);
                CodeFirstSQLite.DropTable<OrderedPoolFilter>(this);
                CodeFirstSQLite.DropTable<OrderedPoolSorting>(this);
                CodeFirstSQLite.DropTable<OrderedPool>(this);
                CodeFirstSQLite.DropTable<OrderedProcedureCareer>(this);
                CodeFirstSQLite.DropTable<OrderedProcedure>(this);
                CodeFirstSQLite.DropTable<CareerLengthRange>(this);
                CodeFirstSQLite.DropTable<CareerGenerator>(this);
                CodeFirstSQLite.DropTable<UnitTemplateAttachment>(this);
                CodeFirstSQLite.DropTable<UnitTemplate>(this);
                CodeFirstSQLite.DropTable<Experience>(this);
                CodeFirstSQLite.DropTable<Echelon>(this);
                CodeFirstSQLite.DropTable<Rank>(this);
                CodeFirstSQLite.DropTable<DbVersion>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<DbVersion>(this);
                CodeFirstSQLite.CreateTable<Rank>(this);
                CodeFirstSQLite.CreateTable<Echelon>(this);
                CodeFirstSQLite.CreateTable<Experience>(this);
                CodeFirstSQLite.CreateTable<UnitTemplate>(this);
                CodeFirstSQLite.CreateTable<UnitTemplateAttachment>(this);
                CodeFirstSQLite.CreateTable<CareerGenerator>(this);
                CodeFirstSQLite.CreateTable<CareerLengthRange>(this);

                CodeFirstSQLite.CreateTable<OrderedProcedure>(this);
                CodeFirstSQLite.CreateTable<OrderedProcedureCareer>(this);
                CodeFirstSQLite.CreateTable<OrderedPool>(this);
                CodeFirstSQLite.CreateTable<OrderedPoolFilter>(this);
                CodeFirstSQLite.CreateTable<OrderedPoolGroup>(this);
                CodeFirstSQLite.CreateTable<OrderedPoolSorting>(this);
                CodeFirstSQLite.CreateTable<OrderedPoolCareer>(this);
                CodeFirstSQLite.CreateTable<OrderedPoolSpecialty>(this);

                CodeFirstSQLite.CreateTable<RandomizedProcedure>(this);
                CodeFirstSQLite.CreateTable<RandomizedPool>(this);
                CodeFirstSQLite.CreateTable<RandomizedPoolFilter>(this);
                CodeFirstSQLite.CreateTable<RandomizedPoolSorting>(this);
                CodeFirstSQLite.CreateTable<RandomizedProcedureCareer>(this);
                CodeFirstSQLite.CreateTable<RandomizedPoolCareer>(this);

                CodeFirstSQLite.CreateTable<Specialty>(this);
                CodeFirstSQLite.CreateTable<BilletCatagory>(this);
                CodeFirstSQLite.CreateTable<Billet>(this);
                CodeFirstSQLite.CreateTable<BilletCareer>(this);
                CodeFirstSQLite.CreateTable<BilletExperience>(this);
                CodeFirstSQLite.CreateTable<BilletSelectionFilter>(this);
                CodeFirstSQLite.CreateTable<BilletSelectionGroup>(this);
                CodeFirstSQLite.CreateTable<BilletSelectionSorting>(this);
                CodeFirstSQLite.CreateTable<BilletSpecialty>(this);
                CodeFirstSQLite.CreateTable<BilletOrderedProcedure>(this);
                CodeFirstSQLite.CreateTable<BilletRandomizedProcedure>(this);
                CodeFirstSQLite.CreateTable<BilletSpecialtyRequirement>(this);

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
                RandomizedProcedures = new DbSet<RandomizedProcedure>(this);
                RandomizedProcedures.Add(new RandomizedProcedure()
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
