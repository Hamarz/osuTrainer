using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osuTrainer.Forms
{
    public partial class OsuTrainer : Form
    {
        public User currentUser;
        public Beatmap currentBeatmap = new Beatmap();
        // Key = Beatmap Id
        public Dictionary<int, ScoreInfo> existingScores;
        public Dictionary<int, ScoreInfo> scoreSuggestions;
        SortableBindingList<ScoreInfo> sortableScores;
        int minSuggestions = 20;
        int maxpp = 9001;

        public OsuTrainer()
        {
            InitializeComponent();
            this.Location = new Point(Screen.GetWorkingArea(this).Right - Convert.ToInt32(Size.Width * 1.5),
                          Screen.GetWorkingArea(this).Bottom - Convert.ToInt32(Size.Height * 1.3));
        }

        private void OsuTrainer_Load(object sender, EventArgs e)
        {
            if (!UpdateUser(true))
            {
                Close();
            }
            MinppTextbox.Text = Convert.ToString(Properties.Settings.Default.Minpp);
        }



        private void ChangeUserButton_Click(object sender, EventArgs e)
        {
            UpdateUser();
        }

        private bool UpdateUser(bool start = false)
        {
            Forms.Login login = new Forms.Login(start);
            if (login.ShowDialog() == DialogResult.OK)
            {
                if (currentUser == null || currentUser.Username != login.newUser.Username)
                {
                    currentUser = login.newUser;
                    existingScores = new Dictionary<int, ScoreInfo>();
                    ScoreParser.GetTopScores(currentUser);
                    for (int i = 0; i < currentUser.TopScores.Count; i++)
                    {
                        existingScores.Add(currentUser.TopScores[i].BeatmapId, currentUser.TopScores[i]);
                    }
                    UpdateSuggestions();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                BeatmapParser.GetBeatmapFromApi(currentBeatmap, Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[2].Value));
                BeatmapIdLbl.Text = Convert.ToString(currentBeatmap.Beatmap_id);
                ArtistLbl.Text = currentBeatmap.Artist;
                TitleLbl.Text = currentBeatmap.Title;
                CreatorLbl.Text = currentBeatmap.Creator;
                TotalTimeLbl.Text = TimeSpan.FromSeconds(currentBeatmap.Total_length).ToString(@"mm\:ss");
                DrainingTimeLbl.Text = TimeSpan.FromSeconds(currentBeatmap.Hit_length).ToString(@"mm\:ss");
                BpmLbl.Text = currentBeatmap.Bpm.ToString("F2");
                pictureBox1.ImageLocation = currentBeatmap.ThumbnailUrl;
            }
        }

        private void UpdateRankimages()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                switch ((GlobalVars.RankImage)dataGridView1.Rows[i].Cells[5].Value)
                {
                    case GlobalVars.RankImage.S_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.S_small;
                        break;
                    case GlobalVars.RankImage.A_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.A_small;
                        break;
                    case GlobalVars.RankImage.X_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.X_small;
                        break;
                    case GlobalVars.RankImage.SH_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.SH_small;
                        break;
                    case GlobalVars.RankImage.XH_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.XH_small;
                        break;
                    case GlobalVars.RankImage.B_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.B_small;
                        break;
                    case GlobalVars.RankImage.C_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.C_small;
                        break;
                    case GlobalVars.RankImage.D_small:
                        dataGridView1.Rows[i].Cells[0].Value = Properties.Resources.D_small;
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateSuggestions()
        {
            if (Properties.Settings.Default.Minpp == 0)
            {
                Properties.Settings.Default.Minpp = Convert.ToInt32(existingScores.First().Value.ppRaw * .9);
                MinppTextbox.Text = Convert.ToString(Properties.Settings.Default.Minpp);
                Properties.Settings.Default.Save();
            }
            scoreSuggestions = ScoreParser.GetScoreSuggestions(existingScores, currentUser, Properties.Settings.Default.Minpp, maxpp, minSuggestions);
            sortableScores = new SortableBindingList<ScoreInfo>();
            sortableScores.Load(scoreSuggestions.Values);
            dataGridView1.DataSource = sortableScores;

            if (dataGridView1.Columns.Count < 6)
            {
                dataGridView1.Columns.Insert(0, new DataGridViewImageColumn());
            }
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Descending);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            UpdateRankimages();
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows[1].Selected = true;
            }
            else
            {
                MessageBox.Show("No suitable maps found.");
            }
        }

        private void MinppTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Minpp = Convert.ToInt32(MinppTextbox.Text);
            Properties.Settings.Default.Save();
            UpdateSuggestions();
        }
    }
}
