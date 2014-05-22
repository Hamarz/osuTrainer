using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public int[] userids;
        private SortableBindingList<ScoreInfo> scoreSugDisplay;

        // Key = Beatmap ID
        public Dictionary<int, Beatmap> beatmapCache;
        private int currentBeatmap;
        private GlobalVars.Mods mods;
        private int skippedIds;
        private TimeSpan maxDuration = TimeSpan.FromSeconds(2);
        private const int pbMax = 50;
        private const int pbMaxhalf = 25;
        private Object thisLock = new Object();

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

            this.Text = "osu! Trainer " + Assembly.GetExecutingAssembly().GetName().Version;

            LoadUsers();

            mods = (GlobalVars.Mods)Properties.Settings.Default.Mods;
            FillDataGrid();

            switch (mods)
            {
                case GlobalVars.Mods.DoubleTime:
                    DoubletimeCB.Checked = true;
                    break;

                case GlobalVars.Mods.DoubleTime | GlobalVars.Mods.Hidden:
                    DoubletimeCB.Checked = true;
                    HiddenCB.Checked = true;
                    break;

                case GlobalVars.Mods.DoubleTime | GlobalVars.Mods.HardRock:
                    DoubletimeCB.Checked = true;
                    HardrockCB.Checked = true;
                    break;

                case GlobalVars.Mods.Hidden:
                    HiddenCB.Checked = true;
                    break;

                case GlobalVars.Mods.HardRock:
                    HardrockCB.Checked = true;
                    break;

                case GlobalVars.Mods.HardRock | GlobalVars.Mods.Hidden:
                    HiddenCB.Checked = true;
                    HardrockCB.Checked = true;
                    break;

                case GlobalVars.Mods.HardRock | GlobalVars.Mods.Hidden | GlobalVars.Mods.DoubleTime:
                    HiddenCB.Checked = true;
                    HardrockCB.Checked = true;
                    DoubletimeCB.Checked = true;
                    break;

                case GlobalVars.Mods.Flashlight:
                    FlashlightCB.Checked = true;
                    break;

                default:
                    break;
            }
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
            if (currentUser.Pp_rank > 50000)
            {
                skippedIds = 10;
            }
            else if (currentUser.Pp_rank > 10000)
            {
                skippedIds = 5;
            }
            else if (currentUser.Pp_rank > 1000)
            {
                skippedIds = 2;
            }
            else if (currentUser.Pp_rank > 200)
            {
                skippedIds = 1;
            }
            else
            {
                skippedIds = 0;
            }

            Properties.Settings.Default.Mods = (int)mods;
            Properties.Settings.Default.Save();
            trackBar1.Minimum = (int)currentUser.BestScores.Last().PP;
            trackBar1.Maximum = (int)currentUser.BestScores.First().PP + 1;
            double minPP = (double)trackBar1.Value;
            MinPPLabel.Text = Convert.ToString(trackBar1.Value);
            progressBar1.Value = 0;
            progressBar1.Maximum = pbMax;
            progressBar1.Value = progressBar1.Value + 2;
            await Task.Factory.StartNew(() => UpdateSuggestionsAsync(minPP));
            dataGridView1.DataSource = scoreSugDisplay;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[0].HeaderText = "";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].Width = 75;
            dataGridView1.Sort(dataGridView1.Columns[5], ListSortDirection.Ascending);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            progressBar1.Value = progressBar1.Maximum;
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No suitable maps found.");
            }
        }

        private void CheckUser()
        {
            using (Login login = new Login(false))
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
            using (Login login = new Login(true))
            {
                if (login.ShowDialog() != DialogResult.Cancel)
                {
                    currentUser = login.newUser;
                    Properties.Settings.Default.UserId = currentUser.User_id.ToString();
                    Properties.Settings.Default.Username = currentUser.Username;
                    Properties.Settings.Default.Save();
                    scoreSugDisplay = null;
                    trackBar1.Value = trackBar1.Minimum;
                    FillDataGrid();
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Beatmap selected;
                beatmapCache.TryGetValue((int)dataGridView1.SelectedRows[0].Cells[6].Value, out selected);
                ArtistLbl.Text = selected.Artist;
                TitleLbl.Text = selected.Title;
                CreatorLbl.Text = selected.Creator;
                TotalTimeLbl.Text = TimeSpan.FromSeconds(selected.Total_length).ToString(@"mm\:ss");
                DrainingTimeLbl.Text = TimeSpan.FromSeconds(selected.Hit_length).ToString(@"mm\:ss");
                BpmLbl.Text = selected.Bpm.ToString("F2");
                try
                {
                    pictureBox1.Load(selected.ThumbnailUrl);
                }
                catch (Exception)
                {
                    pictureBox1.Image = null;
                }
            }
        }

        private int FindStartingUser(double targetpp)
        {
            int low = 0;
            int high = userids.Length - 1;
            int midpoint = 0;
            int iterations = 0;
            while (low < high && iterations < 7)
            {
                midpoint = low + (high - low) / 2;
                User miduser = new User(userids[midpoint].ToString());
                if (targetpp > miduser.Pp_raw)
                {
                    high = midpoint - 1;
                }
                else if (miduser.Pp_raw - targetpp < 100)
                {
                    return midpoint;
                }
                else
                {
                    low = midpoint - 1;
                }
                iterations++;
            }
            return midpoint;
        }

        private void UpdateSuggestionsAsync(double minPP)
        {
            scoreSugDisplay = new SortableBindingList<ScoreInfo>();
            ConcurrentBag<UserBest> scoreSuggestions = new ConcurrentBag<UserBest>();
            ConcurrentBag<int> addedScores = new ConcurrentBag<int>();
            int startid = currentUser.Pp_raw < 200 ? 12849 :
                currentUser.Pp_rank < 5001 ? startid = currentUser.Pp_rank - 2 :
                FindStartingUser(currentUser.Pp_raw);
            foreach (var score in currentUser.BestScores)
            {
                addedScores.Add(score.Beatmap_Id);
            }
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.For(0, 1, (i, state) =>
            {
                while (sw.Elapsed < maxDuration)
                {
                    string json = "";
                    lock (thisLock)
                    {
                        json = client.DownloadString(GlobalVars.UserBestAPI + userids[startid]);
                    }
                    List<UserBest> tempList = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j].PP > minPP)
                        {
                            if ((tempList[j].Enabled_Mods == (mods | GlobalVars.Mods.NoVideo) || tempList[j].Enabled_Mods == mods))
                            {
                                if (!addedScores.Contains(tempList[j].Beatmap_Id))
                                {
                                    scoreSuggestions.Add(tempList[j]);
                                    addedScores.Add(tempList[j].Beatmap_Id);
                                    Invoke((MethodInvoker)delegate
                                    {
                                        if (progressBar1.Value < pbMaxhalf)
                                        {
                                            progressBar1.Value++;
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            startid -= skippedIds;
                            break;
                        }
                    }
                    if (startid > 0)
                    {
                        startid--;
                    }
                    else
                    {
                        break;
                    }
                }
                state.Break();
            });

            Invoke((MethodInvoker)delegate
            {
                progressBar1.Value = pbMaxhalf;
            });
            beatmapCache = new Dictionary<int, Beatmap>();
            foreach (var score in scoreSuggestions)
            {
                Beatmap beatmap = new Beatmap(score.Beatmap_Id);
                beatmapCache.Add(beatmap.Beatmap_id, beatmap);
                scoreSugDisplay.Add(new ScoreInfo { BeatmapName = beatmap.Title, Version = beatmap.Version, Artist = beatmap.Artist, Enabled_Mods = score.Enabled_Mods, ppRaw = (int)Math.Truncate(score.PP), RankImage = GetRankImage(score.Rank), BeatmapId = beatmap.Beatmap_id });
                Invoke((MethodInvoker)delegate
                {
                    if (progressBar1.Value < pbMax)
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
                currentBeatmap = (int)dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].Cells[6].Value;
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(trackBar1, trackBar1.Value.ToString());
            MinPPLabel.Text = Convert.ToString(trackBar1.Value);
        }

        private void HiddenCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HiddenCB.Checked)
            {
                if (!mods.HasFlag(GlobalVars.Mods.Hidden))
                {
                    mods |= GlobalVars.Mods.Hidden;
                }
            }
            else
            {
                if (mods.HasFlag(GlobalVars.Mods.Hidden))
                {
                    mods -= GlobalVars.Mods.Hidden;
                }
            }
        }

        private void DoubletimeCB_CheckedChanged(object sender, EventArgs e)
        {
            if (DoubletimeCB.Checked)
            {
                if (!mods.HasFlag(GlobalVars.Mods.DoubleTime))
                {
                    mods |= GlobalVars.Mods.DoubleTime;
                }
            }
            else
            {
                if (mods.HasFlag(GlobalVars.Mods.DoubleTime))
                {
                    mods -= GlobalVars.Mods.DoubleTime;
                }
            }
        }

        private void FlashlightCB_CheckedChanged(object sender, EventArgs e)
        {
            if (FlashlightCB.Checked)
            {
                if (!mods.HasFlag(GlobalVars.Mods.Flashlight))
                {
                    mods |= GlobalVars.Mods.Flashlight;
                }
            }
            else
            {
                if (mods.HasFlag(GlobalVars.Mods.Flashlight))
                {
                    mods -= GlobalVars.Mods.Flashlight;
                }
            }
        }

        private void HardrockCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HardrockCB.Checked)
            {
                if (!mods.HasFlag(GlobalVars.Mods.HardRock))
                {
                    mods |= GlobalVars.Mods.HardRock;
                }
            }
            else
            {
                if (mods.HasFlag(GlobalVars.Mods.HardRock))
                {
                    mods -= GlobalVars.Mods.HardRock;
                }
            }
        }

        private void SearchtimeTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SearchtimeTB, (SearchtimeTB.Value * 2).ToString());
            maxDuration = TimeSpan.FromSeconds(SearchtimeTB.Value);
        }
    }
}