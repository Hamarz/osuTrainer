using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using osuTrainerOS.Properties;
using ServiceStack.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace osuTrainerOS.Forms
{
    public partial class osuTrainerOS : Form
    {
        // Key = Beatmap ID
        public Dictionary<int, Beatmap> beatmapCache;

        public IUser currentUser;
        private string osuDirectory;
        private string osuExe;
        private const int pbMax = 60;
        private const int pbMaxhalf = 30;
        private CustomWebClient client = new CustomWebClient();
        private int selectedBeatmap;
        private Object firstLock = new Object();
        private TimeSpan maxDuration;
        private GlobalVars.Mods mods;
        private SortableBindingList<ScoreInfo> scoreSugDisplay;
        private Object secondLock = new Object();
        private int skippedIds = 1;

        public osuTrainerOS()
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
            UpdateLbl.Tag = @"https://github.com/condone/osuTrainer/releases";
            UpdateLbl.LinkBehavior = LinkBehavior.AlwaysUnderline;
            UpdateLbl.Click += UpdateLbl_Click;
        }
        private void CheckUser()
        {
            using (var login = new Login(false))
            {
                if (login.ShowDialog() == DialogResult.Cancel)
                {
                    Close();
                }
                currentUser = UserFactory.GetUser(Properties.Settings.Default.GameMode);
                currentUser.GetInfo(login.userString, true);
                Properties.Settings.Default.UserId = currentUser.User_id.ToString(CultureInfo.InvariantCulture);
                Properties.Settings.Default.Username = currentUser.Username;
                Properties.Settings.Default.Save();
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
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
            LoadScores();
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
            mods = (GlobalVars.Mods)Settings.Default.Mods;
            GameModeCB.SelectedIndex = Settings.Default.GameMode;
            MinPPTB.Value = Settings.Default.SelectedMinPP;
            MinPPTextBox.Text = Convert.ToString(MinPPTB.Value);
        }

        private void MinPPTB_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(MinPPTB, MinPPTB.Value.ToString(CultureInfo.InvariantCulture));
            MinPPTextBox.Text = Convert.ToString(MinPPTB.Value);
        }

        private async void FindOsu()
        {
            osuDirectory = await FindOsuDirAsync();
            osuExe = Path.Combine(osuDirectory, "osu!.exe");
        }

        private async Task<string> FindOsuDirAsync()
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("osu!\\DefaultIcon");
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

        private string SelectedModsToString()
        {
            string mods = "";
            if (DoubletimeCB.Checked)
            {
                mods += "DT";
            }
            if (HiddenCB.Checked)
            {
                mods += "HD";
            }
            if (HardrockCB.Checked)
            {
                mods += "HR";
            }
            if (FlashlightCB.Checked)
            {
                mods += "FL";
            }
            return mods;
        }

        private void LoadScores()
        {
            var json = client.DownloadString(@"http://osustats.ezoweb.de/API/osuTrainer.php?mode=" + GameModeCB.SelectedIndex + @"&uid=" + currentUser.User_id);
            var userBest = JsonSerializer.DeserializeFromString<List<OsuStatsBest>>(json);
            foreach (var item in userBest.Where(item => currentUser.BestScores.All(e => e.Beatmap_Id != item.Beatmap_Id)))
            {
                currentUser.BestScores.Add(new UserBest
                {
                    Beatmap_Id = item.Beatmap_Id,
                    Enabled_Mods = item.Enabled_Mods, 
                    User_Id = item.Uid, PP = item.PP_Value, 
                    Rank = item.Rank
                });
            }
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

            Text = @"osu! Trainer (OsuStats) " + Assembly.GetExecutingAssembly().GetName().Version;

            LoadScores();

            FindScores(GameModeCB.SelectedIndex);

            GameModeCB.SelectedIndexChanged += GameModeCB_SelectedIndexChanged;

            UpdateCB();
        }
        private void SaveSettings()
        {
            Settings.Default.Mods = (int)mods;
            Settings.Default.Exclusive = ExclusiveCB.Checked;
            Settings.Default.GameMode = GameModeCB.SelectedIndex;
            Settings.Default.SelectedMinPP = MinPPTB.Value;
            Settings.Default.Save();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (UpdateButton.Text == @"Update")
            {
                UpdateButton.Text = @"Updating";
                ScoresAddedLbl.Text = @"0";
                int minPpTextBoxValue = Convert.ToInt32(MinPPTextBox.Text);
                if (minPpTextBoxValue < 1)
                {
                    MinPPTB.Value = 1;
                }
                else if (minPpTextBoxValue > 400)
                {
                    MinPPTB.Value = 400;
                }
                else
                {
                    MinPPTB.Value = minPpTextBoxValue;
                }
                FindScores(GameModeCB.SelectedIndex);
            }
            else
            {
                MessageBox.Show(@"Already updating!");
            }
        }

        private void UpdateCB()
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
        private async void FindScores(int gameMode)
        {
            UpdateButton.Text = @"Updating";
            var minPp = Convert.ToDouble(MinPPTextBox.Text);
            progressBar1.Value = progressBar1.Minimum + 2;
            dataGridView1.DataSource = null;
            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            await Task.Factory.StartNew(() => UpdateSuggestionsAsync(minPp, gameMode));
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
            scoreSugDisplay = new SortableBindingList<ScoreInfo>();
            beatmapCache = new Dictionary<int, Beatmap>();
            for (int index = 0; index < currentUser.BestScores.Count; index++)
            {
                var score = currentUser.BestScores[index];
                beatmapCache.Add(score.Beatmap_Id, null);
            }
            string statsjson = "";
            try
            {
                statsjson = client.DownloadString(@"http://osustats.ezoweb.de/API/osuTrainer.php?mode=" + gameMode + @"&pp_value=" + (int)minPp + @"&mod_only_selected=" + ExclusiveCB.Checked.ToString().ToLower() + @"&mod_string=" + SelectedModsToString());
            }
            catch (Exception)
            {
                return;
            }           
            if (statsjson.Length < 3) return;
            var osuStatsScores = JsonSerializer.DeserializeFromString<List<OsuStatsScores>>(statsjson);
            osuStatsScores =
                osuStatsScores.GroupBy(e => new { e.Beatmap_Id, e.Enabled_Mods }).Select(g => g.First()).ToList();
            for (int i = 0; i < osuStatsScores.Count; i++)
            {
                if (beatmapCache.ContainsKey((osuStatsScores[i].Beatmap_Id))) return;
                var dtmodifier = 1.0;
                var beatmap = new Beatmap
                {
                    Beatmap_id = osuStatsScores[i].Beatmap_Id,
                    BeatmapSet_id = osuStatsScores[i].Beatmap_SetId,
                    Total_length = osuStatsScores[i].Beatmap_Total_Length,
                    Hit_length = osuStatsScores[i].Beatmap_Hit_Length,
                    Version = osuStatsScores[i].Beatmap_Version,
                    Artist = osuStatsScores[i].Beatmap_Artist,
                    Title = osuStatsScores[i].Beatmap_Title,
                    Creator = osuStatsScores[i].Beatmap_Creator,
                    Bpm = osuStatsScores[i].Beatmap_Bpm,
                    Difficultyrating = osuStatsScores[i].Beatmap_Diffrating,
                    Url = GlobalVars.Beatmap + osuStatsScores[i].Beatmap_Id,
                    BloodcatUrl = GlobalVars.Bloodcat + osuStatsScores[i].Beatmap_SetId,
                    ThumbnailUrl = @"http://b.ppy.sh/thumb/" + osuStatsScores[i].Beatmap_SetId + @"l.jpg"
                };
                beatmapCache[osuStatsScores[i].Beatmap_Id] = beatmap;
                if (osuStatsScores[i].Enabled_Mods.HasFlag(GlobalVars.Mods.DT) || osuStatsScores[i].Enabled_Mods.HasFlag(GlobalVars.Mods.NC))
                {
                    dtmodifier = 1.5;
                }
                scoreSugDisplay.Add(new ScoreInfo
                {
                    Mods = (osuStatsScores[i].Enabled_Mods & ~GlobalVars.Mods.Autoplay),
                    BeatmapName = osuStatsScores[i].Beatmap_Title,
                    Version = osuStatsScores[i].Beatmap_Version,
                    Creator = osuStatsScores[i].Beatmap_Creator,
                    Artist = osuStatsScores[i].Beatmap_Artist,
                    BPM = (int)Math.Truncate(beatmap.Bpm * dtmodifier),
                    ppRaw = (int)Math.Truncate(osuStatsScores[i].PP_Value),
                    RankImage = GetRankImage(osuStatsScores[i].Rank),
                    BeatmapId = osuStatsScores[i].Beatmap_Id
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

        private void UpdateLbl_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripLabel)) return;
            var toolStripLabel1 = sender as ToolStripLabel;
            Process.Start(toolStripLabel1.Tag.ToString());
            toolStripLabel1.LinkVisited = true;
        }
    }
}