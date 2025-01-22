using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.Base;

/// <summary>
/// Базовая модель-представление для всех остальных VM
/// </summary>
public class ViewModelBase : INPCBase, IDisposable {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    protected static readonly WeakestLinkLogic WeakestLinkLogic = App.ServiceProvider.GetService<WeakestLinkLogic>();
    protected MainWindowViewModel mainWindowViewModel = App.ServiceProvider.GetService<MainWindowViewModel>();
    protected static Random rand = new();

    private RelayCommand enterCommand;
    private RelayCommand cancelCommand;

    /// <summary>
    /// Команда при нажатии клавиши Enter
    /// </summary>
    public RelayCommand EnterCommand {
        get => enterCommand;
        set => SetField(ref enterCommand, value);
    }

    /// <summary>
    /// Команда при нажатии клавиши Escape
    /// </summary>
    public RelayCommand CancelCommand {
        get => cancelCommand;
        set => SetField(ref cancelCommand, value);
    }

    public ViewModelBase() {
        mainWindowViewModel.OnDialogResult += OnDialogResult;
    }

    /// <summary>
    /// Обработчик события закрытия диалогового окна
    /// </summary>
    /// <param name="result">Результат диалогового окна</param>
    protected virtual void OnDialogResult(MessageBoxResult result) { }

    /// <summary>
    /// Поменять текущую страницу главного окна
    /// </summary>
    /// <typeparam name="T">Тип новой страницы, на которую нужно поменять</typeparam>
    protected void ChangeMWPage<T>() where T : UserControl {
        logger.Debug($"Change MW Page to: {typeof(T).Name}");

        (mainWindowViewModel.CurrentMWPage.DataContext as ViewModelBase)?.Dispose();
        mainWindowViewModel.CurrentMWPage = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Поменять текущую страницу окна игрока
    /// </summary>
    /// <typeparam name="T">Тип новой страницы, на которую нужно поменять</typeparam>
    protected void ChangePWPage<T>() where T : UserControl {
        logger.Debug($"Change PW Page to: {typeof(T).Name}");

        (mainWindowViewModel.CurrentPWPage.DataContext as ViewModelBase)?.Dispose();
        mainWindowViewModel.CurrentPWPage = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Возвращает текущий контекст страницы окна игрока
    /// </summary>
    /// <typeparam name="T">Тип контекста данных</typeparam>
    /// <returns>Объект контекста данных страницы игрока</returns>
    protected T GetPlayerPageDataContext<T>() where T : ViewModelBase {
        logger.SignedDebug();
        return mainWindowViewModel.CurrentPWPage.DataContext as T;
    }

    /// <summary>
    /// Меняет текущую страницу на главное меню
    /// </summary>
    protected void GoToMainMenu() {
        logger.Debug($"Go to Main Menu");
        ChangeMWPage<MainMenuPage>();
    }

    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected void UIDispatcherInvokeAsync(Action action) {
        var dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess()) {
            action();
        }
        else {
            dispatchObject.InvokeAsync(action);
        }
    }

    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected async Task UIDispatcherInvokeAsync(Func<Task> action) {
        var dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess()) {
            await action();
        }
        else {
            await dispatchObject.InvokeAsync(action);
        }
    }

    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения синхронно
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected void UIDispatcherInvoke(Action action) {
        Dispatcher dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess()) {
            action();
        }
        else {
            dispatchObject.InvokeAsync(action).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() {
        logger.SignedDebug("Dispose");

        mainWindowViewModel.OnDialogResult -= OnDialogResult;
    }
}