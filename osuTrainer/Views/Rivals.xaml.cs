using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using osuTrainer.ViewModels;
using ServiceStack;
using ServiceStack.Text;

namespace osuTrainer.Views
{
    /// <summary>
    ///     Interaction logic for Rivals.xaml
    /// </summary>
    public partial class Rivals : UserControl
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private DispatcherTimer _timer;
        private readonly CustomWebClient _client = new CustomWebClient();
        private List<int> _rivals = new List<int>();
        private readonly SortedSet<UserScore> _score = new SortedSet<UserScore>();
        private readonly string _textfile = "rivals.txt";

        public Rivals()
        {
            InitializeComponent();
        }
        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            foreach (int rival in _rivals)
            {
                string json =
                    _client.DownloadString(GlobalVars.UserApi + "7725cd9f63aef9abcdeee48a9c4b9c0f23ec3b68" + "&u=" +
                                           rival + "&event_days=" + 31);
                var test = JsonSerializer.DeserializeFromString<List<User>>(json);
                foreach (Event item in test.First().Events)
                {
                    Match match = Regex.Match(item.Display_Html, @"#(\d+?).+?'>(.+?)<.+?\((.+?)\)");
                    if (match.Groups.Count > 3)
                    {
                        _score.Add(new UserScore
                        {
                            Rank = Convert.ToInt32(match.Groups[1].Value),
                            BeatmapName = match.Groups[2].Value,
                            Beatmap_id = item.Beatmap_Id,
                            Beatmapset_Id = item.Beatmapset_Id,
                            Date = item.Date - new TimeSpan(8, 0, 0),
                            User_Id = test.First().User_Id,
                            Username = test.First().Username,
                            Mode = match.Groups[3].Value
                        });
                    }
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!_worker.IsBusy && _rivals.Count>0)
            {
                _worker.RunWorkerAsync();
            }
        }
        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            UpdateDisplay();
        }
        private void Rivals_OnInitialized(object sender, EventArgs e)
        {
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 5, 0)
            };
            _timer.Start();
            _timer.Tick += timer_Tick;
            _worker.DoWork += WorkerOnDoWork;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            LoadRivals();
            timer_Tick(sender, e);
        }

        private void UpdateDisplay()
        {
            RivalsSp.Children.Clear();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            int row = 0;
            foreach (UserScore item in _score)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                var link = new Hyperlink();
                link.NavigateUri = new Uri(GlobalVars.UserUrl + item.User_Id);
                link.RequestNavigate += LinkOnRequestNavigate;
                link.Inlines.Add(new Run(item.Username));

                var text = new TextBlock();
                text.Inlines.Add(link);
                text.Inlines.Add(" achieved rank #" + item.Rank + " on ");

                link = new Hyperlink();
                link.NavigateUri = new Uri(GlobalVars.BeatmapUrl + item.Beatmap_id);
                link.RequestNavigate += LinkOnRequestNavigate;
                link.Inlines.Add(new Run(item.BeatmapName));

                text.Inlines.Add(link);
                text.Inlines.Add(" (" + item.Mode + ")");
                grid.Children.Add(text);
                Grid.SetRow(text, row);
                Grid.SetColumn(text, 1);

                text = new TextBlock();
                text.Text = ElapsedDate(item.Date);
                grid.Children.Add(text);
                Grid.SetRow(text, row);
                Grid.SetColumn(text, 0);

                row++;
            }
            RivalsSp.Children.Add(grid);
        }

        private string ElapsedDate(DateTime date)
        {
            TimeSpan elapsed = DateTime.Now.ToUniversalTime() - date;
            if (elapsed.Days > 0)
            {
                if (elapsed.Days > 1)
                {
                    return elapsed.Days + " Days ago";
                }
                return elapsed.Days + " Day ago";
            }
            if (elapsed.Hours > 0)
            {
                if (elapsed.Hours > 1)
                {
                    return elapsed.Hours + " Hours ago";
                }
                return elapsed.Hours + " Hour ago";
            }
            if (elapsed.Minutes > 1)
            {
                return elapsed.Minutes + " Minutes ago";
            }
            return elapsed.Minutes + " Minute ago";
        }

        private void LinkOnRequestNavigate(object sender, RequestNavigateEventArgs requestNavigateEventArgs)
        {
            Process.Start(requestNavigateEventArgs.Uri.AbsoluteUri);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var editor = new Editor();
            editor.Owner = Application.Current.MainWindow;
            editor.ShowDialog();
            if (editor.Saved)
            {
                LoadRivals();
                timer_Tick(sender, e);
            }
        }

        private void LoadRivals()
        {
            if (File.Exists(_textfile))
            {
                using (var reader = new StreamReader(_textfile))
                {
                    string line;
                    _rivals = new List<int>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#")) continue;
                        try
                        {
                            _rivals.Add(Convert.ToInt32(line));
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Failed reading " + _textfile);
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(_textfile + "not found.");
            }
        }
    }

    internal class UserScore : IEquatable<UserScore>, IComparable<UserScore>
    {
        public int Rank { get; set; }
        public string BeatmapName { get; set; }
        public int Beatmap_id { get; set; }
        public int Beatmapset_Id { get; set; }
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string Mode { get; set; }
        public DateTime Date { get; set; }

        public int CompareTo(UserScore other)
        {
            if (other.Equals(this))
                return 0;

            return other.Date.CompareTo(Date);
        }

        public bool Equals(UserScore other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Beatmap_id == other.Beatmap_id && User_Id == other.User_Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Beatmap_id;
                hashCode = (hashCode * 397) ^ User_Id;
                return hashCode;
            }
        }
    }

    internal class User
    {
        public int User_Id { get; set; }
        public string Username { get; set; }
        public int Count300 { get; set; }
        public int Conut100 { get; set; }
        public int Count50 { get; set; }
        public int Playcount { get; set; }
        public int Ranked_Score { get; set; }
        public int Total_Score { get; set; }
        public int Pp_Rank { get; set; }
        public double Level { get; set; }
        public double Pp_Raw { get; set; }
        public double Accuracy { get; set; }
        public int Count_Rank_Ss { get; set; }
        public int Count_Rank_S { get; set; }
        public int Count_Rank_A { get; set; }
        public string Country { get; set; }
        public List<Event> Events { get; set; }
    }

    internal class Event
    {
        public string Display_Html { get; set; }
        public int Beatmap_Id { get; set; }
        public int Beatmapset_Id { get; set; }
        public DateTime Date { get; set; }
        public int Epicfactor { get; set; }
    }
}