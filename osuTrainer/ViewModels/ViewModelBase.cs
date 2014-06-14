using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace osuTrainer.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected CustomWebClient _client;
        private bool _isWorking;
        private double _minPp;
        private ObservableCollection<ScoreInfo> _scores = new ObservableCollection<ScoreInfo>();
        private int _scoresAdded;
        private string _updateContent;
        private List<int> _userScores = new List<int>();
        private string _userid;
        public ICommand OpenBeatmapLinkCommand { get; set; }
        public ICommand CopyLinkCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand DownloadOdCommand { get; set; }
        public ICommand DownloadBcCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public ObservableCollection<ScoreInfo> Scores
        {
            get { return _scores; }
            set
            {
                _scores = value;
                RaisePropertyChanged("Scores");
            }
        }

        public ScoreInfo SelectedScoreInfo { get; set; }

        public string UpdateContent
        {
            get { return _updateContent; }
            set
            {
                if (Equals(value, _updateContent))
                {
                    return;
                }
                _updateContent = value;
                RaisePropertyChanged("UpdateContent");
            }
        }

        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                if (Equals(value, _isWorking))
                {
                    return;
                }
                _isWorking = value;
                RaisePropertyChanged("IsWorking");
            }
        }

        public double MinPp
        {
            get { return _minPp; }
            set
            {
                if (Equals(value, _minPp))
                {
                    return;
                }
                _minPp = value;
                RaisePropertyChanged("MinPp");
            }
        }

        public bool IsExclusiveCbChecked { get; set; }
        public bool IsDoubletimeCbChecked { get; set; }
        public bool IsHardrockCbChecked { get; set; }
        public bool IsHiddenCbChecked { get; set; }
        public bool IsFlashlightCbChecked { get; set; }
        public bool IsFcOnlyCbChecked { get; set; }

        public int ScoresAdded
        {
            get { return _scoresAdded; }
            protected set
            {
                _scoresAdded = value;
                RaisePropertyChanged("ScoresAdded");
            }
        }

        public int SelectedGameMode { get; set; }
        public string ApiKey { get; set; }

        public string Userid
        {
            get { return _userid; }
            set
            {
                _userid = value;
                RaisePropertyChanged("Userid");
            }
        }

        public List<int> UserScores
        {
            get { return _userScores; }
            protected set { _userScores = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void GetScoresAsync()
        {
            Scores = await Task.Run(() => GetScores()).ConfigureAwait(false);
        }

        protected virtual ObservableCollection<ScoreInfo> GetScores()
        {
            return null;
        }

        public void ClearScores()
        {
            Scores.Clear();
            ScoresAdded = 0;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected string GetRankImageUri(GlobalVars.Rank rank)
        {
            switch (rank)
            {
                case GlobalVars.Rank.S:
                    return "/osuTrainer;component/Resources/S_small.png";

                case GlobalVars.Rank.A:
                    return "/osuTrainer;component/Resources/A_small.png";

                case GlobalVars.Rank.X:
                    return "/osuTrainer;component/Resources/X_small.png";

                case GlobalVars.Rank.SH:
                    return "/osuTrainer;component/Resources/SH_small.png";

                case GlobalVars.Rank.XH:
                    return "/osuTrainer;component/Resources/XH_small.png";

                case GlobalVars.Rank.B:
                    return "/osuTrainer;component/Resources/B_small.png";

                case GlobalVars.Rank.C:
                    return "/osuTrainer;component/Resources/C_small.png";

                case GlobalVars.Rank.D:
                    return "/osuTrainer;component/Resources/D_small.png";
                default:
                    return null;
            }
        }

        protected string SelectedModsToString()
        {
            string mods = "";
            if (IsDoubletimeCbChecked)
            {
                mods += "DT";
            }
            if (IsHiddenCbChecked)
            {
                mods += "HD";
            }
            if (IsHardrockCbChecked)
            {
                mods += "HR";
            }
            if (IsFlashlightCbChecked)
            {
                mods += "FL";
            }
            return mods;
        }

        protected GlobalVars.Mods SelectedModsToEnum()
        {
            var mods = GlobalVars.Mods.None;
            if (IsDoubletimeCbChecked)
            {
                mods |= GlobalVars.Mods.DT;
            }
            if (IsHiddenCbChecked)
            {
                mods |= GlobalVars.Mods.HD;
            }
            if (IsHardrockCbChecked)
            {
                mods |= GlobalVars.Mods.HR;
            }
            if (IsFlashlightCbChecked)
            {
                mods |= GlobalVars.Mods.FL;
            }
            return mods;
        }

        protected double GetAccuracy(int count50, int count100, int count300, int countmiss, int countkatu, int countgeki)
        {
            // https://osu.ppy.sh/wiki/Accuracy
            switch (SelectedGameMode)
            {
                case 0:
                    return (float)(count50 * 50 + count100 * 100 + count300 * 300) / (countmiss + count50 + count100 + count300) / 3;
                case 1:
                    return (float)(count100 * .5 + count300) / (countmiss + count100 + count300) * 100;
                case 2:
                    return (float)(count300 + count100 + count50) /
                           (count300 + count100 + count50 + countkatu + countmiss) * 100;
                case 3:
                    return (float)(count50 * 50 + count100 * 100 + countkatu * 200 + count300 * 300 + countgeki * 300) /
                           (countmiss + count50 + count100 + countkatu + count300 + countgeki) / 3;
                default:
                    return 0.0;
            }
        }
    }
}