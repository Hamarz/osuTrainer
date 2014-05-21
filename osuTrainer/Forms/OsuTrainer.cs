using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Text;

namespace osuTrainer.Forms
{
    public partial class OsuTrainer : Form
    {
        private CustomWebClient client = new CustomWebClient();
        public User currentUser;
        public List<UserBest> existingBest = new List<UserBest>();
        private int minSuggestions = 20;
        public int[] userids;
        private SortedSet<UserBest> scoreSuggestions = new SortedSet<UserBest>();
        private SortableBindingList<ScoreInfo> scoreSugDisplay;

        // Key = Beatmap ID
        public Dictionary<int, Beatmap> beatmapCache;

        private int currentBeatmap;

        public OsuTrainer()
        {
            InitializeComponent();
            this.Location = new Point(Screen.GetWorkingArea(this).Right - Convert.ToInt32(Size.Width * 1.5),
                          Screen.GetWorkingArea(this).Bottom - Convert.ToInt32(Size.Height * 1.3));
        }

        private void OsuTrainer_Load(object sender, EventArgs e)
        {
            CheckAPIKey();

            CheckUser();

            LoadUsers();

            FillDataGrid();
        }

        private void CheckAPIKey()
        {
            if (Properties.Settings.Default.APIKey.Length < 1 || client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + Properties.Settings.Default.APIKey + "&u=1").Length > 3)
            {
                using (GetAPIKey getApiKey = new GetAPIKey())
                {
                    if (getApiKey.ShowDialog() == DialogResult.Cancel)
                    {
                        Close();
                    }
                }
            }
        }

