using System.Windows;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class MainMenuVM : ViewModelBase {
    
    public RelayCommand NewGameCommand => new(_ => StartNewGame());

    /// <summary>
    /// Подсоединяет к существующей сессии
    /// </summary>
    public RelayCommand ConnectToGameCommand => new(_ => throw new NotImplementedException());
    
    /// <summary>
    /// Переходит в настройки разрешения экрана
    /// </summary>
    public RelayCommand SettingsCommand => new(_ => ChangeMWPage<SettingsPage>());
    
    /// <summary>
    /// Переходит в режим редактора
    /// </summary>
    public RelayCommand EditorModeCommand => new(_ => ChangeMWPage<EditorPage>());
    
    /// <summary>
    /// Закрывает приложение
    /// </summary>
    public RelayCommand ExitCommand => new(_ => Application.Current.Shutdown());
    
    /// <summary>
    /// Начинает новую игру
    /// </summary>
    private void StartNewGame() {
        if (!WeakestLinkLogic.CanNewGame) {
            mainWindowViewModel.ShowMessageBox("Невозможно начать новую игру, так как в игре недостаточно вопросов/подколок.\r\n\r\nРегулярных вопросов должно быть не меньше 100, а финальных вопросов и подколок не меньше 20.\r\n\r\nДобавьте в игру вопросы и подколки через режим редактора", "Ошибка");
            return;
        }

        ChangeMWPage<InputPlayersPage>();
    }
}