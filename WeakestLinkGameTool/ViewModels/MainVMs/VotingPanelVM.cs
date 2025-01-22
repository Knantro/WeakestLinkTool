using System.Collections.ObjectModel;
using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана с панелью голосования
/// </summary>
public class VotingPanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private bool isRoundStatisticsSelected;
    private bool isPersonalStatisticsSelected;
    private bool votingCanStop;
    private bool isVotingInProgress;
    private bool isVotingDone;
    private Player selectedPlayer;

    /// <summary>
    /// Номер раунда
    /// </summary>
    public int RoundNumber { get; set; }

    /// <summary>
    /// Банк раунда
    /// </summary>
    public int RoundBank { get; set; }

    /// <summary>
    /// Текущий банк игры
    /// </summary>
    public int FullBank { get; set; }

    /// <summary>
    /// Выбрана ли статистика раунда
    /// </summary>
    public bool IsRoundStatisticsSelected {
        get => isRoundStatisticsSelected;
        set => SetField(ref isRoundStatisticsSelected, value);
    }

    /// <summary>
    /// Выбрана ли индивидуальная статистика игрока
    /// </summary>
    public bool IsPersonalStatisticsSelected {
        get => isPersonalStatisticsSelected;
        set {
            SetField(ref isPersonalStatisticsSelected, value);
            OnPropertyChanged(nameof(PersonalPlayerStatistics));
        }
    }

    /// <summary>
    /// Можно ли остановить голосование
    /// </summary>
    public bool VotingCanStop {
        get => votingCanStop;
        set => SetField(ref votingCanStop, value);
    }

    /// <summary>
    /// Идёт ли сейчас голосование
    /// </summary>
    public bool IsVotingInProgress {
        get => isVotingInProgress;
        set => SetField(ref isVotingInProgress, value);
    }

    /// <summary>
    /// Завершено ли голосование
    /// </summary>
    public bool IsVotingDone {
        get => isVotingDone;
        set => SetField(ref isVotingDone, value);
    }

    /// <summary>
    /// Распределены ли все голоса между игроками
    /// </summary>
    public bool IsAllVotesGiven => Players.Sum(x => x.VotesCount) == Players.Count;

    /// <summary>
    /// Текущий игрок
    /// </summary>
    public Player SelectedPlayer {
        get => selectedPlayer;
        set => SetField(ref selectedPlayer, value);
    }

    /// <summary>
    /// Текущие игроки
    /// </summary>
    public ObservableCollection<Player> Players { get; set; } = [];

    /// <summary>
    /// Индивидуальная статистика игрока
    /// </summary>
    public ObservableCollection<PlayerStatistics> PersonalPlayerStatistics { get; set; }

    /// <summary>
    /// Статистика раунда
    /// </summary>
    public ObservableCollection<PlayerStatistics> RoundStatistics { get; set; }

    public RelayCommand<Player> SelectStatisticsCommand => new(FormPersonalStatistics, _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand<Player> DecreaseVoteCommand => new(DecreasePlayerVotes, player => player.VotesCount > 0 && !IsVotingInProgress && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand<Player> IncreaseVoteCommand => new(IncreasePlayerVotes, player => player.VotesCount < Players.Count - 1 && !IsVotingInProgress && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand<Player> KickPlayerCommand => new(KickPlayer, _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand RoundStatisticsCommand => new(_ => ShowRoundStatistics(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand StopVotingCommand => new(_ => StopVoting(), _ => IsVotingInProgress && VotingCanStop && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand DoneVotingCommand => new(_ => DoneVoting(), _ => !IsVotingDone && IsAllVotesGiven && !mainWindowViewModel.IsMessageBoxVisible);

    public VotingPanelVM() {
        logger.SignedDebug();
        RoundNumber = WeakestLinkLogic.CurrentSession.CurrentRound.Number;
        RoundBank = WeakestLinkLogic.CurrentSession.CurrentRound.BankedMoney ?? 0;
        FullBank = WeakestLinkLogic.CurrentSession.FullBank;
        RoundStatistics = WeakestLinkLogic.CurrentSession.CurrentRound.Statistics.PlayersStatistics.Values.ToObservableCollection();
        Players = WeakestLinkLogic.CurrentSession.ActivePlayers.ToObservableCollection();
        IsVotingInProgress = true;
        IsRoundStatisticsSelected = true;
        EnterCommand = StopVotingCommand;
        StartVoting();
    }

    /// <summary>
    /// Начинает голосование
    /// </summary>
    private async Task StartVoting() {
        logger.Debug("Start voting");
        SoundManager.Pause(SoundName.GENERAL_BED);
        SoundManager.Play(SoundName.GENERAL_STING);
        await Task.Delay(SoundConst.VOTING_BED_START_COOLDOWN);
        SoundManager.LoopPlay(SoundName.VOTING_BED, SoundConst.VOTING_BED_LOOP_POSITION_A, SoundConst.VOTING_BED_LOOP_POSITION_B);
        VotingCanStop = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Формирует индивидуальную статистику игрока
    /// </summary>
    /// <param name="player">Игрок, для которого формируется статистика</param>
    private void FormPersonalStatistics(Player player) {
        logger.Debug($"Form personal statistics for {player.Name}");
        SelectedPlayer = player;
        PersonalPlayerStatistics = player.Statistics.ToObservableCollection();
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
    /// Уменьшить количество голосов определённому игроку
    /// </summary>
    /// <param name="player">Игрок, которому уменьшают голоса</param>
    private void DecreasePlayerVotes(Player player) {
        logger.Debug($"Decrease player votes for {player.Name}");
        player.VotesCount--;
        OnPropertyChanged(nameof(IsAllVotesGiven));
    }

    /// <summary>
    /// Увеличить количество голосов определённому игроку
    /// </summary>
    /// <param name="player">Игрок, которому увеличивают голоса</param>
    private void IncreasePlayerVotes(Player player) {
        logger.Debug($"Increase player votes for {player.Name}");
        player.VotesCount++;
        OnPropertyChanged(nameof(IsAllVotesGiven));
    }

    /// <summary>
    /// Исключает игрока, объявляя его слабым звеном
    /// </summary>
    /// <param name="player">Игрок, объявленный слабым звеном</param>
    private void KickPlayer(Player player) {
        logger.Info($"{player.Name} is kicked");
        WeakestLinkLogic.KickPlayer(player);
        ChangeMWPage<WalkAShamePanelPage>();
    }

    /// <summary>
    /// Показывает статистику раунда
    /// </summary>
    private void ShowRoundStatistics() {
        logger.SignedDebug();
        IsRoundStatisticsSelected = true;
        IsPersonalStatisticsSelected = false;
    }

    /// <summary>
    /// Останавливает голосование
    /// </summary>
    private void StopVoting() {
        logger.Debug("Stop voting");
        SoundManager.FadeWith(SoundName.VOTING_BED, SoundName.GENERAL_STING, SoundConst.VOTING_BED_GENERAL_STING_FADE_OUT, fadeInMilliseconds: null);
        SoundManager.Resume(SoundName.GENERAL_BED);
        IsVotingInProgress = false;
        EnterCommand = DoneVotingCommand;
    }

    /// <summary>
    /// Завершает голосование
    /// </summary>
    private void DoneVoting() {
        logger.Debug("Done voting");
        SoundManager.PlayWithVolumeFade(SoundName.VOTING_STING, SoundName.GENERAL_BED, SoundConst.GENERAL_BED_FADE_VOLUME,
            SoundConst.GENERAL_BED_VOTING_STING_FADE_VOLUME_DURATION, SoundConst.GENERAL_BED_VOTING_STING_FADE_VOLUME_AWAIT_TIME);

        IsVotingDone = true;
    }
}