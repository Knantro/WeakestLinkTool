﻿using System.Collections.ObjectModel;
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
    public RelayCommand<Player> DecreaseVoteCommand => new(DecreasePlayerVotes);
    public RelayCommand<Player> IncreaseVoteCommand => new(IncreasePlayerVotes);
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
        // TODO: Музыка
        IsVotingInProgress = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DoneVoting() {
        // TODO: Музыка
        IsVotingDone = true;
    }
}