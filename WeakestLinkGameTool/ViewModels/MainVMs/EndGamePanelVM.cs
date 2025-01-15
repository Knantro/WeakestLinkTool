using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class EndGamePanelVM : ViewModelBase {

    private bool isFullGameStatisticsSelected;
    private bool isPersonalStatisticsSelected;
    private Player selectedPlayer;
    
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
    public Player FirstFinalist { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Player SecondFinalist { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public List<bool> FirstFinalistAnswers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public List<bool> SecondFinalistAnswers { get; set; }
    
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
        set {
            SetField(ref isPersonalStatisticsSelected, value);
            OnPropertyChanged(nameof(PersonalPlayerStatistics));
        }
    }
    
    public RelayCommand<Player> SelectStatisticsCommand => new(FormPersonalStatistics);
    public RelayCommand StartNewGameCommand => new(_ => StartNewGame());
    public RelayCommand FullStatisticsCommand => new(_ => ShowFullGameStatistics());
    public RelayCommand ToMenuCommand => new(_ => GoToMenu());

    public EndGamePanelVM() {
        SoundManager.FadeWith(SoundName.WINNER_THEME, SoundName.CLOSING_TITLES, fadeOutMilliseconds: 500, // TODO: Magic const
            soundInPositionA: SoundConst.CLOSING_TITLES_LOOP_POSITION_A, soundInPositionB: SoundConst.CLOSING_TITLES_LOOP_POSITION_B);
        
        Winner = WeakestLinkLogic.CurrentSession.Winner;
        FirstFinalist = WeakestLinkLogic.CurrentSession.FirstFinalist;
        FirstFinalistAnswers = WeakestLinkLogic.CurrentSession.CurrentRound.Statistics.PlayersStatistics[FirstFinalist].FinalRoundAnswers;
        SecondFinalist = WeakestLinkLogic.CurrentSession.SecondFinalist;
        SecondFinalistAnswers = WeakestLinkLogic.CurrentSession.CurrentRound.Statistics.PlayersStatistics[SecondFinalist].FinalRoundAnswers;
        TotalBank = WeakestLinkLogic.CurrentSession.FullBank;
        Players = new ObservableCollection<Player>(WeakestLinkLogic.CurrentSession.AllPlayers);
        FullGameStatistics = new ObservableCollection<FullPlayerStatistics>(WeakestLinkLogic.CurrentSession.GetFullGameStatistics());
        IsFullGameStatisticsSelected = true;
        
        ChangePWPage<EndGamePage>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartNewGame() {
        SoundManager.Stop(SoundName.CLOSING_TITLES);
        WeakestLinkLogic.NewSessionSamePlayers();
        ChangeMWPage<RegularRoundPage>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void GoToMenu() {
        SoundManager.FadeWith(SoundName.CLOSING_TITLES, SoundName.CLOSING_TITLES_STING, fadeOutMilliseconds: SoundConst.CLOSING_TITLES_FADE);
        GetPlayerPageDataContext<EndGameVM>().CompleteCreditsImmediately();
        GoToMainMenu();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowFullGameStatistics() {
        IsFullGameStatisticsSelected = true;
        IsPersonalStatisticsSelected = false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void FormPersonalStatistics(Player player) {
        SelectedPlayer = player;
        var skip = player == FirstFinalist || player == SecondFinalist ? 1 : 0;
        IsFullGameStatisticsSelected = false;
        PersonalPlayerStatistics = new ObservableCollection<PlayerStatistics>(player.Statistics.SkipLast(skip));
        IsPersonalStatisticsSelected = true;
    }
}