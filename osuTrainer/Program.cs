using System;
using System.Threading;
using System.Windows.Forms;

namespace osuTrainer
{
    internal static class Program
    {
        static readonly Mutex Mutex = new Mutex(false, "osuTrainer");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!Mutex.WaitOne(TimeSpan.FromSeconds(0), false))
            {
                MessageBox.Show(@"osu! Trainer is already running!", "", MessageBoxButtons.OK);
                return;
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Forms.OsuTrainer());
            }
            finally { Mutex.ReleaseMutex(); }
        }
    }
}