using System.Data.SQLite;
using System.IO;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    public class SimDatabase : BaseDatabase
    {
        #region Database Entity Sets

        /// <summary>
        /// Gets a set of <see cref="Assignment"/> entites stored in the database
        /// </summary>
        public DbSet<Assignment> Assignments { get; set; }

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
        /// Gets a set of <see cref="Promotion"/> entites stored in the database
        /// </summary>
        public DbSet<Promotion> Promotions { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.RankGradeStatistics"/> entites stored in the database
        /// </summary>
        public DbSet<RankGradeStatistics> RankGradeStatistics { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Soldier"/> entites stored in the database
        /// </summary>
        public DbSet<Soldier> Soldiers { get; set; }

        /// <summary>
        /// Gets a set of <see cref="SpecialtyAssignment"/> entites stored in the database
        /// </summary>
        public DbSet<SpecialtyAssignment> SpecialtyAssignments { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.SoldierExperience"/> entites stored in the database
        /// </summary>
        public DbSet<SoldierExperience> SoldierExperience { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Database.SpecialtyGradeStatistics"/> entites stored in the database
        /// </summary>
        public DbSet<SpecialtyGradeStatistics> SpecialtyGradeStatistics { get; set; }

        /// <summary>
        /// Gets a set of <see cref="Unit"/> entites stored in the database
        /// </summary>
        public DbSet<Unit> Units { get; set; }

        /// <summary>
        /// Gets a set of <see cref="UnitAttachment"/> entites stored in the database
        /// </summary>
        public DbSet<UnitAttachment> UnitAttachments { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected SimDatabase(SQLiteConnectionStringBuilder builder) : base(builder)
        {
            // Create Database Sets
            Assignments = new DbSet<Assignment>(this);
            IterationDates = new DbSet<IterationDate>(this);
            PastAssignments = new DbSet<PastAssignment>(this);
            Positions = new DbSet<Position>(this);
            Promotions = new DbSet<Promotion>(this);
            RankGradeStatistics = new DbSet<RankGradeStatistics>(this);
            Soldiers = new DbSet<Soldier>(this);
            SoldierExperience = new DbSet<SoldierExperience>(this);
            SpecialtyAssignments = new DbSet<SpecialtyAssignment>(this);
            SpecialtyGradeStatistics = new DbSet<SpecialtyGradeStatistics>(this);
            Units = new DbSet<Unit>(this);
            UnitAttachments = new DbSet<UnitAttachment>(this);
        }

        /// <summary>
        /// Drops all tables from the database, and the creates new
        /// tables.
        /// </summary>
        private void CreateTables()
        {
            // Wrap in a transaction
            using (SQLiteTransaction tr = base.BeginTransaction())
            {
                // Delete old table rementants
                CodeFirstSQLite.DropTable<SpecialtyGradeStatistics>(this);
                CodeFirstSQLite.DropTable<RankGradeStatistics>(this);
                CodeFirstSQLite.DropTable<SpecialtyAssignment>(this);
                CodeFirstSQLite.DropTable<Assignment>(this);
                CodeFirstSQLite.DropTable<PastAssignment>(this);
                CodeFirstSQLite.DropTable<Position>(this);
                CodeFirstSQLite.DropTable<UnitAttachment>(this);
                CodeFirstSQLite.DropTable<Unit>(this);
                CodeFirstSQLite.DropTable<Promotion>(this);
                CodeFirstSQLite.DropTable<SoldierExperience>(this);
                CodeFirstSQLite.DropTable<Soldier>(this);
                CodeFirstSQLite.DropTable<IterationDate>(this);

                // Create the needed database tables
                CodeFirstSQLite.CreateTable<IterationDate>(this);
                CodeFirstSQLite.CreateTable<Soldier>(this);
                CodeFirstSQLite.CreateTable<SoldierExperience>(this);
                CodeFirstSQLite.CreateTable<Promotion>(this);
                CodeFirstSQLite.CreateTable<Unit>(this);
                CodeFirstSQLite.CreateTable<UnitAttachment>(this);
                CodeFirstSQLite.CreateTable<Position>(this);
                CodeFirstSQLite.CreateTable<PastAssignment>(this);
                CodeFirstSQLite.CreateTable<Assignment>(this);
                CodeFirstSQLite.CreateTable<SpecialtyAssignment>(this);
                CodeFirstSQLite.CreateTable<RankGradeStatistics>(this);
                CodeFirstSQLite.CreateTable<SpecialtyGradeStatistics>(this);

                // Commit the transaction
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
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = source;
            builder.ForeignKeys = true;
            builder.Pooling = true;
            builder.JournalMode = SQLiteJournalModeEnum.Wal;

            // Copy contents from the AppData.db to this database
            SimDatabase me = new SimDatabase(builder);
            db.Connection.BackupDatabase(me.Connection, "main", "main", -1, null, 0);

            // Create Simulation Related Database Sets
            me.CreateTables();

            // Return fresh database
            return me;
        }

        public static SimDatabase Open(string fileName)
        {
            // Create connection builder
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.Combine(Program.RootPath, "Data", fileName);
            builder.ForeignKeys = true;
            builder.JournalMode = SQLiteJournalModeEnum.Wal;

            // Copy contents from the AppData.db to this database
            return new SimDatabase(builder);
        }
    }
}
