using System;
using System.Diagnostics;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class OpenBeatmapLinkCommand : ICommand
    {
        private readonly ScoresViewModel _viewModelBase;

        public OpenBeatmapLinkCommand(ScoresViewModel viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModelBase.Scores.Count > 0;
        }

        public void Execute(object parameter)
        {
            Process.Start(GlobalVars.BeatmapUrl + _viewModelBase.SelectedScoreInfo.BeatmapId + GlobalVars.Mode +
                          _viewModelBase.SelectedGameMode);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}