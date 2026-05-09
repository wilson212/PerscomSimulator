using System;

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

            // Run the main GUI
            System.Windows.Forms.Application.Run(new MainForm());
        }
    }
}