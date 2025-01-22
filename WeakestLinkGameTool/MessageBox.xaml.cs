using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool;

/// <summary>
/// Диалоговое окно
/// </summary>
public partial class MessageBox : UserControl {
    private MainWindowViewModel mainWindowViewModel = App.ServiceProvider.GetService<MainWindowViewModel>();

    public MessageBox() {
        Loaded += ForceFocus;
        Unloaded += (_, _) => { Loaded -= ForceFocus; };
        
        InitializeComponent();
    }
    
    private void ForceFocus(object sender, EventArgs args) => Focus();

    public void AddButtons(MessageBoxButton buttons) {
        switch (buttons) {
            case MessageBoxButton.OK:
                AddButton("OK", MessageBoxResult.OK, isDefault: true);
                break;
            case MessageBoxButton.OKCancel:
                AddButton("OK", MessageBoxResult.OK, isDefault: true);
                AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                break;
            case MessageBoxButton.YesNo:
                AddButton("Да", MessageBoxResult.Yes, isDefault: true);
                AddButton("Нет", MessageBoxResult.No, isCancel: true);
                break;
            // case MessageBoxButton.YesNoCancel:
            //     AddButton("Yes", MessageBoxResult.Yes, isDefault: true);
            //     AddButton("No", MessageBoxResult.No);
            //     AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true);
            //     break;
            default:
                throw new ArgumentException("Unknown button value", nameof(buttons));
        }
    }

    private void AddButton(string text, MessageBoxResult result, bool isDefault = false, bool isCancel = false) {
        var button = new Button { Content = text };
        if (isDefault) (DataContext as MessageBoxVM)!.EnterCommand = new RelayCommand(_ => RaiseDialogResult(result));
        if (isCancel) (DataContext as MessageBoxVM)!.CancelCommand = new RelayCommand(_ => RaiseDialogResult(result));
        
        button.Click += (_, _) => { RaiseDialogResult(result); };
        button.Background = Brushes.MidnightBlue;
        button.Margin = new Thickness(10, 0, 10, 0);
        button.FontSize = 35;
        button.Height = 90;
        button.Width = 230;
        button.Style = FindResource("DefaultButton") as Style;
        button.HorizontalAlignment = result switch {
            MessageBoxResult.OK => HorizontalAlignment.Right,
            MessageBoxResult.Cancel => HorizontalAlignment.Left,
            MessageBoxResult.Yes => HorizontalAlignment.Right,
            MessageBoxResult.No => HorizontalAlignment.Left,
        };

        ButtonContainer.Children.Add(button);
    }

    private void RaiseDialogResult(MessageBoxResult result) {
        Result = result;
        mainWindowViewModel.RaiseDialogResult(result);
    }

    public MessageBoxResult Result { get; set; } = MessageBoxResult.None;
}