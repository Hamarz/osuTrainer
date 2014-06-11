using System.Threading;
using System.Windows;

namespace osuTrainer
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _instanceMutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            _instanceMutex = new Mutex(true, @"osuTrainer", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("osu! Trainer is already running.");
                _instanceMutex = null;
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_instanceMutex != null)
                _instanceMutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}