using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using osuTrainer.Properties;
using ServiceStack.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace osuTrainer.Forms
{
    public partial class OsuTrainer : Form
    {
        // Key = Beatmap ID
        private Dictionary<int, Beatmap> beatmapCache;

        private int[] ctbids;
        private IUser currentUser;
        private int[] maniaids;
        private int[] standardids;
        private int[] taikoids;
        private string osuDirectory;
        private string osuExe;
        private const int pbMax = 60;
        private readonly CustomWebClient client = new CustomWebClient();
        private int selectedBeatmap;
        private readonly Object firstLock = new Object();
        private TimeSpan maxDuration;
        private GlobalVars.Mods mods;
        private SortableBindingList<ScoreInfo> scoreSugDisplay;
        private readonly Object secondLock = new Object();
        private int skippedIds = 1;
        public OsuTrainer()
        {
            InitializeComponent();
            progressBar1.Maximum = pbMax;
            this.Location = new Point(Screen.GetWorkingArea(this).Right - Convert.ToInt32(Size.Width * 1.5),
                          Screen.GetWorkingArea(this).Bottom - Convert.ToInt32(Size.Height * 1.3));
            GameModeCB.DataSource = Enum.GetValues(typeof(GlobalVars.GameMode));
        }

        private void beatmapPage_Click(object sender, EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == selectedBeatmap).Value.Url + GlobalVars.Mode + GameModeCB.SelectedIndex);
        }

        private void copyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(beatmapCache.Single(x => x.Key == selectedBeatmap).Value.Url + GlobalVars.Mode + GameModeCB.SelectedIndex);
        }

        private void dl_Click(object sender, EventArgs e)
        {
            Process.Start(GlobalVars.DownloadURL + beatmapCache.Single(x => x.Key == selectedBeatmap).Value.BeatmapSet_id);
        }

        private void osuDirect_Click(object sender, EventArgs e)
        {
            var osuDirect = new Process();
            osuDirect.StartInfo.FileName = osuExe;
            osuDirect.StartInfo.Arguments = @"osu://dl/" + beatmapCache.Single(x => x.Key == selectedBeatmap).Value.BeatmapSet_id;
            osuDirect.Start();
        }

        private async void CheckUpdates()
        {
            var newestVersion = await Updater.Check();
            if (newestVersion <= Assembly.GetExecutingAssembly().GetName().Version) return;
            UpdateLbl.IsLink = true;
            UpdateLbl.Text = @"Update to " + newestVersion + @" available.";
            UpdateLbl.Tag = "https://github.com/condone/osuTrainer/releases";
            UpdateLbl.LinkBehavior = LinkBehavior.AlwaysUnderline;
            UpdateLbl.Click += UpdateLbl_Click;
        }

        private void ChangeUserButton_Click(object sender, EventArgs e)
        {
            using (var login = new Login(true))
            {
                if (login.ShowDialog() == DialogResult.Cancel) return;
                currentUser = UserFactory.GetUser(Properties.Settings.Default.GameMode);
                currentUser.GetInfo(login.UserString, true);
                Properties.Settings.Default.UserId = currentUser.User_id.ToString(CultureInfo.InvariantCulture);
                Properties.Settings.Default.Username = currentUser.Username;
                Properties.Settings.Default.Save();
                LoadUserSettings();
                PlayersCheckedLbl.Text = @"0";
                ScoresAddedLbl.Text = @"0";
                UpdateDataGrid(Properties.Settings.Default.GameMode);
            }
        }

        private void CheckUser()
        {
            using (var login = new Login(false))
            {
                if (login.ShowDialog() == DialogResult.Cancel)
                {
                    Close();
                }
                currentUser = UserFactory.GetUser(Settings.Default.GameMode);
                currentUser.GetInfo(login.UserString, true);
                Settings.Default.UserId = currentUser.User_id.ToString(CultureInfo.InvariantCulture);
                Settings.Default.Username = currentUser.Username;
                Settings.Default.Save();
            }
            LoadUserSettings();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedBeatmap = (int)dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].Cells["BeatmapId"].Value;
                dataGridView1.Rows[dataGridView1.HitTest(e.X, e.Y).RowIndex].Selected = true;
                var m = new ContextMenu();
                var beatmapPage = new MenuItem("Beatmap link");
                var copyToClipboard = new MenuItem("Copy link to clipboard");
                var dl = new MenuItem("Download");
                var osuDirect = new MenuItem("Download with osu!direct");
                var dlBloodcat = new MenuItem("Download with Bloodcat");
                m.MenuItems.Add(beatmapPage);
                m.MenuItems.Add(copyToClipboard);
                m.MenuItems.Add(dl);
                m.MenuItems.Add(osuDirect);
                m.MenuItems.Add(dlBloodcat);
                beatmapPage.Click += beatmapPage_Click;
                copyToClipboard.Click += copyToClipboard_Click;
                dl.Click += dl_Click;
                osuDirect.Click += osuDirect_Click;
                dlBloodcat.Click += dlBloodcat_Click;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1) return;
            Beatmap selected;
            beatmapCache.TryGetValue((int)dataGridView1.SelectedRows[0].Cells["BeatmapId"].Value, out selected);
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

        private void DoubletimeCB_CheckedChanged(object sender, EventArgs e)
        {
            if (DoubletimeCB.Checked)
            {
                mods |= GlobalVars.Mods.DT;
            }
            else
            {
                mods &= ~GlobalVars.Mods.DT;
            }
        }

        private void dlBloodcat_Click(object sender, EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == selectedBeatmap).Value.BloodcatUrl);
        }

        private int FindStartingUser(double targetpp, int gameMode, int[] ids)
        {
            var low = 0;
            var high = ids.Length - 1;
            var midpoint = 0;
            var iterations = 0;
            while (low < high && iterations < 7)
            {
                midpoint = low + (high - low) / 2;
                IUser miduser = UserFactory.GetUser(gameMode);
                miduser.GetInfo(ids[midpoint].ToString(CultureInfo.InvariantCulture));
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

        private void FlashlightCB_CheckedChanged(object sender, EventArgs e)
        {
            if (FlashlightCB.Checked)
            {
                mods |= GlobalVars.Mods.FL;
            }
            else
            {
                mods &= ~GlobalVars.Mods.FL;
            }
        }

        private void GameModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentUser = UserFactory.GetUser(GameModeCB.SelectedIndex);
            currentUser.GetInfo(Settings.Default.Username);
            LoadUserSettings();
        }

        private Bitmap GetRankImage(GlobalVars.Rank rank)
        {
            switch (rank)
            {
                case GlobalVars.Rank.S:
                    return Resources.S_small;

                case GlobalVars.Rank.A:
                    return Resources.A_small;

                case GlobalVars.Rank.X:
                    return Resources.X_small;

                case GlobalVars.Rank.SH:
                    return Resources.SH_small;

                case GlobalVars.Rank.XH:
                    return Resources.XH_small;

                case GlobalVars.Rank.B:
                    return Resources.B_small;

                case GlobalVars.Rank.C:
                    return Resources.C_small;

                case GlobalVars.Rank.D:
                    return Resources.D_small;

                default:
                    return null;
            }
        }

        private void HardrockCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HardrockCB.Checked)
            {
                mods |= GlobalVars.Mods.HR;
            }
            else
            {
                mods &= ~GlobalVars.Mods.HR;
            }
        }

        private void HiddenCB_CheckedChanged(object sender, EventArgs e)
        {
            if (HiddenCB.Checked)
            {
                mods |= GlobalVars.Mods.HD;
            }
            else
            {
                mods &= ~GlobalVars.Mods.HD;
            }
        }

        private void LoadSettings()
        {
            maxDuration = TimeSpan.FromSeconds(Settings.Default.Searchduration);
            SearchtimeTB.Value = Settings.Default.Searchduration;
            mods = (GlobalVars.Mods)Settings.Default.Mods;
            GameModeCB.SelectedIndex = Settings.Default.GameMode;
        }

        private void LoadUsers()
        {
            var formatter = new BinaryFormatter();
            using (var fs = new FileStream("standard", FileMode.Open, FileAccess.Read))
            {
                standardids = (int[])formatter.Deserialize(fs);
            }
            using (var fs = new FileStream("taiko", FileMode.Open, FileAccess.Read))
            {
                taikoids = (int[])formatter.Deserialize(fs);
            }
            using (var fs = new FileStream("ctb", FileMode.Open, FileAccess.Read))
            {
                ctbids = (int[])formatter.Deserialize(fs);
            }
            using (var fs = new FileStream("mania", FileMode.Open, FileAccess.Read))
            {
                maniaids = (int[])formatter.Deserialize(fs);
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

        private void MinPPTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(MinPPTB, MinPPTB.Value.ToString(CultureInfo.InvariantCulture));
            MinPPLabel.Text = Convert.ToString(MinPPTB.Value);
        }

        private async void FindOsu()
        {
            osuDirectory = await FindOsuDirAsync();
            osuExe = Path.Combine(osuDirectory, @"osu!.exe");
        }

        private async Task<string> FindOsuDirAsync()
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"osu!\\DefaultIcon");
                if (key != null)
                {
                    object o = key.GetValue(null);
                    if (o != null)
                    {
                        var filter = new Regex(@"(?<="")[^\""]*(?="")");
                        return Path.GetDirectoryName(filter.Match(o.ToString()).ToString());
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }

        private void OsuTrainer_Load(object sender, EventArgs e)
        {
            if (Settings.Default.UpgradeRequired)
            {
                if (MessageBox.Show(@"Load settings from the previous version?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Settings.Default.Upgrade();
                }
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            CheckUpdates();

            FindOsu();

            LoadSettings();

            CheckUser();

            Text = @"osu! Trainer " + Assembly.GetExecutingAssembly().GetName().Version;

            LoadUsers();

            UpdateDataGrid(GameModeCB.SelectedIndex);

            GameModeCB.SelectedIndexChanged += this.GameModeCB_SelectedIndexChanged;

            UpdateCb();
        }
        private void SaveSettings()
        {
            Settings.Default.Searchduration = SearchtimeTB.Value;
            Settings.Default.Mods = (int)mods;
            Settings.Default.Exclusive = ExclusiveCB.Checked;
            Settings.Default.GameMode = GameModeCB.SelectedIndex;
            Settings.Default.Save();
        }

        private void SearchtimeTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(SearchtimeTB, (SearchtimeTB.Value * 2).ToString());
            maxDuration = TimeSpan.FromSeconds(SearchtimeTB.Value);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (UpdateButton.Text == @"Update")
            {
                PlayersCheckedLbl.Text = @"0";
                ScoresAddedLbl.Text = @"0";
                UpdateDataGrid(GameModeCB.SelectedIndex);
            }
            else
            {
                MessageBox.Show(@"Already updating!");
            }
        }

        private void UpdateCb()
        {
            if (mods.HasFlag(GlobalVars.Mods.DT))
            {
                DoubletimeCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.HD))
            {
                HiddenCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.HR))
            {
                HardrockCB.Checked = true;
            }
            if (mods.HasFlag(GlobalVars.Mods.FL))
            {
                FlashlightCB.Checked = true;
            }
            ExclusiveCB.Checked = Settings.Default.Exclusive;
        }
        private async void UpdateDataGrid(int gameMode)
        {
            UpdateButton.Text = @"Updating";
            var minPp = (double)MinPPTB.Value;
            progressBar1.Value = progressBar1.Minimum + 2;
            dataGridView1.DataSource = null;
            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            await Task.Run(() => UpdateSuggestionsAsync(minPp, gameMode));
            dataGridView1.DataSource = scoreSugDisplay;
            dataGridView1.Columns["BeatmapId"].Visible = false;
            dataGridView1.Columns["RankImage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns["RankImage"].HeaderText = "";
            dataGridView1.Columns["Creator"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns["Mods"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns["BPM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns["ppRaw"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns["Artist"].Width = 75;
            dataGridView1.Columns["Version"].Width = 55;
            dataGridView1.Sort(dataGridView1.Columns["ppRaw"], ListSortDirection.Ascending);
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            progressBar1.Value = progressBar1.Maximum;
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show(@"No suitable maps found.");
            }
            else
            {
                dataGridView1_SelectionChanged(new object(), new EventArgs());
            }
            SaveSettings();
            UpdateButton.Text = @"Update";

        }
        private void UpdateSuggestionsAsync(double minPp, int gameMode)
        {
            int[] userids;
            int startid;
            switch (gameMode)
            {
                case 0:
                    userids = standardids;
                    startid = currentUser.PpRaw < 200 ? 12843 :
                        currentUser.PpRank < 5001 ? currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode, userids);
                    break;

                case 1:
                    userids = taikoids;
                    startid = currentUser.PpRaw < 200 ? 6806 :
                        currentUser.PpRank < 5001 ? currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode, userids);
                    break;

                case 2:
                    userids = ctbids;
                    startid = currentUser.PpRaw < 200 ? 7638 :
                        currentUser.PpRank < 5001 ? currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode, userids);
                    break;

                case 3:
                    userids = maniaids;
                    startid = currentUser.PpRaw < 200 ? 7569 :
                        currentUser.PpRank < 5001 ? currentUser.PpRank - 2 :
                        FindStartingUser(currentUser.PpRaw, gameMode, userids);
                    break;

                default:
                    return;
            }
            scoreSugDisplay = new SortableBindingList<ScoreInfo>();
            beatmapCache = new Dictionary<int, Beatmap>();
            var modsAndNv = mods | GlobalVars.Mods.NV;
            foreach (var score in currentUser.BestScores)
            {
                beatmapCache.Add(score.Beatmap_Id, null);
            }
            var pChecked = 0;
            var sw = Stopwatch.StartNew();
            Parallel.For(0, 999, (i, state) =>
            {
                while (sw.Elapsed < maxDuration)
                {
                    var json = "";
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
                        int @checked = pChecked;
                        Invoke((MethodInvoker)delegate
                        {
                            PlayersCheckedLbl.Text = @checked.ToString();
                        });
                    }
                    var userBestList = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
                    for (var j = 0; j < userBestList.Count; j++)
                    {
                        if (userBestList[j].PP < minPp)
                        {
                            break;
                        }
                        if ((!ExclusiveCB.Checked ||
                             (userBestList[j].Enabled_Mods != modsAndNv && userBestList[j].Enabled_Mods != mods)) &&
                            (ExclusiveCB.Checked || !userBestList[j].Enabled_Mods.HasFlag(mods))) continue;
                        lock (secondLock)
                        {
                            if (beatmapCache.ContainsKey(userBestList[j].Beatmap_Id)) continue;
                            var beatmap = new Beatmap(userBestList[j].Beatmap_Id);
                            var dtmodifier = 1.0;
                            if (userBestList[j].Enabled_Mods.HasFlag(GlobalVars.Mods.DT) || userBestList[j].Enabled_Mods.HasFlag(GlobalVars.Mods.NC))
                            {
                                dtmodifier = 1.5;
                            }
                            beatmapCache[beatmap.Beatmap_id] = beatmap;
                            scoreSugDisplay.Add(new ScoreInfo
                            {
                                BeatmapName = beatmap.Title,
                                Version = beatmap.Version,
                                Creator = beatmap.Creator,
                                Artist = beatmap.Artist,
                                Mods = userBestList[j].Enabled_Mods,
                                BPM = (int)Math.Truncate(beatmap.Bpm * dtmodifier),
                                ppRaw = (int)Math.Truncate(userBestList[j].PP),
                                RankImage = GetRankImage(userBestList[j].Rank),
                                BeatmapId = beatmap.Beatmap_id
                            });
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
                state.Break();
            });
        }

        private void UpdateLbl_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripLabel)) return;
            var toolStripLabel1 = sender as ToolStripLabel;
            Process.Start(toolStripLabel1.Tag.ToString());
            toolStripLabel1.LinkVisited = true;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == (int)dataGridView1.SelectedRows[0].Cells["ppRaw"].Value).Value.Url + GlobalVars.Mode + GameModeCB.SelectedIndex);
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(beatmapCache.Single(x => x.Key == (int)dataGridView1.SelectedRows[0].Cells["ppRaw"].Value).Value.Url + GlobalVars.Mode + GameModeCB.SelectedIndex);
        }
    }
}