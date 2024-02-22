using System;
using System.Windows.Input;

namespace GUI.Utils;

public class SimpleWpfCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public SimpleWpfCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? ReturnTrue;
    }

    public void Execute(object parameter)
    {
        if (parameter is not T t)
            throw new ArgumentException($"Parameter is not type of {typeof(T).Name}");

        _execute.Invoke(t);
    }

    public bool CanExecute(object parameter) => parameter is T t && _canExecute.Invoke(t);
    
    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    private static bool ReturnTrue(T t) => true;
}