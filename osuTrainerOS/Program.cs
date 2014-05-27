using System;
using System.Threading;
using System.Windows.Forms;

namespace osuTrainerOS
{
    internal static class Program
    {
        private static readonly Mutex Mutex = new Mutex(false, "osuTrainerOS");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!Mutex.WaitOne(TimeSpan.FromSeconds(0), false))
            {
                MessageBox.Show(@"osu! Trainer (OsuStats) is already running!", "", MessageBoxButtons.OK);
                return;
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Forms.osuTrainerOS());
            }
            finally { Mutex.ReleaseMutex(); }
        }
    }
}