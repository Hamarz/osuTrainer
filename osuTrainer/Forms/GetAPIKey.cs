using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace osuTrainer.Forms
{
    public partial class GetAPIKey : Form
    {
        private CustomWebClient client = new CustomWebClient();

        public GetAPIKey()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + textBox1.Text + "&u=1").Length < 3)
            {
                Properties.Settings.Default.APIKey = textBox1.Text;
                Properties.Settings.Default.Save();
                Close();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please provide a valid API key.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void GetAPIKey_Load(object sender, EventArgs e)
        {
            linkLabel1.Text = @"Visit https://osu.ppy.sh/p/api for your API Key";
            linkLabel1.LinkArea = new LinkArea(6, 24);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://osu.ppy.sh/p/api");
        }
    }
}