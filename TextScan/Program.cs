using System;
using System.Windows.Forms;

namespace TextScan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set DPI awareness for better rendering on high-DPI displays
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Form1 mainForm = new Form1();

            // If you want the app to always start minimized, uncomment the following line
            // if (args.Length == 0) args = new string[] { "/minimized" };

            Application.Run(mainForm);
        }

        // P/Invoke for DPI awareness
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}