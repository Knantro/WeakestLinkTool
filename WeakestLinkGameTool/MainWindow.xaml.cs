using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool;

/// <summary>
/// Окно ведущего
/// </summary>
public partial class MainWindow : Window {
    public MainWindowViewModel MainDataContext => App.ServiceProvider.GetService<MainWindowViewModel>();

    private PlayerWindow playerWindow = new();

    public MainWindow() {
        InitializeComponent();

        MainDataContext.CurrentMWPage = Activator.CreateInstance<MainMenuPage>();

        Deactivated += (_, _) => MainDataContext.CurrentMWPage.Focus();
        SourceInitialized += (_, _) => playerWindow.Show();
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);

        playerWindow.Close();
    }
}