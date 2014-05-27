using System;
using System.Windows.Forms;

namespace osuTrainerOS.Forms
{
    public partial class Login : Form
    {
        private bool changeUser;
        public string userString;

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
            switch (Properties.Settings.Default.GameMode)
            {
                case 0:
                    userString = UserStandard.UserString(UsernameTextbox.Text);
                    break;
                case 1:
                    userString = UserTaiko.UserString(UsernameTextbox.Text);
                    break;
                case 2:
                    userString = UserCtb.UserString(UsernameTextbox.Text);
                    break;
                case 3:
                    userString = UserMania.UserString(UsernameTextbox.Text);
                    break;
                default:
                    break;
            }
            if (userString.Length > 33)
            {
                Close();
                DialogResult = DialogResult.OK;
            }
            else if (userString.Length < 3)
            {
                MessageBox.Show("User not found!");
            }
            else
            {
                MessageBox.Show(userString);
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