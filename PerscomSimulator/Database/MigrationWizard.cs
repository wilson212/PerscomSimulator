using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// This class is used to migrate changes to the AppData.db database
    /// </summary>
    class MigrationWizard
    {
        protected BaseDatabase Database { get; set; }

        public MigrationWizard(BaseDatabase db)
        {
            Database = db;
        }

        /// <summary>
        /// Migrates the database tables to the latest version
        /// </summary>
        internal void MigrateTables()
        {
            if (AppDatabase.CurrentVersion != AppDatabase.DatabaseVersion)
            {
                // Create backup
                File.Copy(
                    Path.Combine(Program.RootPath, "data", "AppData.db"),
                    Path.Combine(Program.RootPath, "data", "backups",
                        $"AppData_v{AppDatabase.DatabaseVersion}_{Epoch.Now}.db"
                    )
                );

                // Perform updates until we are caught up!
                while (AppDatabase.CurrentVersion != AppDatabase.DatabaseVersion)
                {
                    switch (AppDatabase.DatabaseVersion.ToString())
                    {
                        case "":
                            break;
                        default:
                            throw new Exception($"Unexpected database version: {AppDatabase.DatabaseVersion}");
                    }

                    // Fetch version
                    Database.GetVersion();
                }

                // Always perform a vacuum to optimize the database
                Database.Execute("VACUUM;");
            }
        }

        /// <summary>
        /// Performs an integrity check on the database, and returns the
        /// number of issues found.
        /// </summary>
        /// <returns></returns>
        internal int PerformIntegrityCheck()
        {
            // Log any integrity errors in the database
            var results = Database.Query("PRAGMA integrity_check;").ToList();
            if (results.Count > 0 && results[0]["integrity_check"].ToString() != "ok")
            {
                LogErrors(results, "IntegrityErrors.log");
                return results.Count;
            }

            return 0;
        }

        /// <summary>
        /// Performs a VACUUM on the database
        /// </summary>
        /// <seealso cref="https://sqlite.org/lang_vacuum.html"/>
        internal void VacuumDatabase()
        {
            Database.Execute("VACUUM;");
        }

        /// <summary>
        /// Logs the results of a foreign_key_check or integrity_check
        /// </summary>
        /// <param name="results"></param>
        /// <param name="fileName"></param>
        private void LogErrors(List<Dictionary<string, object>> results, string fileName)
        {
            // Ensure our directory exists
            string directory = Path.Combine(Program.RootPath, "Errors");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create the log file
            string path = Path.Combine(Program.RootPath, "errors", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                int i = 1;
                foreach (var item in results)
                {
                    writer.WriteLine("Error #" + i++);
                    foreach (string key in item.Keys)
                    {
                        string value = item[key].ToString();
                        writer.WriteLine($"\t{key} = {value}");
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// This method is used to perform a mass-migration on a table in the database.
        /// Essentially, this method renames the table, creates a new table using the same
        /// name, and copies all the data from the old table to the new.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RecreateTable<T>() where T : class
        {
            // Get the in-memory table mapping
            TableMapping table = EntityCache.GetTableMap(typeof(T));

            // Rename table
            var newName = table.TableName + "_old";
            Database.Execute($"ALTER TABLE `{table.TableName}` RENAME TO `{newName}`");

            // Create new table
            Database.CreateTable<T>();

            // Select from old table, and import to the new table
            // NOTE: had to do this the slow way because LowRpmRange_EngineBrake kept
            // throwing a constraing failure (not null).
            var items = Database.Query<T>($"SELECT * FROM `{newName}`");
            var set = new DbSet<T>(Database);
            set.AddRange(items);

            // Drop old table
            Database.Execute($"DROP TABLE `{newName}`");
        }
    }
}
