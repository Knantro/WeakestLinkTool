using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool;

/// <summary>
/// Окно ведущего
/// </summary>
public partial class MainWindow : Window {

    public MainWindowViewModel MainDataContext => App.ServiceProvider.GetService<MainWindowViewModel>();
    
    public MainWindow()
    {
        InitializeComponent();
        
        MainDataContext.CurrentMWPage = Activator.CreateInstance<EndGamePage>();

        // SourceInitialized += (sender, args) => {
        //     var secondWindow = new PlayerWindow {
        //         Owner = this
        //     };
        //
        //     secondWindow.Show();
        // };
    }
}