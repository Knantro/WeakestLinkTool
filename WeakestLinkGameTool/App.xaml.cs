using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool;

/// <summary>
/// Главный класс приложения
/// </summary>
public partial class App : Application {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private object locker = new();
    private bool isFatalOccured;

    /// <summary>
    /// Настройки приложения
    /// </summary>
    public static AppSettings Settings { get; set; }

    /// <summary>
    /// Провайдер сервисов
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    public App() {
        logger.SignedInfo("Application starting");
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    /// <summary>
    /// Глобальный обработчик ошибок
    /// </summary>
    /// <param name="sender">Инициатор ошибки</param>
    /// <param name="e">Аргументы ошибки</param>
    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal(e.Exception, "Unhandled exception occured! Shutdown an application!");
                Shutdown();
            }
        }
    }

    /// <summary>
    /// Глобальный обработчик ошибок
    /// </summary>
    /// <param name="sender">Инициатор ошибки</param>
    /// <param name="e">Аргументы ошибки</param>
    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal((Exception)e.ExceptionObject, "Unhandled exception occured! Shutdown an application!");
            }
        }
    }

    /// <summary>
    /// Глобальный обработчик ошибок
    /// </summary>
    /// <param name="sender">Инициатор ошибки</param>
    /// <param name="e">Аргументы ошибки</param>
    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal(e.Exception, "Unhandled exception occured! Shutdown an application!");
            }
        }
    }

    /// <summary>
    /// Обрабатывает событие запуска приложения
    /// </summary>
    /// <param name="e">Аргументы события</param>
    protected override void OnStartup(StartupEventArgs e) {
        logger.SignedInfo();
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        Settings = FileStorage.Load<AppSettings>(FilePaths.GetFullPath(FilePaths.APPSETTINGS)) ?? new AppSettings();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Обрабатывает событие выключения приложения
    /// </summary>
    /// <param name="e">Аргументы события</param>
    protected override void OnExit(ExitEventArgs e) {
        logger.SignedInfo();
        base.OnExit(e);
        SoundManager.DisposeAll();
    }

    /// <summary>
    /// Конфигурирует основные сервисы
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    private void ConfigureServices(IServiceCollection services) {
        logger.SignedInfo();
        services.AddTransient(typeof(MainWindow));
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<WeakestLinkLogic>();
    }
}