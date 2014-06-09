using System;
using System.Windows.Controls;
using System.Windows.Input;
using osuTrainer.ViewModels;

namespace osuTrainer.Commands
{
    internal class UpdateCommand : ICommand
    {
        private readonly ViewModelBase _viewModelBase;

        public UpdateCommand(ViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public bool CanExecute(object parameter)
        {
            return !_viewModelBase.IsWorking;
        }

        public void Execute(object parameter)
        {
            var passwordBox = (PasswordBox) parameter;
            _viewModelBase.ApiKey = passwordBox.Password;
            _viewModelBase.ClearScores();
            _viewModelBase.GetScoresAsync();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}