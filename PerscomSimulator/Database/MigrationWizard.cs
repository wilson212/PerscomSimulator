using System;
using System.Collections.Generic;
using System.IO;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// This class is used to migrate changes to the AppData.db database
    /// </summary>
    internal class MigrationWizard
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
            // Check if we need to migrate
            if (BaseDatabase.CurrentVersion == BaseDatabase.DatabaseVersion) return;
            
            // Ensure directory exists
            var path = Path.Combine(Program.RootPath, "Data", "Backups");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Create backup
            File.Copy(
                Path.Combine(Program.RootPath, "data", "AppData.db"),
                Path.Combine(path, $"AppData_v{BaseDatabase.DatabaseVersion}_{Epoch.Now}.db")
            );
            
            // Begin a transaction
            var transaction = Database.BeginTransaction();

            try
            {
                // Perform updates until we are caught up!
                while (BaseDatabase.CurrentVersion != BaseDatabase.DatabaseVersion)
                {
                    switch (BaseDatabase.DatabaseVersion.ToString())
                    {
                        case "2.0":
                            Database.CreateTable<PositionBlueprintRank>();
                            Database.Execute("INSERT INTO DbVersion (`Version`, `AppliedOn`) VALUES ('2.1', datetime('now'))");
                            break;
                        case "2.1":
                            Database.CreateTable<PositionBlueprintOccupation>();
                            Database.Execute("INSERT INTO DbVersion (`Version`, `AppliedOn`) VALUES ('2.2', datetime('now'))");
                            break;
                        case "2.2":
                            Database.DropTable<UnitBlueprintAttachment>();
                            Database.CreateTable<UnitBlueprintAttachment>();
                            Database.Execute("INSERT INTO DbVersion (`Version`, `AppliedOn`) VALUES ('2.3', datetime('now'))");
                            break;
                        case "2.3":
                            Database.MigrateTable<PositionCatagory>();
                            Database.Execute("INSERT INTO DbVersion (`Version`, `AppliedOn`) VALUES ('2.4', datetime('now'))");
                            break;
                        default:
                            throw new Exception($"Unexpected database version: {BaseDatabase.DatabaseVersion}");
                    }

                    // Fetch version
                    Database.GetVersion();
                }
                
                // Commit the transaction
                transaction.Commit();

                // Always perform a vacuum to optimize the database
                Database.Execute("VACUUM;");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
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
            string path = Path.Combine(Program.RootPath, "Errors", fileName);
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
    }
}
