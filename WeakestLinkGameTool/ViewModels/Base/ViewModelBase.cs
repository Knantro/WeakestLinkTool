using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Logic;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.Base; 

public class ViewModelBase : INPCBase, IDisposable {
    protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
    protected static readonly WeakestLinkLogic WeakestLinkLogic = App.ServiceProvider.GetService<WeakestLinkLogic>();
    protected MainWindowViewModel mainWindowViewModel = App.ServiceProvider.GetService<MainWindowViewModel>();
    protected static Random rand = new();

    #region PlayerVMEvents



    #endregion

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
        (mainWindowViewModel.CurrentMWPage.DataContext as ViewModelBase)?.Dispose();
        mainWindowViewModel.CurrentMWPage = Activator.CreateInstance<T>();
    }
    
    /// <summary>
    /// Поменять текущую страницу окна игрока
    /// </summary>
    /// <typeparam name="T">Тип новой страницы, на которую нужно поменять</typeparam>
    protected void ChangePWPage<T>() where T : UserControl {
        (mainWindowViewModel.CurrentPWPage.DataContext as ViewModelBase)?.Dispose();
        mainWindowViewModel.CurrentPWPage = Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Возвращает текущий контекст страницы окна игрока
    /// </summary>
    /// <typeparam name="T">Тип контекста данных</typeparam>
    /// <returns>Объект контекста данных страницы игрока</returns>
    protected T GetPlayerPageDataContext<T>() where T : ViewModelBase => 
        mainWindowViewModel.CurrentPWPage.DataContext as T;

    /// <summary>
    /// Меняет текущую страницу на главное меню
    /// </summary>
    protected void GoToMainMenu() => ChangeMWPage<MainMenuPage>();
    
    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected void UIDispatcherInvokeAsync(Action action)
    {
        var dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess())
        {
            action();
        }
        else
        {
            dispatchObject.InvokeAsync(action);
        }
    }

    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected async Task UIDispatcherInvokeAsync(Func<Task> action)
    {
        var dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess())
        {
            await action();
        }
        else
        {
            await dispatchObject.InvokeAsync(action);
        }
    }

    /// <summary>
    /// Запускает указанное действие в контексте потока UI приложения синхронно
    /// </summary>
    /// <param name="action">Действие для запуска</param>
    protected void UIDispatcherInvoke(Action action)
    {
        Dispatcher dispatchObject = Application.Current.Dispatcher;
        if (dispatchObject == null || dispatchObject.CheckAccess())
        {
            action();
        }
        else
        {
            dispatchObject.InvokeAsync(action).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Освободить ресурсы
    /// </summary>
    public virtual void Dispose() {
        mainWindowViewModel.OnDialogResult -= OnDialogResult;
    }
}