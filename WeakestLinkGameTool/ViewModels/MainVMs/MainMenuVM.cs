using System.Windows;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление главного меню
/// </summary>
public class MainMenuVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public RelayCommand NewGameCommand => new(_ => StartNewGame(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Подсоединяет к существующей сессии
    /// </summary>
    public RelayCommand ConnectToGameCommand => new(_ => throw new NotImplementedException(), _ => false);

    /// <summary>
    /// Переходит в настройки разрешения экрана
    /// </summary>
    public RelayCommand SettingsCommand => new(_ => ChangeMWPage<SettingsPage>(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Переходит в режим редактора
    /// </summary>
    public RelayCommand EditorModeCommand => new(_ => ChangeMWPage<EditorPage>(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    /// <summary>
    /// Закрывает приложение
    /// </summary>
    public RelayCommand ExitCommand => new(_ => {
        logger.Info("Shutting down application");
        Application.Current.Shutdown();
    }, _ => !mainWindowViewModel.IsMessageBoxVisible);

    public MainMenuVM() {
        logger.SignedDebug();
    }

    /// <summary>
    /// Начинает новую игру
    /// </summary>
    private void StartNewGame() {
        logger.Info("Starting new game");
        if (!WeakestLinkLogic.CanNewGame) {
            logger.Warn("Can't start new game");
            mainWindowViewModel.ShowMessageBox(
                "Невозможно начать новую игру, так как в игре недостаточно вопросов/подколок.\r\n\r\nРегулярных вопросов должно быть не меньше 100, а финальных вопросов и подколок не меньше 20.\r\n\r\nДобавьте в игру вопросы и подколки через режим редактора",
                "Ошибка");
            return;
        }

        ChangeMWPage<InputPlayersPage>();
    }
}