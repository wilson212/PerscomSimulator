using System;
using System.Windows.Forms;

namespace Perscom
{
    static class Program
    {
        public static string RootPath { get; } = Application.StartupPath;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set Exception Handler
            Application.ThreadException += ExceptionHandler.OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler.OnUnhandledException;

            // Run the main GUI
            Application.Run(new MainForm());
        }
    }
}