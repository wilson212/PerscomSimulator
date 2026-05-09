using System;
using Telerik.WinControls;

namespace Perscom
{
    static class Program
    {
        public static string RootPath { get; } = System.Windows.Forms.Application.StartupPath;

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
            RadMessageBox.SetThemeName("FluentPerscomBlue");

            // Run the main GUI
            System.Windows.Forms.Application.Run(new MainForm());
        }
    }
}