using System;
using Telerik.WinControls;

namespace Perscom
{
    static class Program
    {
        /// <summary>
        /// Gets the root directory path of the application at startup.
        /// This property provides the base path for accessing application-specific
        /// resources and directories, such as configuration files, image directories,
        /// and error logs. Useful for building paths relative to the startup location
        /// of the application.
        /// </summary>
        public static string RootPath { get; } = System.Windows.Forms.Application.StartupPath;

        /// <summary>
        /// Gets the name of the current application theme.
        /// </summary>
        public static string ThemeName { get; } = "FluentPerscomBlue";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup visual styles
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(true);

            // Set Exception Handler
            System.Windows.Forms.Application.ThreadException += ExceptionHandler.OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.OnUnhandledException;

            // 1. Load the theme package from the embedded resource
            // The string format is usually "ProjectNamespace.Folder.FileName.tssp"
            ThemeResolutionService.LoadPackageResource("Perscom.Resources.FluentPerscomBlue.tssp");

            // 2. Apply it globally to the entire application
            // Make sure the string matches the internal theme name defined inside the VSB
            //ThemeResolutionService.ApplicationThemeName = "FluentPerscomBlue";

            // Setup Rad Message Box
            RadMessageBox.SetThemeName(ThemeName);

            // Run the main GUI
            System.Windows.Forms.Application.Run(new MainForm());
        }
    }
}