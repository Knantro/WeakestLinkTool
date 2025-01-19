using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана с вводом имён игроков
/// </summary>
public class InputPlayersVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const int MAX_PLAYERS_COUNT = 11;

    /// <summary>
    /// Игроки 
    /// </summary>
    public ObservableCollection<Player> Players { get; set; } = [];

    public RelayCommand AddPlayerCommand => new(_ => AddPlayer(), _ => Players.Count < MAX_PLAYERS_COUNT && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand<Player> RemovePlayerCommand => new(RemovePlayer, _ => Players.Count > 0 && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand BackCommand => new(_ => BackToMenu(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand StartGameCommand => new(_ => StartGame(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    public InputPlayersVM() {
        logger.SignedDebug();
        WeakestLinkLogic.InitSession();
        ChangePWPage<EmptyPage>();
    }

    /// <summary>
    /// Добавляет игрока в игру
    /// </summary>
    private void AddPlayer() {
        logger.Debug("Adding player");
        if (Players.Count < MAX_PLAYERS_COUNT) {
            var player = new Player { Number = Players.Count + 1 };
            Players.Add(player);
            WeakestLinkLogic.CurrentSession.AllPlayers.Add(player);
            logger.Debug("Player added");
        }
        else {
            logger.Warn("Players count limit reached");
            mainWindowViewModel.ShowMessageBox("Игроков не может быть больше 11", "Ошибка");
        }
    }

    /// <summary>
    /// Удаляет игрока из игры
    /// </summary>
    /// <param name="player">Игрок для удаления</param>
    private void RemovePlayer(Player player) {
        logger.Debug($"Removing player {(player == null ? string.Empty : player.Number)}");
        if (player is not null) {
            Players.Remove(player);

            // Удалили с рандомного места, нужно переназначить номера игроков снова
            for (var i = 0; i < Players.Count; i++) {
                Players[i].Number = i + 1;
            }
        }
        else {
            player = Players.Last();
            Players.Remove(player);
        }

        WeakestLinkLogic.CurrentSession.AllPlayers.Remove(player);
    }

    /// <summary>
    /// Возвращает в главное меню
    /// </summary>
    private void BackToMenu() {
        logger.SignedDebug();
        GoToMainMenu();
    }

    /// <summary>
    /// Начинает игру
    /// </summary>
    private void StartGame() {
        logger.Debug("Starting game");
        if (!WeakestLinkLogic.CanStartGame) {
            mainWindowViewModel.ShowMessageBox("Игроков не может быть меньше 7 или больше 11 и у всех игроков должно быть имя", "Ошибка");
            return;
        }

        WeakestLinkLogic.StartGame();
        ChangeMWPage<IntroPanelPage>();
        ChangePWPage<IntroPage>();
    }
}