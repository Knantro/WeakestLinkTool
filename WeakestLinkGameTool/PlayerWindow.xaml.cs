using System.Windows;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool;

/// <summary>
/// Окно игрока (для демонстрации экрана)
/// </summary>
public partial class PlayerWindow : Window
{
    public PlayerWindow()
    {
        InitializeComponent();
        
        ((MainWindowViewModel)DataContext).CurrentMWPage = Activator.CreateInstance<EditorPage>();
    }
}