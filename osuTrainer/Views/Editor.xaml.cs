using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace osuTrainer.Views
{
    /// <summary>
    ///     Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        private readonly string _textfile = "rivals.txt";
        public bool Saved;
        private bool _textChanged;

        public Editor()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_textChanged)
            {
                MessageBoxResult result = MessageBox.Show("You've made some changes. Save and refresh events?", "",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    var fileStream = new FileStream(_textfile, FileMode.Create);
                    var range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Text);
                    fileStream.Close();
                    Saved = true;
                }
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            base.OnClosing(e);
        }

        private void LoadFile()
        {
            try
            {
                rtbEditor.AppendText(File.ReadAllText(_textfile));
                _textChanged = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Failed loading " + _textfile);
            }
        }

        private void Editor_OnInitialized(object sender, EventArgs e)
        {
            rtbEditor.Document.PageWidth = 1000;
            LoadFile();
        }

        private void RtbEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _textChanged = true;
        }
    }
}