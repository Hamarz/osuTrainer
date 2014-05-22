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
            if (User.Exists(UsernameTextbox.Text))
            {
                newUser = new User(UsernameTextbox.Text);
                Close();
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("User not found!");
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
            }
        }
    }
}