using System.Windows.Input;

namespace MachineFlowers.ViewModels
{
    public class RelayCommand : ICommand
    {
        // Fields to store the action to execute and the function that determines canExecute state
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        // Constructor takes an Action to execute and an optional Func<bool> to determine canExecute state.
        // If canExecute is not provided, it defaults to null, meaning the command can always execute.
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Event raised when the ability of the command to execute changes.
        // It's wired to the CommandManager.RequerySuggested event to automatically query the CanExecute state.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Determines whether the command can be executed.
        // If _canExecute is null, it returns true; otherwise, it returns the result of the _canExecute function.
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        // Executes the _execute action.
        // This method is called when the command is executed.
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}