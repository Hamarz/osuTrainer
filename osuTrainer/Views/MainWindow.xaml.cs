using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro;
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
        }

        private void SettingsFlyoutOnIsOpenChanged(object sender, EventArgs eventArgs)
        {
            Settings.Default.Save();
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
            try
            {
                newestVersion = await Updater.Check();
                if (newestVersion <= Assembly.GetExecutingAssembly().GetName().Version) return;
                hyperLinkText.Text = @"Update to " + newestVersion + @" available.";
                UpdateUri.Click += UpdateUri_Click;
            }
            catch (Exception)
            {
                // Ignore ratelimitedexception
            }
        }

        private void UpdateUri_Click(object sender, RoutedEventArgs e)
        {
            var updater = new Process
            {
                StartInfo =
                {
                    FileName = @"Updater.exe",
                    Arguments =
                        @"osuTrainer.exe ""https://github.com/condone/osuTrainer/releases/download/v" + newestVersion +
                        @"/osuTrainer.zip"""
                }
            };
            updater.Start();
        }

        private void ToggleFlyout(int index)
        {
            var flyout = Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeListBox.SelectedIndex == 0)
            {
                Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(this);
                AppTheme expectedTheme = ThemeManager.GetAppTheme("BaseLight");

                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, expectedTheme);
            }
            else
            {
                Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(this);
                AppTheme expectedTheme = ThemeManager.GetAppTheme("BaseDark");

                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, expectedTheme);
            }
        }

        private void FlyoutListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlyoutListBox.SelectedIndex == 0)
            {
                settingsFlyout.Theme = FlyoutTheme.Light;
            }
            else
            {
                settingsFlyout.Theme = FlyoutTheme.Dark;
            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            Title = "osu! Trainer " + Assembly.GetExecutingAssembly().GetName().Version;
            LoadPreviousSettings();
            CheckUpdates();
            settingsFlyout.IsOpenChanged += SettingsFlyoutOnIsOpenChanged;
        }
    }
}