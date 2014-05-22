using System;
using System.Windows.Forms;

namespace osuTrainer.Forms
{
    public partial class Login : Form
    {
        public User newUser;
        private bool changeUser;

        public Login(bool changeUser)
        {
            InitializeComponent();
            this.changeUser = changeUser;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            string json = User.UserString(UsernameTextbox.Text);
            if (json.Length>33)
            {
                newUser = new User(json,true);
                Close();
                DialogResult = DialogResult.OK;
            }
            else if (json.Length < 3)
            {
                MessageBox.Show("User not found!");
            }
            else
            {
                MessageBox.Show(json);
                using (GetAPIKey getApiKey = new GetAPIKey())
                {
                    if (getApiKey.ShowDialog() == DialogResult.Cancel)
                    {
                        Close();
                    }
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            UsernameTextbox.Text = Properties.Settings.Default.Username;
            if (!changeUser)
            {
                if (UsernameTextbox.Text.Length > 0)
                {
                    ConfirmButton_Click(sender,e);
                }
                else
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
        }
    }
}