        private async void FillDataGrid()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = minSuggestions * 2 + 5;
            progressBar1.Value = progressBar1.Value + 2;
            await Task.Factory.StartNew(() => UpdateSuggestionsAsync());
            dataGridView1.DataSource = scoreSugDisplay;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            progressBar1.Value = progressBar1.Maximum;
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No suitable maps found.");
            }
        }

        private void CheckUser()
        {
            if (!User.Exists(Properties.Settings.Default.UserId))
            {
                using (Login login = new Login())
                {
                    if (login.ShowDialog() == DialogResult.Cancel)
                    {
                        Close();
                    }
                    currentUser = login.newUser;
                    Properties.Settings.Default.UserId = currentUser.User_id.ToString();
                    Properties.Settings.Default.Username = currentUser.Username;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                currentUser = new User(Properties.Settings.Default.UserId);
                Properties.Settings.Default.Username = currentUser.Username;
                Properties.Settings.Default.Save();
            }
        }

        private void LoadUsers()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("ids", FileMode.Open, FileAccess.Read))
            {
                userids = (int[])formatter.Deserialize(fs);
            }
        }

        private void ChangeUserButton_Click(object sender, EventArgs e)
        {
            using (Login login = new Login())
            {
                if (login.ShowDialog() != DialogResult.Cancel)
                {
                    currentUser = login.newUser;
                    Properties.Settings.Default.UserId = currentUser.User_id.ToString();
                    Properties.Settings.Default.Username = currentUser.Username;
                    Properties.Settings.Default.Save();
                    scoreSugDisplay = null;
                    FillDataGrid();
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Beatmap selected;
                beatmapCache.TryGetValue((int)dataGridView1.SelectedRows[0].Cells[5].Value, out selected);
                ArtistLbl.Text = selected.Artist;
                TitleLbl.Text = selected.Title;
                CreatorLbl.Text = selected.Creator;
                TotalTimeLbl.Text = TimeSpan.FromSeconds(selected.Total_length).ToString(@"mm\:ss");
                DrainingTimeLbl.Text = TimeSpan.FromSeconds(selected.Hit_length).ToString(@"mm\:ss");
                BpmLbl.Text = selected.Bpm.ToString("F2");
                pictureBox1.Load(selected.ThumbnailUrl);
            }
        }

        private int FindStartingUser(double targetpp)
        {
            int low = 0;
            int high = userids.Length - 1;
            int midpoint = 0;
            while (low < high)
            {
                midpoint = low + (high - low) / 2;
                User miduser = new User(userids[midpoint].ToString());
                if (targetpp > miduser.Pp_raw)
                {
                    high = midpoint - 1;
                }
                else if (miduser.Pp_raw - targetpp < 200)
                {
                    return midpoint;
                }
                else
                {
                    low = midpoint - 1;
                }
            }
            return midpoint;
        }

        private void UpdateSuggestionsAsync()
        {
            scoreSugDisplay = new SortableBindingList<ScoreInfo>();
            int startid = currentUser.Pp_raw < 700 ? 3499 :
                currentUser.Pp_rank < 51 ? startid = currentUser.Pp_rank - 2 :
                FindStartingUser(currentUser.Pp_raw);
            double minPP = (currentUser.BestScores.First().PP + currentUser.BestScores.Skip(1).First().PP) / 2;
            int foundSuggestions = 0;
            int noSuggestions = 0;
            while (foundSuggestions < minSuggestions)
            {
                string json = client.DownloadString(GlobalVars.UserBestAPI + userids[startid]);
                List<UserBest> tempList = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j].PP > minPP)
                    {
                        if (!currentUser.BestScores.Any(score => score.Beatmap_Id == tempList[j].Beatmap_Id))
                        {
                            if (scoreSuggestions.Add(tempList[j]))
                            {
                                foundSuggestions++;
                                Invoke((MethodInvoker)delegate
                                {
                                    progressBar1.Value++;
                                });
                            }
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            noSuggestions++;
                            startid = startid - 5;
                        }
                        break;
                    }
                }
                if (noSuggestions > 8)
                {
                    break;
                }
                if (startid > 1)
                {
                    startid--;
                }
                else
                {
                    break;
                }
            }
            beatmapCache = new Dictionary<int, Beatmap>();
            foreach (var score in scoreSuggestions)
            {
                Beatmap beatmap = new Beatmap(score.Beatmap_Id);
                beatmapCache.Add(beatmap.Beatmap_id, beatmap);
                scoreSugDisplay.Add(new ScoreInfo { BeatmapName = beatmap.Title, Artist = beatmap.Artist, Enabled_Mods = score.Enabled_Mods, ppRaw = (int)Math.Truncate(score.PP), RankImage = GetRankImage(score.Rank), BeatmapId = beatmap.Beatmap_id });
                Invoke((MethodInvoker)delegate
                {
                    if (progressBar1.Value < progressBar1.Maximum)
                    {
                        progressBar1.Value++;
                    }
                });
            }
        }

        private Bitmap GetRankImage(GlobalVars.Rank rank)
        {
            switch (rank)
            {
                case GlobalVars.Rank.S:
                    return Properties.Resources.S_small;

                case GlobalVars.Rank.A:
                    return Properties.Resources.A_small;

                case GlobalVars.Rank.X:
                    return Properties.Resources.X_small;

                case GlobalVars.Rank.SH:
                    return Properties.Resources.SH_small;

                case GlobalVars.Rank.XH:
                    return Properties.Resources.XH_small;

                case GlobalVars.Rank.B:
                    return Properties.Resources.B_small;

                case GlobalVars.Rank.C:
                    return Properties.Resources.C_small;

                case GlobalVars.Rank.D:
                    return Properties.Resources.D_small;

                default:
                    return null;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            FillDataGrid();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                currentBeatmap = (int)dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].Cells[5].Value;
                dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].Selected = true;
                ContextMenu m = new ContextMenu();
                MenuItem beatmapPage = new MenuItem("Beatmap Page");
                MenuItem download = new MenuItem("Download from Bloodcat");
                m.MenuItems.Add(beatmapPage);
                m.MenuItems.Add(download);
                beatmapPage.Click += new System.EventHandler(beatmapPage_Click);
                download.Click += new System.EventHandler(download_Click);
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void beatmapPage_Click(object sender, System.EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == currentBeatmap).Value.Url);
        }

        private void download_Click(object sender, System.EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == currentBeatmap).Value.BloodcatUrl);
        }
    }
}