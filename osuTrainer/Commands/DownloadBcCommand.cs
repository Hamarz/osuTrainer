using System;
using System.Diagnostics;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class DownloadBcCommand : ICommand
    {
        private readonly ViewModelBase _viewModelBase;

        public DownloadBcCommand(ViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Process.Start(GlobalVars.BloodcatUrl + _viewModelBase.SelectedScoreInfo.BeatmapSetId);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}