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
    public class AppDatabase : BaseDatabase
    {
        /// <summary>
        /// Contains the Connection string needed to create and connect
        /// to the application's SQLite database
        /// </summary>
        protected static SQLiteConnectionStringBuilder Builder;

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
            
        }
    }
}
