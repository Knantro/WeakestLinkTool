using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана панели конца игры
/// </summary>
public class EndGamePanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private bool isFullGameStatisticsSelected;
    private bool isPersonalStatisticsSelected;
    private Player selectedPlayer;

    /// <summary>
    /// Победитель игры
    /// </summary>
    public Player Winner { get; set; }

    /// <summary>
    /// Итоговый банк
    /// </summary>
    public int TotalBank { get; set; }

    /// <summary>
    /// Первый финалист
    /// </summary>
    public Player FirstFinalist { get; set; }

    /// <summary>
    /// Второй финалист
    /// </summary>
    public Player SecondFinalist { get; set; }

    /// <summary>
    /// Ответы первого финалиста
    /// </summary>
    public List<bool> FirstFinalistAnswers { get; set; }

    /// <summary>
    /// Ответы второго финалиста
    /// </summary>
    public List<bool> SecondFinalistAnswers { get; set; }

    /// <summary>
    /// Все игроки текущей сессии
    /// </summary>
    public ObservableCollection<Player> Players { get; set; }

    /// <summary>
    /// Индивидуальная статистика игрока
    /// </summary>
    public ObservableCollection<PlayerStatistics> PersonalPlayerStatistics { get; set; }

    /// <summary>
    /// Полная статистика игры
    /// </summary>
    public ObservableCollection<FullPlayerStatistics> FullGameStatistics { get; set; }

    /// <summary>
    /// Текущий игрок
    /// </summary>
    public Player SelectedPlayer {
        get => selectedPlayer;
        set => SetField(ref selectedPlayer, value);
    }

    /// <summary>
    /// Выбрана ли опция показа полной статистики игры
    /// </summary>
    public bool IsFullGameStatisticsSelected {
        get => isFullGameStatisticsSelected;
        set => SetField(ref isFullGameStatisticsSelected, value);
    }

    /// <summary>
    /// Выбрана ли опция показа индивидуальной статистики игрока
    /// </summary>
    public bool IsPersonalStatisticsSelected {
        get => isPersonalStatisticsSelected;
        set {
            SetField(ref isPersonalStatisticsSelected, value);
            OnPropertyChanged(nameof(PersonalPlayerStatistics));
        }
    }

    public RelayCommand<Player> SelectStatisticsCommand => new(FormPersonalStatistics, _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand StartNewGameCommand => new(_ => StartNewGame(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand FullStatisticsCommand => new(_ => ShowFullGameStatistics(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand ToMenuCommand => new(_ => GoToMenu(), _ => !mainWindowViewModel.IsMessageBoxVisible);

    public EndGamePanelVM() {
        logger.SignedDebug();
        SoundManager.FadeWith(SoundName.WINNER_THEME, SoundName.CLOSING_TITLES, fadeOutMilliseconds: SoundConst.WINNER_THEME_FADE,
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
    /// Начинает новую игру с теми же игроками
    /// </summary>
    private void StartNewGame() {
        logger.SignedDebug();
        SoundManager.Stop(SoundName.CLOSING_TITLES);
        SoundManager.LoopPlay(SoundName.GENERAL_BED, SoundConst.GENERAL_BED_LOOP_POSITION_A, SoundConst.GENERAL_BED_LOOP_POSITION_B);
        WeakestLinkLogic.NewSessionSamePlayers();
        ChangeMWPage<RegularRoundPage>();
    }

    /// <summary>
    /// Возвращает в главное меню
    /// </summary>
    private void GoToMenu() {
        logger.SignedDebug();
        SoundManager.FadeWith(SoundName.CLOSING_TITLES, SoundName.CLOSING_TITLES_STING, fadeOutMilliseconds: SoundConst.CLOSING_TITLES_FADE);
        GetPlayerPageDataContext<EndGameVM>().CompleteCreditsImmediately();
        GoToMainMenu();
    }

    /// <summary>
    /// Показывает полную статистику игры
    /// </summary>
    private void ShowFullGameStatistics() {
        logger.SignedDebug();
        IsFullGameStatisticsSelected = true;
        IsPersonalStatisticsSelected = false;
    }

    /// <summary>
    /// Показывает индивидуальную статистику игрока
    /// </summary>
    /// <param name="player">Игрок, для которого формируется индивидуальная статистика</param>
    private void FormPersonalStatistics(Player player) {
        logger.Debug($"Forming personal statistics for {player.Name}");
        SelectedPlayer = player;
        IsFullGameStatisticsSelected = false;
        PersonalPlayerStatistics = new ObservableCollection<PlayerStatistics>(player.Statistics.Where(x => x.RoundName != "Финал"));
        IsPersonalStatisticsSelected = true;
    }
}