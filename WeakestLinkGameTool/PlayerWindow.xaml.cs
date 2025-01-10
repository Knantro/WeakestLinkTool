using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool;

/// <summary>
/// Окно игрока (для демонстрации экрана)
/// </summary>
public partial class PlayerWindow : Window
{
    public MainWindowViewModel MainDataContext => App.ServiceProvider.GetService<MainWindowViewModel>();
    
    public PlayerWindow()
    {
        InitializeComponent();
        
        ((MainWindowViewModel)DataContext).CurrentPWPage = Activator.CreateInstance<EmptyPage>();
    }
}