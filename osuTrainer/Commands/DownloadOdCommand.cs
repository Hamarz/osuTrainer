using System;
using System.Diagnostics;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class DownloadOdCommand : ICommand
    {
        private readonly ScoresViewModel _viewModelBase;

        public DownloadOdCommand(ScoresViewModel viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModelBase.Scores.Count > 0;
        }

        public void Execute(object parameter)
        {
            Process.Start(GlobalVars.OsuDirectURL + _viewModelBase.SelectedScoreInfo.BeatmapSetId);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}