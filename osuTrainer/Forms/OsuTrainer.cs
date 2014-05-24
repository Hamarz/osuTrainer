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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Text;

namespace osuTrainer.Forms
{
    public partial class OsuTrainer : Form
    {
        private CustomWebClient client = new CustomWebClient();
        public IUser currentUser;
        public int[] standardids;
        public int[] taikoids;
        public int[] ctbids;
        public int[] maniaids;
        private SortableBindingList<ScoreInfo> scoreSugDisplay;
        // Key = Beatmap ID
        public Dictionary<int, Beatmap> beatmapCache;
        private int currentBeatmap;
        private GlobalVars.Mods mods;
        private int skippedIds = 1;
        private TimeSpan maxDuration;
        private const int pbMax = 60;
        private const int pbMaxhalf = 30;
        private Object firstLock = new Object();
        private Object secondLock = new Object();
        public OsuTrainer()
        {
            InitializeComponent();
            progressBar1.Maximum = pbMax;
            this.Location = new Point(Screen.GetWorkingArea(this).Right - Convert.ToInt32(Size.Width * 1.5),
                          Screen.GetWorkingArea(this).Bottom - Convert.ToInt32(Size.Height * 1.3));
            GameModeCB.DataSource = Enum.GetValues(typeof(GlobalVars.GameMode));
        }

        private void OsuTrainer_Load(object sender, EventArgs e)
        {
            LoadSettings();

            CheckUser();

            this.Text = "osu! Trainer " + Assembly.GetExecutingAssembly().GetName().Version;

            LoadUsers();

            UpdateDataGrid(GameModeCB.SelectedIndex);
            
            this.GameModeCB.SelectedIndexChanged += new System.EventHandler(this.GameModeCB_SelectedIndexChanged);

            UpdateCB();
        }

        private void LoadSettings()
        {
            maxDuration = TimeSpan.FromSeconds(Properties.Settings.Default.Searchduration);
            SearchtimeTB.Value = Properties.Settings.Default.Searchduration;
            mods = (GlobalVars.Mods)Properties.Settings.Default.Mods;
            GameModeCB.SelectedIndex = Properties.Settings.Default.GameMode;
        }

