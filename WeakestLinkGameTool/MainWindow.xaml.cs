using System.Windows;
using System.Windows.Input;
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

        MainDataContext.OnDialogResult += _ => MainDataContext.CurrentMWPage.Focus();
        Deactivated += (_, _) => MainDataContext.CurrentMWPage.Focus();
        SourceInitialized += (_, _) => playerWindow.Show();
    }

    protected override void OnClosed(EventArgs e) {
        base.OnClosed(e);
        
        playerWindow.Close();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key is Key.Enter or Key.Escape) {
            if (MainDataContext.IsMessageBoxVisible) {
                foreach (KeyBinding key in MainDataContext.MessageBox.InputBindings) {
                    if (key.Key == Key.Enter && e.Key == Key.Enter) {
                        key.Command.Execute(null);
                    }
                
                    if (key.Key == Key.Escape && e.Key == Key.Escape) {
                        key.Command.Execute(null);
                    }
                }
                
                e.Handled = true;
                return;
            }
            
            foreach (KeyBinding key in MainDataContext.CurrentMWPage.InputBindings) {
                if (key.Key == Key.Enter && e.Key == Key.Enter && key.Command?.CanExecute(null) == true) {
                    key.Command.Execute(null);
                }
                
                if (key.Key == Key.Escape && e.Key == Key.Escape && key.Command?.CanExecute(null) == true) {
                    key.Command.Execute(null);
                }
            }
            
            e.Handled = true;
        }
    }
}