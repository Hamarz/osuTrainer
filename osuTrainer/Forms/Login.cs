using System;
using System.Windows.Forms;

namespace osuTrainer.Forms
{
    public partial class Login : Form
    {
        private readonly bool changeUser;
        public string UserString;

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
                    UserString = UserStandard.UserString(UsernameTextbox.Text);
                    break;
                case 1:
                    UserString = UserTaiko.UserString(UsernameTextbox.Text);
                    break;
                case 2:
                    UserString = UserCtb.UserString(UsernameTextbox.Text);
                    break;
                case 3:
                    UserString = UserMania.UserString(UsernameTextbox.Text);
                    break;
                default:
                    break;
            }
            if (UserString.Length > 33)
            {
                Close();
                DialogResult = DialogResult.OK;
            }
            else if (UserString.Length < 3)
            {
                MessageBox.Show(@"User not found!");
            }
            else
            {
                MessageBox.Show(UserString);
                using (var getApiKey = new GetApiKey())
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
                    using (var getApiKey = new GetApiKey())
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