using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace osuTrainer.Forms
{
    public partial class Login : Form
    {
        public User newUser;
        private bool _start;

        public Login(bool start = false)
        {
            _start = start;
           InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            UsernameTextbox.Text = Properties.Settings.Default.Username;
            if (_start)
            {
                ConfirmButton_Click(sender, e);          
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            newUser = new User();
            if (UsernameTextbox.Text != "")
            {
                if (UserParser.GetUser(UsernameTextbox.Text, newUser))
                {
                    Properties.Settings.Default.Username = UsernameTextbox.Text;
                    Properties.Settings.Default.Save();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("User not found!");
                }
            }
        }
    }
}
