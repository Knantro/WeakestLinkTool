using WeakestLinkGameTool.Commands;

namespace WeakestLinkGameTool.ViewModels.Base;

/// <summary>
/// Модель-представление диалогового окна
/// </summary>
public class MessageBoxVM : ViewModelBase {
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
}