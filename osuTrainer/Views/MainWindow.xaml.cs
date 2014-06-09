using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using osuTrainer.Properties;
using osuTrainer.ViewModels;

namespace osuTrainer.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Version newestVersion;

        public MainWindow()
        {
            InitializeComponent();
            LoadPreviousSettings();
            CheckUpdates();
        }

        private void LoadPreviousSettings()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }

        private async void CheckUpdates()
        {
            newestVersion = await Updater.Check();
            if (newestVersion <= Assembly.GetExecutingAssembly().GetName().Version) return;
            hyperLinkText.Text = "Update to " + newestVersion + " available.";
            UpdateUri.Click += UpdateUri_Click;
        }

        private void UpdateUri_Click(object sender, RoutedEventArgs e)
        {
            var updater = new Process
            {
                StartInfo =
                {
                    FileName = "Updater.exe",
                    Arguments =
                        @"osuTrainer.exe ""https://github.com/condone/osuTrainer/releases/download/v" + newestVersion +
                        @"/osuTrainer.zip"""
                }
            };
            updater.Start();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = Settings.Default.ApiKey;
            GameModeCb.ItemsSource = Enum.GetValues(typeof(GlobalVars.GameMode)).Cast<int>();
            ScoreSourceCb.Items.Add("osu! API");
            ScoreSourceCb.Items.Add("osustats API");
            ScoreSourceCb.SelectedIndex = Settings.Default.DataSource;
            switch (Settings.Default.DataSource)
            {
                case 1:
                    DataContext = new OsuStatsViewModel();
                    break;
                default:
                    DataContext = new OsuApiViewModel();
                    break;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Username = UsernameTb.Text;
            Settings.Default.ApiKey = PasswordBox.Password;
            Settings.Default.MinPp = PpSlider.Value;
            Settings.Default.GameMode = GameModeCb.SelectedIndex;
            Settings.Default.IsDoubletimeCbChecked = DoubletimeCb.IsChecked ?? false;
            Settings.Default.IsHardrockCbChecked = HardrockCb.IsChecked ?? false;
            Settings.Default.IsHiddenCbChecked = HiddenCb.IsChecked ?? false;
            Settings.Default.IsFlashlightCbChecked = FlashlightCb.IsChecked ?? false;
            Settings.Default.IsFcOnlyCbChecked = FcOnlyCb.IsChecked ?? false;
            Settings.Default.IsExclusiveCbChecked = ExclusiveCb.IsChecked ?? false;
            Settings.Default.Save();
            if (ScoreSourceCb.SelectedIndex != Settings.Default.DataSource)
            {
                switch (ScoreSourceCb.SelectedIndex)
                {
                    case 1:
                        DataContext = new OsuStatsViewModel();
                        break;
                    default:
                        DataContext = new OsuApiViewModel();
                        break;
                }
            }
            Settings.Default.DataSource = ScoreSourceCb.SelectedIndex;
            Settings.Default.Save();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ApiLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}