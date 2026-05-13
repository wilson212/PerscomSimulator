using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    public abstract class BaseDatabase : SQLiteContext
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
        /// Represents a collection of factions stored in the database.
        /// </summary>
        public DbSet<Faction> Factions { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionBlueprint"/> entites stored in the database
        /// </summary>
        public DbSet<PositionBlueprint> PositionBlueprints { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionCatagory"/> entites stored in the database
        /// </summary>
        public DbSet<PositionCatagory> PositionCatagories { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionBlueprintExperience"/> entites stored in the database
        /// </summary>
        public DbSet<PositionBlueprintExperience> PositionExperience { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SelectionFilter"/> entites stored in the database
        /// </summary>
        public DbSet<SelectionFilter> SelectionFilters { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SelectionGroup"/> entites stored in the database
        /// </summary>
        public DbSet<SelectionGroup> SelectionGroups { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.SelectionSorting"/> entites stored in the database
        /// </summary>
        public DbSet<SelectionSorting> SelectionSortings { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionOccupationRequirement"/> entites stored in the database
        /// </summary>
        public DbSet<PositionOccupationRequirement> BilletSpecialtyRequirements { get; set; }

        /// <summary>
        /// Gets a set of <see cref="CareerLengths"/> entites stored in the database
        /// </summary>
        public DbSet<CareerLength> CareerLengths { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Echelon"/> entites stored in the database
        /// </summary>
        public DbSet<Echelon> Echelons { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.Experience"/> entites stored in the database
        /// </summary>
        public DbSet<Experience> Experience { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="Occupation"/> entites stored in the database
        /// </summary>
        public DbSet<Occupation> Occupations { get; set; }
        
        public DbSet<Persona> Personas { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="PromotionBoard"/> entites stored in the database
        /// </summary>
        public DbSet<PromotionBoard> PromotionBoards { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="PromotionBoardWeight"/> entites stored in the database
        /// </summary>
        public DbSet<PromotionBoardWeight> PromotionBoardWeights { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Rank"/> entites stored in the database
        /// </summary>
        public DbSet<Rank> Ranks { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="RankClassification"/> entites stored in the database
        /// </summary>
        public DbSet<RankClassification> RankClassifications { get; set; }

        /// <summary>
        /// Gets a set of <see cref="UnitBlueprint"/> entites stored in the database
        /// </summary>
        public DbSet<UnitBlueprint> UnitBlueprints { get; set; }

        /// <summary>
        /// Gets a set of <see cref="UnitBlueprintAttachment"/> entites stored in the database
        /// </summary>
        public DbSet<UnitBlueprintAttachment> UnitTypeAttachments { get; set; }
        
        public DbSet<SelectionSoldierPool> SelectionSoldierPools { get; set; }

        public DbSet<CustomSelectionProceedure> CustomSelectionProceedures { get; set; }

        public DbSet<PositionPerformanceModel> PositionPerformanceModels { get; set; }

        public DbSet<PersonaTrait> PersonaTraits { get; set; }

        public DbSet<PersonaAttribute> PersonaAttributes { get; set; }

        public DbSet<TraitEffect> TraitEffects { get; set; }

        public DbSet<PersonalityTrait> PersonalityTraits { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance of BaseDatabase
        /// </summary>
        /// <param name="Builder"></param>
        public BaseDatabase(SqliteConnectionStringBuilder Builder) : base(Builder)
        {
            Debug.WriteLine($"Database: {Builder.DataSource}");
            // Open connection first
            base.Connect();
            
            Debug.WriteLine("Database initialized with WAL journal mode, NORMAL synchronous, 20MB cache, MEMORY temp store, 256MB mmap, and 4KB page size.");
            
            Execute("PRAGMA journal_mode = WAL;");      // Write-Ahead Logging - massive concurrency + write perf
            Execute("PRAGMA synchronous = NORMAL;");     // Safe with WAL, much faster than FULL
            Execute("PRAGMA cache_size = -20000;");      // 20MB page cache (default is only ~2MB)
            Execute("PRAGMA temp_store = MEMORY;");      // Temp tables in RAM
            Execute("PRAGMA mmap_io = 268435456;");      // 256MB memory-mapped I/O
            Execute("PRAGMA page_size = 4096;");         // Only effective on new DBs, but good default
            
            Debug.WriteLine("Database connection opened.");

            // Grab the current tables version
            if (DatabaseVersion == null)
            {
                try
                {
                    GetVersion();
                }
                catch (SqliteException e) when (e.Message.Contains("no such table"))
                {
                    Debug.WriteLine("Database is empty, creating tables...");
                    
                    // Rebuild database tables
                    BuildTables();

                    // Try 1 last time to get the Version
                    GetVersion();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get database version: {e.Message}");
                    throw new Exception($"Failed to get database version: {e.Message}");
                }
            }

            // Create Database Sets
            DbVersions = new DbSet<DbVersion>(this);
            Factions = new DbSet<Faction>(this);
            RankClassifications = new DbSet<RankClassification>(this);
            Echelons = new DbSet<Echelon>(this);
            Experience = new DbSet<Experience>(this);
            CareerLengths = new DbSet<CareerLength>(this);
            Occupations = new DbSet<Occupation>(this);
            PersonalityTraits = new DbSet<PersonalityTrait>(this);
            TraitEffects = new DbSet<TraitEffect>(this);
            Personas = new DbSet<Persona>(this);
            PersonaAttributes = new DbSet<PersonaAttribute>(this);
            PersonaTraits = new DbSet<PersonaTrait>(this);
            Ranks = new DbSet<Rank>(this);
            UnitBlueprints = new DbSet<UnitBlueprint>(this);
            UnitTypeAttachments = new DbSet<UnitBlueprintAttachment>(this);
            PositionCatagories = new DbSet<PositionCatagory>(this);
            PositionBlueprints = new DbSet<PositionBlueprint>(this);
            PositionExperience = new DbSet<PositionBlueprintExperience>(this);
            PositionPerformanceModels = new DbSet<PositionPerformanceModel>(this);
            BilletSpecialtyRequirements = new DbSet<PositionOccupationRequirement>(this);
            SelectionFilters = new DbSet<SelectionFilter>(this);
            SelectionGroups = new DbSet<SelectionGroup>(this);
            SelectionSortings = new DbSet<SelectionSorting>(this);
            CustomSelectionProceedures = new DbSet<CustomSelectionProceedure>(this);
            SelectionSoldierPools = new DbSet<SelectionSoldierPool>(this);
            PromotionBoards = new DbSet<PromotionBoard>(this);
            PromotionBoardWeights = new DbSet<PromotionBoardWeight>(this);

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
        /// Drops all tables from the database, and then creates new tables.
        /// Tables are ordered to respect foreign key constraints:
        /// - Drops go child-first (dependents before parents)
        /// - Creates go parent-first (parents before dependents)
        /// </summary>
        protected void BuildTables()
        {
            using (var tr = base.BeginTransaction())
            {
                // ============================================================
                // DROP TABLES — child-first order (dependents before parents)
                // ============================================================

                // Tier 5: Leaf tables (depend on Tier 4 or lower)
                this.DropTable<PromotionBoardWeight>();
                this.DropTable<SelectionSoldierPool>();
                this.DropTable<SelectionSorting>();
                this.DropTable<SelectionGroup>();
                this.DropTable<SelectionFilter>();
                this.DropTable<PositionOccupationRequirement>();
                this.DropTable<PositionPerformanceModel>();
                this.DropTable<PositionBlueprintExperience>();

                // Tier 4: Depend on Tier 3 or lower
                this.DropTable<PromotionBoard>();
                this.DropTable<PositionBlueprint>();
                this.DropTable<UnitBlueprintAttachment>();

                // Tier 3: Depend on Tier 2 or lower
                this.DropTable<PersonaTrait>();
                this.DropTable<PersonaAttribute>();
                this.DropTable<TraitEffect>();
                this.DropTable<Rank>();
                this.DropTable<UnitBlueprint>();

                // Tier 2: Depend on Tier 1 or lower
                this.DropTable<Persona>();
                this.DropTable<PersonalityTrait>();
                this.DropTable<CustomSelectionProceedure>();
                this.DropTable<PositionCatagory>();

                // Tier 1: No FK dependencies (root tables)
                this.DropTable<Faction>();
                this.DropTable<RankClassification>();
                this.DropTable<Echelon>();
                this.DropTable<Experience>();
                this.DropTable<CareerLength>();
                this.DropTable<Occupation>();
                this.DropTable<DbVersion>();

                // ============================================================
                // CREATE TABLES — parent-first order (parents before dependents)
                // ============================================================

                // Tier 1: Root tables (no FK dependencies)
                this.CreateTable<DbVersion>();
                this.CreateTable<Faction>();
                this.CreateTable<RankClassification>();
                this.CreateTable<Echelon>();
                this.CreateTable<Experience>();
                this.CreateTable<CareerLength>();
                this.CreateTable<Occupation>();

                // Tier 2: Depend on Tier 1
                this.CreateTable<PositionCatagory>();
                this.CreateTable<CustomSelectionProceedure>();
                this.CreateTable<PersonalityTrait>();
                this.CreateTable<Persona>();                        // -> CareerLength

                // Tier 3: Depend on Tier 2
                this.CreateTable<UnitBlueprint>();                  // -> Echelon
                this.CreateTable<Rank>();                           // -> RankClassification, self-ref
                this.CreateTable<TraitEffect>();                    // -> PersonalityTrait
                this.CreateTable<PersonaAttribute>();               // -> Persona
                this.CreateTable<PersonaTrait>();                   // -> Persona, PersonalityTrait

                // Tier 4: Depend on Tier 3
                this.CreateTable<UnitBlueprintAttachment>();        // -> UnitBlueprint, UnitBlueprint
                this.CreateTable<PositionBlueprint>();              // -> UnitBlueprint, PositionCatagory, Rank, Echelon, Occupation
                this.CreateTable<PromotionBoard>();                 // -> Rank, RankClassification, Occupation

                // Tier 5: Leaf tables (depend on Tier 4)
                this.CreateTable<PositionBlueprintExperience>();    // -> PositionBlueprint, Experience
                this.CreateTable<PositionPerformanceModel>();       // -> PositionBlueprint
                this.CreateTable<PositionOccupationRequirement>();  // -> PositionBlueprint, Occupation
                this.CreateTable<SelectionFilter>();                // -> PositionBlueprint
                this.CreateTable<SelectionGroup>();                 // -> PositionBlueprint
                this.CreateTable<SelectionSorting>();
                this.CreateTable<SelectionSoldierPool>();           // -> CustomSelectionProceedure, Rank
                this.CreateTable<PromotionBoardWeight>();           // -> PromotionBoard

                // Seed Echelons
                Echelons = new DbSet<Echelon>(this);

                Echelon e = CreateEntity<Echelon>();
                e.Name = "<<Inherit From Parent>>";
                e.HierarchyLevel = 99;
                Echelons.Add(e);

                var echelons = new string[] {
                    "Fire Team", "Squad", "Platoon", "Company", "Battalion", "Regiment", "Brigade",
                    "Division", "Corp", "Field Army", "Army Group", "Army Region", "Command",
                    "Branch", "Joint Command", "Faction", "Alliance", "Perscom"
                };

                int level = 1;
                foreach (string name in echelons)
                {
                    Echelon ec = CreateEntity<Echelon>();
                    ec.Name = name;
                    ec.HierarchyLevel = level++;
                    Echelons.Add(ec);
                }

                // Seed Billet Categories
                PositionCatagories = new DbSet<PositionCatagory>(this);
                var catagories = new String[] {
                    "General", "Special Staff Group", "S6 Staff", "S5 Staff", "S4 Staff",
                    "S3 Staff", "S2 Staff", "S1 Staff", "Personal Staff Group",
                    "Chief of Staff", "Leadership", "Command Group"
                };

                level = 1;
                foreach (string name in catagories)
                {
                    var cat = new PositionCatagory();
                    cat.Name = name;
                    cat.ZIndex = level++;
                    PositionCatagories.Add(cat);
                }

                // Create version record
                DbVersions = new DbSet<DbVersion>(this);
                DbVersion version = CreateEntity<DbVersion>();
                version.Version = CurrentVersion;
                version.AppliedOn = DateTime.Now;
                DbVersions.Add(version);

                tr.Commit();
            }
        }

        /// <summary>
        /// Creates a live backup of a SQLite database.
        /// </summary>
        /// <param name="destinationDbPath">The full path for the new backup file.</param>
        public void CreateBackup(string destinationDbPath)
        {
            // Create a connection to the empty destination backup file.
            var destinationConnectionString = $"Data Source={destinationDbPath};";
            using (var destinationConnection = new SqliteConnection(destinationConnectionString))
            {
                destinationConnection.Open();

                // 3. Use the built-in BackupDatabase method.
                base.Connection.BackupDatabase(destinationConnection);
            }
        }
    }
}
