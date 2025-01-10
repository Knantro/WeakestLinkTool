using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class InputPlayersVM : ViewModelBase {
    
    /// <summary>
    /// Игроки 
    /// </summary>
    public ObservableCollection<Player> Players { get; set; } = [];
    
    public RelayCommand AddPlayerCommand => new(_ => AddPlayer(), _ => Players.Count < 11);
    public RelayCommand<Player> RemovePlayerCommand => new(RemovePlayer, _ => Players.Count > 0);
    public RelayCommand BackCommand => new(_ => BackToMenu());
    public RelayCommand StartGameCommand => new(_ => StartGame());

    public InputPlayersVM() {
        WeakestLinkLogic.InitSession();
        ChangePWPage<EmptyPage>();
    }

    /// <summary>
    /// Добавляет игрока в игру
    /// </summary>
    private void AddPlayer() {
        if (Players.Count < 11) {
            var player = new Player { Number = Players.Count + 1 };
            Players.Add(player);
            WeakestLinkLogic.CurrentSession.AllPlayers.Add(player);
        }
        else {
            mainWindowViewModel.ShowMessageBox("Игроков не может быть больше 11", "Ошибка");
        }
    }

    /// <summary>
    /// Удаляет игрока из игры
    /// </summary>
    /// <param name="player">Игрок для удаления</param>
    private void RemovePlayer(Player player) {
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
        GoToMainMenu();
    }

    /// <summary>
    /// Начинает игру
    /// </summary>
    private void StartGame() {
        if (!WeakestLinkLogic.CanStartGame) {
            mainWindowViewModel.ShowMessageBox("Игроков не может быть меньше 7 или больше 11 и у всех игроков должно быть имя", "Ошибка");
            return;
        }
        
        ChangeMWPage<IntroPanelPage>();
        ChangePWPage<IntroPage>();
    }
}