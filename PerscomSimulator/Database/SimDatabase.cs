using CrossLite;
using CrossLite.CodeFirst;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a specialized database for managing simulation-related data,  including entities such as assignments,
    /// positions, soldiers, and statistics.
    /// </summary>
    /// <remarks>The <see cref="SimDatabase"/> class provides access to various entity sets through
    /// properties of type <see cref="DbSet{T}"/>. These entity sets represent  collections of data stored in the
    /// database, such as assignments, positions,  and soldiers. The class also includes methods for creating and
    /// managing the  database schema, as well as static methods for creating or opening a simulation  database file.
    /// <para> This class extends <see cref="BaseDatabase"/> and is designed to work with  SQLite as the underlying
    /// database engine. </para></remarks>
    public class SimDatabase : BaseDatabase
    {
        #region Database Entity Sets

        /// <summary>
        /// Gets a set of <see cref="Assignment"/> entites stored in the database
        /// </summary>
        public DbSet<Assignment> Assignments { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionBlueprintStats"/> entites stored in the database
        /// </summary>
        public DbSet<PositionBlueprintStats> PositionBlueprintStats { get; set; }

        /// <summary>
        /// Gets a set of <see cref="IterationDate"/> entites stored in the database
        /// </summary>
        public DbSet<IterationDate> IterationDates { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PastAssignment"/> entites stored in the database
        /// </summary>
        public DbSet<PastAssignment> PastAssignments { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Position"/> entites stored in the database
        /// </summary>
        public DbSet<Position> Positions { get; set; }

        /// <summary>
        /// Gets a set of <see cref="PositionStatistics"/> entites stored in the database
        /// </summary>
        public DbSet<PositionStatistics> PositionStatistics { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Promotion"/> entites stored in the database
        /// </summary>
        public DbSet<Promotion> Promotions { get; set; }
        
        public DbSet<PromotionBoardResult> PromotionBoardResults { get; set; }
        
        public DbSet<PromotableCandidate> PromotableCandidates { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="Database.RankGradeStatistics"/> entites stored in the database
        /// </summary>
        public DbSet<RankGradeStatistics> RankGradeStatistics { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Soldier"/> entites stored in the database
        /// </summary>
        public DbSet<Soldier> Soldiers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="OccupationAssignment"/> entites stored in the database
        /// </summary>
        public DbSet<OccupationAssignment> SpecialtyAssignments { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.SoldierExperience"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierExperience> SoldierExperience { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="Database.SoldierAttribute"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierAttribute> SoldierAttributes { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SoldierMonthlyRecord"/> entities stored in the database.
        /// </summary>
        public DbSet<SoldierMonthlyRecord> SoldierMonthlyRecords { get; set; }
        
        /// <summary>
        /// Gets a set of <see cref="Database.SoldierTraitAttachment"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierTraitAttachment> SoldierTraitAttachments { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.SpecialtyGradeStatistics"/> entites stored in the database
        /// </summary>
        public DbSet<SpecialtyGradeStatistics> SpecialtyGradeStatistics { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Unit"/> entites stored in the database
        /// </summary>
        public DbSet<Unit> Units { get; set; }

        #endregion

        /// <summary>
        /// Represents a simulation-specific database that extends the base database functionality.
        /// Provides mechanisms to manage and interact with simulation-related data sets.
        /// </summary>
        protected SimDatabase(SqliteConnectionStringBuilder builder) : base(builder)
        {
            // Create Database Sets
            IterationDates = new DbSet<IterationDate>(this);
            Soldiers = new DbSet<Soldier>(this);
            SoldierAttributes = new DbSet<SoldierAttribute>(this);
            SoldierMonthlyRecords = new DbSet<SoldierMonthlyRecord>(this);
            SoldierTraitAttachments = new DbSet<SoldierTraitAttachment>(this);
            SoldierExperience = new DbSet<SoldierExperience>(this);
            Units = new DbSet<Unit>(this);
            Positions = new DbSet<Position>(this);
            Assignments = new DbSet<Assignment>(this);
            PastAssignments = new DbSet<PastAssignment>(this);
            SpecialtyAssignments = new DbSet<OccupationAssignment>(this);
            Promotions = new DbSet<Promotion>(this);
            PromotableCandidates = new DbSet<PromotableCandidate>(this);
            PromotionBoardResults = new DbSet<PromotionBoardResult>(this);
            PositionBlueprintStats = new DbSet<PositionBlueprintStats>(this);
            PositionStatistics = new DbSet<PositionStatistics>(this);
            RankGradeStatistics = new DbSet<RankGradeStatistics>(this);
            SpecialtyGradeStatistics = new DbSet<SpecialtyGradeStatistics>(this);
        }

        /// <summary>
        /// Drops and recreates all simulation-only tables.
        /// Tables are ordered to respect foreign key constraints:
        /// - Drops go child-first (dependents before parents)
        /// - Creates go parent-first (parents before dependents)
        /// </summary>
        private void CreateTables()
        {
            using (var tr = base.BeginTransaction())
            {
                // ============================================================
                // DROP TABLES — child-first order (dependents before parents)
                // ============================================================

                // Tier 4: Statistics (leaf tables, depend on Tier 2/3)
                this.DropTable<PositionStatistics>();     // -> Position
                this.DropTable<PositionBlueprintStats>();       // -> PositionBlueprint
                this.DropTable<SpecialtyGradeStatistics>(); // -> Occupation
                this.DropTable<RankGradeStatistics>();    // -> UnitBlueprint

                // Tier 3: Assignment/promotion records (depend on Soldier, Position, etc.)
                this.DropTable<Assignment>();             // -> Soldier, Position, IterationDate, Rank
                this.DropTable<PastAssignment>();         // -> Soldier, Position, IterationDate, Rank
                this.DropTable<OccupationAssignment>();   // -> Soldier, Occupation, IterationDate
                this.DropTable<Promotion>();              // -> Soldier, Rank, IterationDate
                this.DropTable<PromotableCandidate>();    // -> Soldier, PromotionBoard, IterationDate
                this.DropTable<PromotionBoardResult>();   // -> Soldier, PromotionBoard, IterationDate

                // Tier 2: Soldier-dependent tables & Position
                this.DropTable<SoldierAttribute>();       // -> Soldier
                this.DropTable<SoldierMonthlyRecord>();   // -> Soldier
                this.DropTable<SoldierTraitAttachment>(); // -> Soldier, PersonalityTrait, IterationDate
                this.DropTable<SoldierExperience>();      // -> Soldier, Experience
                this.DropTable<Position>();               // -> PositionBlueprint, Unit

                // Tier 1: Core simulation entities
                this.DropTable<Soldier>();                // -> Persona, Rank, Occupation, IterationDate, CareerLength
                this.DropTable<Unit>();                   // -> UnitBlueprint

                // Tier 0: No sim-entity dependencies
                this.DropTable<IterationDate>();

                // ============================================================
                // CREATE TABLES — parent-first order (parents before dependents)
                // ============================================================

                // Tier 0: Root sim tables
                this.CreateTable<IterationDate>();

                // Tier 1: Core simulation entities (reference base tables + IterationDate)
                this.CreateTable<Unit>();                   // -> UnitBlueprint (base)
                this.CreateTable<Soldier>();                // -> Persona, Rank, Occupation (base), IterationDate, CareerLength (base)

                // Tier 2: Depend on Soldier and/or Unit
                this.CreateTable<Position>();               // -> PositionBlueprint (base), Unit
                this.CreateTable<SoldierExperience>();      // -> Soldier, Experience (base)
                this.CreateTable<SoldierAttribute>();       // -> Soldier
                this.CreateTable<SoldierMonthlyRecord>();   // -> Soldier
                this.CreateTable<SoldierTraitAttachment>(); // -> Soldier, PersonalityTrait (base), IterationDate

                // Tier 3: Depend on Soldier + Position/IterationDate/Rank
                this.CreateTable<Assignment>();             // -> Soldier, Position, IterationDate, Rank (base)
                this.CreateTable<PastAssignment>();         // -> Soldier, Position, IterationDate, Rank (base)
                this.CreateTable<OccupationAssignment>();   // -> Soldier, Occupation (base), IterationDate
                this.CreateTable<Promotion>();              // -> Soldier, Rank (base), IterationDate
                this.CreateTable<PromotableCandidate>();    // -> Soldier, PromotionBoard (base), IterationDate
                this.CreateTable<PromotionBoardResult>();   // -> Soldier, PromotionBoard (base), IterationDate

                // Tier 4: Statistics (leaf tables)
                this.CreateTable<RankGradeStatistics>();    // -> UnitBlueprint (base)
                this.CreateTable<SpecialtyGradeStatistics>(); // -> Occupation (base)
                this.CreateTable<PositionBlueprintStats>();       // -> PositionBlueprint (base)
                this.CreateTable<PositionStatistics>();     // -> Position

                tr.Commit();
            }
        }

        /// <summary>
        /// Static Constructor
        /// </summary>
        static SimDatabase()
        {
            // Define folder path to the AppData.db
            string source = Path.Combine(Program.RootPath, "Data");
            if (!Directory.Exists(source))
                Directory.CreateDirectory(source);
        }

        public static SimDatabase CreateNew(AppDatabase db, string fileName)
        {
            // Delete existing database
            var source = Path.Combine(Program.RootPath, "Data", fileName);
            if (File.Exists(source))
            {
                File.Delete(source);
            }

            // Create connection builder
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = source,
                ForeignKeys = true,
                Pooling = true
            };

            // Copy contents from the AppData.db to this database
            db.CreateBackup(builder.ConnectionString);

            // Create Simulation Related Database Sets
            SimDatabase me = new SimDatabase(builder);
            me.CreateTables();

            // Return fresh database
            return me;
        }

        public static SimDatabase Open(string fileName)
        {
            // Create connection builder
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Program.RootPath, "Data", fileName),
                ForeignKeys = true
            };

            // Copy contents from the AppData.db to this database
            return new SimDatabase(builder);
        }
    }
}
