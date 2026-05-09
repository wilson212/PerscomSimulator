using Microsoft.Data.Sqlite;
using System.IO;

namespace Perscom.Database
{
    /// <summary>
    /// Represents the application's user database, which is used to store and manage UnitBlueprint data, Persona's, and Rank data
    /// </summary>
    /// <remarks>This class initializes the database connection using a static connection string builder
    /// configured for the application's SQLite database file, located in the "Data" directory under the application's
    /// root path. The database is configured to use Write-Ahead Logging (WAL) mode and enforce foreign key
    /// constraints.</remarks>
    public class AppDatabase : BaseDatabase
    {
        /// <summary>
        /// Contains the Connection string needed to create and connect
        /// to the application's SQLite database
        /// </summary>
        protected static SqliteConnectionStringBuilder Builder;

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
            Builder = new SqliteConnectionStringBuilder();
            Builder.DataSource = Path.Combine(source, "AppData.db");
            Builder.ForeignKeys = true;
            Builder["Journal Mode"] = "Wal";
        }

        /// <summary>
        /// Creates a new connection to the AppData.db database
        /// </summary>
        public AppDatabase() : base(Builder)
        {
            
        }
    }
}