        private void UpdateCB()
        {
            if (mods.HasFlag(GlobalVars.Mods.DoubleTime))
            {
                DoubletimeCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.Hidden))
            {
                HiddenCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.HardRock))
            {
                HardrockCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.Flashlight))
            {
                FlashlightCB.Checked = true;
            }
            ExclusiveCB.Checked = Properties.Settings.Default.Exclusive;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Searchduration = SearchtimeTB.Value;
            Properties.Settings.Default.Mods = (int)mods;
            Properties.Settings.Default.Exclusive = ExclusiveCB.Checked;
            Properties.Settings.Default.GameMode = GameModeCB.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private async void UpdateDataGrid(int gameMode)
        {
            double minPP = (double)MinPPTB.Value;
            progressBar1.Value = progressBar1.Minimum + 2;
            await Task.Factory.StartNew(() => UpdateSuggestionsAsync(minPP, gameMode));
            dataGridView1.DataSource = scoreSugDisplay;
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[0].HeaderText = "";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].Width = 75;
            dataGridView1.Columns[3].Width = 50;
            dataGridView1.Sort(dataGridView1.Columns[5], ListSortDirection.Ascending);
            progressBar1.Value = progressBar1.Maximum;
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No suitable maps found.");
            }
            SaveSettings();
        }

        private void CheckUser()
        {
            using (Login login = new Login(false))
            {
                if (login.ShowDialog() == DialogResult.Cancel)
                {
                    Close();
                }
                switch (Properties.Settings.Default.GameMode)
                {
                    case 0:
                        currentUser = new UserStandard(login.userString, true);
                        break;
                    case 1:
                        currentUser = new UserTaiko(login.userString, true);
                        break;
                    case 2:
                        currentUser = new UserCtb(login.userString, true);
                        break;
                    case 3:
                        currentUser = new UserMania(login.userString, true);
                        break;
                    default:
                        break;
                }
                Properties.Settings.Default.UserId = currentUser.User_id.ToString();
                Properties.Settings.Default.Username = currentUser.Username;
                Properties.Settings.Default.Save();
            }
            LoadUserSettings();
        }

        private void LoadUsers()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("standard", FileMode.Open, FileAccess.Read))
            {
                standardids = (int[])formatter.Deserialize(fs);
            }
            using (FileStream fs = new FileStream("taiko", FileMode.Open, FileAccess.Read))
            {
                taikoids = (int[])formatter.Deserialize(fs);
            }
            using (FileStream fs = new FileStream("ctb", FileMode.Open, FileAccess.Read))
            {
                ctbids = (int[])formatter.Deserialize(fs);
            }
            using (FileStream fs = new FileStream("mania", FileMode.Open, FileAccess.Read))
            {
                maniaids = (int[])formatter.Deserialize(fs);
            }
        }

        private void ChangeUserButton_Click(object sender, EventArgs e)
        {
            using (Login login = new Login(true))
            {
                if (login.ShowDialog() != DialogResult.Cancel)
                {
                    switch (Properties.Settings.Default.GameMode)
                    {
                        case 0:
                            currentUser = new UserStandard(login.userString, true);
                            break;
                        case 1:
                            currentUser = new UserTaiko(login.userString, true);
                            break;
                        case 2:
                            currentUser = new UserCtb(login.userString, true);
                            break;
                        case 3:
                            currentUser = new UserMania(login.userString, true);
                            break;
                        default:
                            break;
                    }
                    Properties.Settings.Default.UserId = currentUser.User_id.ToString();
                    Properties.Settings.Default.Username = currentUser.Username;
                    Properties.Settings.Default.Save();
                    LoadUserSettings();
                    UpdateDataGrid(Properties.Settings.Default.GameMode);
                }
            }
        }

        private void LoadUserSettings()
        {
            if (currentUser.BestScores.Count > 0)
            {
                MinPPTB.Minimum = (int)currentUser.BestScores.Last().PP;
                MinPPTB.Maximum = (int)currentUser.BestScores.First().PP + 1;
            }
            else
            {
                MinPPTB.Minimum = 0;
                MinPPTB.Maximum = 20;
            }
            MinPPTB.Value = MinPPTB.Minimum;
            MinPPLabel.Text = Convert.ToString(MinPPTB.Value);
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

        private int FindStartingUser(double targetpp, int gameMode)
        {
            int low = 0;
            int high = standardids.Length - 1;
            int midpoint = 0;
            int iterations = 0;
            IUser miduser = null;
            while (low < high && iterations < 7)
            {
                midpoint = low + (high - low) / 2;
                switch (gameMode)
                {
                    case 0:
                        miduser = new UserStandard(standardids[midpoint].ToString());
                        break;
                    case 1:
                        miduser = new UserTaiko(standardids[midpoint].ToString());
                        break;
                    case 2:
                        miduser = new UserCtb(standardids[midpoint].ToString());
                        break;
                    case 3:
                        miduser = new UserMania(standardids[midpoint].ToString());
                        break;
                    default:
                        break;
                }
                if (targetpp > miduser.PpRaw)
                {
                    high = midpoint - 1;
                }
                else if (miduser.PpRaw - targetpp < 100)
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

        private void UpdateSuggestionsAsync(double minPP, int gameMode)
        {
            int[] userids;
            int startid;
            switch (gameMode)
            {
                case 0:
                    userids = standardids;
                    startid = currentUser.PpRaw < 200 ? 12843 :
                        currentUser.PpRank < 5001 ? startid = currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode);
                    break;
                case 1:
                    userids = taikoids;
                    startid = currentUser.PpRaw < 200 ? 6806 :
                        currentUser.PpRank < 5001 ? startid = currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode);
                    break;
                case 2:
                    userids = ctbids;
                    startid = currentUser.PpRaw < 200 ? 7638 :
                        currentUser.PpRank < 5001 ? startid = currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode);
                    break;
                case 3:
                    userids = maniaids;
                    startid = currentUser.PpRaw < 200 ? 7569 :
                        currentUser.PpRank < 5001 ? startid = currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode);
                    break;
                default:
                    return;
            }
            scoreSugDisplay = new SortableBindingList<ScoreInfo>();
            beatmapCache = new Dictionary<int, Beatmap>();
            GlobalVars.Mods ModsAndNV = mods | GlobalVars.Mods.NoVideo;
            foreach (var score in currentUser.BestScores)
            {
                beatmapCache.Add(score.Beatmap_Id,null);
            }
            int pChecked = 0;
            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0, 999, (i, state) =>
            {
                while (sw.Elapsed < maxDuration)
                {
                    string json = "";
                    lock (firstLock)
                    {
                        if (startid < 0)
                        {
                            state.Break();
                            return;
                        }
                        json = client.DownloadString(GlobalVars.UserBestAPI + userids[startid] + GlobalVars.Mode + gameMode);
                        startid -= skippedIds;
                        pChecked++;
                        Invoke((MethodInvoker)delegate
                        {
                            PlayersCheckedLbl.Text = pChecked.ToString();
                        });
                    }
                    List<UserBest> userBestList = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
                    for (int j = 0; j < userBestList.Count; j++)
                    {
                        if (userBestList[j].PP < minPP)
                        {
                            break;
                        }
                        if ((ExclusiveCB.Checked && (userBestList[j].Enabled_Mods == ModsAndNV || userBestList[j].Enabled_Mods == mods)) || (!ExclusiveCB.Checked && userBestList[j].Enabled_Mods.HasFlag(mods)))
                        {
                            lock (secondLock)
                            {
                                if (!beatmapCache.ContainsKey(userBestList[j].Beatmap_Id))
                                {
                                    Beatmap beatmap = new Beatmap(userBestList[j].Beatmap_Id);
                                    beatmapCache.Add(beatmap.Beatmap_id, beatmap);
                                    scoreSugDisplay.Add(new ScoreInfo { BeatmapName = beatmap.Title, Version = beatmap.Version, Artist = beatmap.Artist, Enabled_Mods = userBestList[j].Enabled_Mods, ppRaw = (int)Math.Truncate(userBestList[j].PP), RankImage = GetRankImage(userBestList[j].Rank), BeatmapId = beatmap.Beatmap_id });
                                    Invoke((MethodInvoker)delegate
                                    {
                                        if (progressBar1.Value < pbMax)
                                        {
                                            progressBar1.Value++;
                                        }
                                        ScoresAddedLbl.Text = Convert.ToString(scoreSugDisplay.Count);
                                    });
                                }
                            }
                        }
                    }
                }
                state.Break();
            });
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
            PlayersCheckedLbl.Text = "0";
            ScoresAddedLbl.Text = "0";
            UpdateDataGrid(GameModeCB.SelectedIndex);
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

        private void MinPPTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(MinPPTB, MinPPTB.Value.ToString());
            MinPPLabel.Text = Convert.ToString(MinPPTB.Value);
        }

        private void HiddenCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HiddenCB.Checked)
            {
                mods |= GlobalVars.Mods.Hidden;
            }
            else
            {
                mods &= ~GlobalVars.Mods.Hidden;
            }
        }

        private void DoubletimeCB_CheckedChanged(object sender, EventArgs e)
        {
            if (DoubletimeCB.Checked)
            {
                mods |= GlobalVars.Mods.DoubleTime;
            }
            else
            {
                mods &= ~GlobalVars.Mods.DoubleTime;
            }
        }

        private void FlashlightCB_CheckedChanged(object sender, EventArgs e)
        {
            if (FlashlightCB.Checked)
            {
                mods |= GlobalVars.Mods.Flashlight;
            }
            else
            {
                mods &= ~GlobalVars.Mods.Flashlight;
            }
        }

        private void HardrockCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HardrockCB.Checked)
            {
                mods |= GlobalVars.Mods.HardRock;
            }
            else
            {
                mods &= ~GlobalVars.Mods.HardRock;
            }
        }

        private void SearchtimeTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SearchtimeTB, (SearchtimeTB.Value * 2).ToString());
            maxDuration = TimeSpan.FromSeconds(SearchtimeTB.Value);
        }

        private void GameModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (GameModeCB.SelectedIndex)
            {
                case 0:
                    currentUser = new UserStandard(Properties.Settings.Default.Username);
                    LoadUserSettings();
                    break;
                case 1:
                    currentUser = new UserTaiko(Properties.Settings.Default.Username);
                    LoadUserSettings();
                    break;
                case 2:
                    currentUser = new UserCtb(Properties.Settings.Default.Username);
                    LoadUserSettings();
                    break;
                case 3:
                    currentUser = new UserMania(Properties.Settings.Default.Username);
                    LoadUserSettings();
                    break;
                default:
                    break;
            }
        }
    }
}