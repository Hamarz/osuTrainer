using System;
using System.Windows.Controls;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class SearchCommand : ICommand
    {
        private readonly SearchViewModel _viewModelBase;

        public SearchCommand(SearchViewModel viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return !_viewModelBase.IsWorking;
        }

        public void Execute(object parameter)
        {
            _viewModelBase.GetScoresAsync();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}