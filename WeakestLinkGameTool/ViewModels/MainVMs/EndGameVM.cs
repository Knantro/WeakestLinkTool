using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class EndGameVM : ViewModelBase {

    private bool isFullGameStatisticsSelected;
    private bool isPersonalStatisticsSelected;
    
    /// <summary>
    /// 
    /// </summary>
    public Player Winner { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int TotalBank { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<Player> Players { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<PlayerStatistics> PersonalPlayerStatistics { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<FullPlayerStatistics> FullGameStatistics { get; set; }
    
    private Player selectedPlayer;

    /// <summary>
    /// 
    /// </summary>
    public Player SelectedPlayer {
        get => selectedPlayer;
        set => SetField(ref selectedPlayer, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsFullGameStatisticsSelected {
        get => isFullGameStatisticsSelected;
        set => SetField(ref isFullGameStatisticsSelected, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsPersonalStatisticsSelected {
        get => isPersonalStatisticsSelected;
        set => SetField(ref isPersonalStatisticsSelected, value);
    }
    
    public RelayCommand<Player> SelectStatisticsCommand => new(FormPersonalStatistics);
    public RelayCommand StartNewGameCommand => new(_ => StartNewGame());
    public RelayCommand FullStatisticsCommand => new(_ => FormFullGameStatistics());
    public RelayCommand ToMenuCommand => new(_ => GoToMainMenu());
    
    public EndGameVM() {
        Winner = WeakestLinkLogic.CurrentSession.Winner;
        TotalBank = WeakestLinkLogic.CurrentSession.FullBank;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartNewGame() {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void FormFullGameStatistics() {
        FullGameStatistics = new ObservableCollection<FullPlayerStatistics>(WeakestLinkLogic.CurrentSession.GetFullGameStatistics());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void FormPersonalStatistics(Player player) {
        SelectedPlayer = player;
        IsFullGameStatisticsSelected = false;
        IsPersonalStatisticsSelected = true;
        PersonalPlayerStatistics = new ObservableCollection<PlayerStatistics>(player.Statistics);
    }
}