using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class VotingPanelVM : ViewModelBase {
    
    private bool isRoundStatisticsSelected;
    private bool isPersonalStatisticsSelected;
    private bool isVotingInProgress;
    private bool isVotingDone;
    private Player selectedPlayer;
    
    /// <summary>
    /// 
    /// </summary>
    public int RoundNumber { get; set; }
    
    /// <summary>
    ///
    /// </summary>
    public int RoundBank { get; set; }
    
    /// <summary>
    ///
    /// </summary>
    public int FullBank { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundStatisticsSelected {
        get => isRoundStatisticsSelected;
        set => SetField(ref isRoundStatisticsSelected, value);
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

    /// <summary>
    /// 
    /// </summary>
    public bool IsVotingInProgress {
        get => isVotingInProgress;
        set => SetField(ref isVotingInProgress, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsVotingDone {
        get => isVotingDone;
        set => SetField(ref isVotingDone, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsAllVotesGiven => Players.Sum(x => x.VotesCount) == Players.Count;

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
    public ObservableCollection<Player> Players { get; set; } = [];
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<PlayerStatistics> PersonalPlayerStatistics { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<PlayerStatistics> RoundStatistics { get; set; }
    
    public RelayCommand<Player> SelectStatisticsCommand => new(FormPersonalStatistics);
    public RelayCommand<Player> DecreaseVoteCommand => new(DecreasePlayerVotes, _ => !IsVotingInProgress);
    public RelayCommand<Player> IncreaseVoteCommand => new(IncreasePlayerVotes, _ => !IsVotingInProgress);
    public RelayCommand<Player> KickPlayerCommand => new(KickPlayer);
    public RelayCommand RoundStatisticsCommand => new(_ => ShowRoundStatistics());
    public RelayCommand StopVotingCommand => new(_ => StopVoting());
    public RelayCommand DoneVotingCommand => new(_ => DoneVoting());
    
    public VotingPanelVM() {
        RoundNumber = WeakestLinkLogic.CurrentSession.CurrentRound.Number;
        RoundBank = WeakestLinkLogic.CurrentSession.CurrentRound.BankedMoney ?? 0;
        FullBank = WeakestLinkLogic.CurrentSession.FullBank;
        RoundStatistics = new ObservableCollection<PlayerStatistics>(WeakestLinkLogic.CurrentSession.CurrentRound.Statistics.PlayersStatistics.Values);
        Players = new ObservableCollection<Player>(WeakestLinkLogic.CurrentSession.ActivePlayers);
        IsVotingInProgress = true;
        IsRoundStatisticsSelected = true;
        Task.Run(async () => {
            SoundManager.Pause(SoundName.GENERAL_BED);
            SoundManager.Play(SoundName.GENERAL_STING);
            await Task.Delay(2300); // TODO: Magic const
            SoundManager.LoopPlay(SoundName.VOTING_BED, SoundConst.VOTING_BED_LOOP_POSITION_A, SoundConst.VOTING_BED_LOOP_POSITION_B);
        });
        // SelectedPlayer = DesignData.Player1;
        // IsPersonalStatisticsSelected = true;
        // IsVotingDone = true;
        //
        // Players = DesignData.Players;
        //
        // IsVotingDone = false;
        //
        // PersonalPlayerStatistics = DesignData.PlayerStatistics;
        //
        // RoundStatistics = new ObservableCollection<PlayerStatistics>(DesignData.RoundStatistics.PlayersStatistics.Values);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void FormPersonalStatistics(Player player) {
        SelectedPlayer = player;
        PersonalPlayerStatistics = new ObservableCollection<PlayerStatistics>(player.Statistics);
        var totalStatistics = WeakestLinkLogic.CurrentSession.GetFullGameStatistics().First(x => x.Player == player);
        PersonalPlayerStatistics.Add(new PlayerStatistics {
            RoundName = "Итого",
            Player = player,
            AnswerSpeeds = player.Statistics.SelectMany(x => x.AnswerSpeeds).ToList(),
            CorrectAnswers = totalStatistics.CorrectAnswers,
            WrongAnswers = totalStatistics.WrongAnswers,
            BankedMoney = totalStatistics.BankedMoney,
        });
        IsRoundStatisticsSelected = false;
        IsPersonalStatisticsSelected = true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void DecreasePlayerVotes(Player player) {
        player.VotesCount--;
        OnPropertyChanged(nameof(IsAllVotesGiven));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void IncreasePlayerVotes(Player player) {
        player.VotesCount++;
        OnPropertyChanged(nameof(IsAllVotesGiven));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void KickPlayer(Player player) {
        WeakestLinkLogic.KickPlayer(player);
        ChangeMWPage<WalkAShamePanelPage>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void ShowRoundStatistics() {
        IsRoundStatisticsSelected = true;
        IsPersonalStatisticsSelected = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopVoting() {
        SoundManager.FadeWith(SoundName.VOTING_BED, SoundName.GENERAL_STING, 2000, fadeInMilliseconds: null); // TODO: Magic const
        SoundManager.Resume(SoundName.GENERAL_BED);
        IsVotingInProgress = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DoneVoting() {
        SoundManager.Play(SoundName.VOTING_STING);
        IsVotingDone = true;
    }
}