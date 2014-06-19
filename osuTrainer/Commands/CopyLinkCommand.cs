using System;
using System.Windows;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class CopyLinkCommand : ICommand
    {
        private readonly ScoresViewModel _viewModelBase;

        public CopyLinkCommand(ScoresViewModel viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModelBase.Scores.Count > 0;
        }

        public void Execute(object parameter)
        {
            Clipboard.SetText(GlobalVars.BeatmapUrl + _viewModelBase.SelectedScoreInfo.BeatmapId + GlobalVars.Mode +
                              _viewModelBase.SelectedGameMode);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}