using System.Diagnostics;
using System.Windows.Input;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Commands; 

/// <summary>
/// Общий вид команды
/// </summary>
public class RelayCommand : INPCBase, ICommand {
    private Logger logger = LogManager.GetCurrentClassLogger();
    private Action<object> action;
    private Predicate<object> canExecuteFunc;

    /// <summary>
    /// Действие команды
    /// </summary>
    public Action<object> Action {
        get => action;
        set => SetField(ref action, value);
    }
    
    /// <summary>
    /// Функция проверки, может ли команда быть запущена
    /// </summary>
    public Predicate<object> CanExecuteFunc {
        get => canExecuteFunc;
        set => SetField(ref canExecuteFunc, value);
    }

    /// <summary>
    /// Событие изменения возможности активации команды
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
 
    public RelayCommand(Action<object> action, Predicate<object> canExecuteFunc = null)
    {
        Action = action;
        CanExecuteFunc = canExecuteFunc;
    }
 
    /// <summary>
    /// Проверяет, может ли команда быть запущена
    /// </summary>
    /// <param name="parameter">Параметр команды</param>
    /// <returns>True, если команда может быть запущена, иначе False</returns>
    public bool CanExecute(object parameter)
    {
        return CanExecuteFunc == null || CanExecuteFunc(parameter);
    }
 
    /// <summary>
    /// Запускает команду на исполнение
    /// </summary>
    /// <param name="parameter">Параметр команды</param>
    public void Execute(object parameter)
    {
        try
        {
            Action(parameter);
        }
        catch (Exception e)
        {
            logger.Warn(e, "Command execution failed");
        }
    }
}

/// <summary>
/// Типизированный общий вид команды 
/// </summary>
/// <typeparam name="T"></typeparam>
public class RelayCommand<T> : INPCBase, ICommand {
    private Logger logger = LogManager.GetCurrentClassLogger();
    private Action<T> action;
    private Predicate<T> canExecuteFunc;
    
    /// <summary>
    /// Действие команды
    /// </summary>
    public Action<T> Action {
        get => action;
        set => SetField(ref action, value);
    }
    
    /// <summary>
    /// Функция проверки, может ли команда быть запущена
    /// </summary>
    public Predicate<T> CanExecuteFunc {
        get => canExecuteFunc;
        set => SetField(ref canExecuteFunc, value);
    }
    
    public RelayCommand(Action<T> action, Predicate<T> canExecuteFunc = null)
    {
        Action = action;
        CanExecuteFunc = canExecuteFunc;
    }

    /// <summary>
    /// Проверяет, может ли команда быть запущена
    /// </summary>
    /// <param name="parameter">Параметр команды</param>
    /// <returns>True, если команда может быть запущена, иначе False</returns>
    public bool CanExecute(object parameter) {
        return CanExecuteFunc == null || CanExecuteFunc((T)parameter);
    }

    /// <summary>
    /// Событие изменения возможности активации команды
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Запускает команду на исполнение
    /// </summary>
    /// <param name="parameter">Параметр команды</param>
    public void Execute(object parameter) {
        try
        {
            Action?.Invoke((T)parameter);
        }
        catch (Exception e)
        {
            logger.Warn(e, "Command execution failed");
        }
    }
}