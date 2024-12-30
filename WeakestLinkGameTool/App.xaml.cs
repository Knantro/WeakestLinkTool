using System.Diagnostics;
using System.IO;
using System.Windows;
using WeakestLinkGameTool.Logic;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private object locker = new();
    private bool isFatalOccured;
    
    public static AppSettings Settings { get; set; }
    
    public static IServiceProvider ServiceProvider { get; private set; }

    public App() {
        logger.SignedInfo("Application starting");
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal(e.Exception, "Unhandled exception occured! Shutdown an application!");
            }
        }
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal((Exception)e.ExceptionObject, "Unhandled exception occured! Shutdown an application!");
            }
        }
    }

    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        lock (locker) {
            if (!isFatalOccured) {
                isFatalOccured = true;
                logger.SignedFatal(e.Exception, "Unhandled exception occured! Shutdown an application!");
            }
        }
    }
    
    protected override void OnStartup(StartupEventArgs e) {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        Settings = FileStorage.Load<AppSettings>(FilePaths.GetFullPath(FilePaths.APPSETTINGS)) ?? new AppSettings();
        
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services) {
        services.AddTransient(typeof(MainWindow));
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<WeakestLinkLogic>();
    }
}