using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

/// <summary>
/// Модель-представление экрана с панелью регулярного раунда
/// </summary>
public class RegularRoundPanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const int CORRECT_ANSWER_ANIMATION_DELAY = 200;
    private const int PLAYER_SHOW_PANEL_DELAY = 2000;
    private const int END_ROUND_DELAY = 3000;

    private Player currentPlayer;
    private TimeSpan timeLeft;
    private Stopwatch answerStopwatch = new();
    private bool canStartRound;
    private bool isRoundStarted;
    private bool isRoundPlayingNow;
    private bool isRoundPaused;
    private bool isRoundEnded;
    private bool isAnswerTimeMeasuring;
    private Question currentQuestion;
    private Question followingQuestion;
    private Joke currentJoke;
    private int bank;
    private int jokesUsedCount;
    private int questionIndex;
    private RegularRoundVM playerDataContext;

    private DispatcherTimerEx timer = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(1) };


    /// <summary>
    /// Денежная цепь
    /// </summary>
    public ObservableCollection<MoneyTreeNodeVisual> MoneyTree { get; set; } = [];

    /// <summary>
    /// Текущий раунд
    /// </summary>
    public Round CurrentRound { get; set; }

    /// <summary>
    /// Текущий игрок
    /// </summary>
    public Player CurrentPlayer {
        get => currentPlayer;
        set => SetField(ref currentPlayer, value);
    }

    /// <summary>
    /// Можно ли начать раунд
    /// </summary>
    public bool CanStartRound {
        get => canStartRound;
        set => SetField(ref canStartRound, value);
    }

    /// <summary>
    /// Начат ли раунд
    /// </summary>
    public bool IsRoundStarted {
        get => isRoundStarted;
        set => SetField(ref isRoundStarted, value);
    }

    /// <summary>
    /// Идёт ли раунд сейчас
    /// </summary>
    public bool IsRoundPlayingNow {
        get => isRoundPlayingNow;
        set => SetField(ref isRoundPlayingNow, value);
    }

    /// <summary>
    /// Закончился ли раунд
    /// </summary>
    public bool IsRoundEnded {
        get => isRoundEnded;
        set => SetField(ref isRoundEnded, value);
    }

    /// <summary>
    /// Текущий вопрос
    /// </summary>
    public Question CurrentQuestion {
        get => currentQuestion;
        set => SetField(ref currentQuestion, value);
    }

    /// <summary>
    /// Текущая подколка
    /// </summary>
    public Joke CurrentJoke {
        get => currentJoke;
        set => SetField(ref currentJoke, value);
    }

    /// <summary>
    /// Предстоящий вопрос
    /// </summary>
    public Question FollowingQuestion {
        get => followingQuestion;
        set => SetField(ref followingQuestion, value);
    }

    /// <summary>
    /// Текущий банк
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    /// <summary>
    /// Текущий номер вопроса для смены вопросов
    /// </summary>
    public int QuestionIndex {
        get => questionIndex;
        set => SetField(ref questionIndex, value);
    }

    /// <summary>
    /// Оставшиеся время раунда
    /// </summary>
    public TimeSpan TimeLeft {
        get => timeLeft;
        set {
            SetField(ref timeLeft, value);
            OnPropertyChanged(nameof(TimeText));
        }
    }

    /// <summary>
    /// Идёт ли измерение времени ответа
    /// </summary>
    public bool IsAnswerTimeMeasuring {
        get => isAnswerTimeMeasuring;
        set => SetField(ref isAnswerTimeMeasuring, value);
    }

    /// <summary>
    /// Остановлен ли раунд
    /// </summary>
    public bool IsRoundPaused {
        get => isRoundPaused;
        set => SetField(ref isRoundPaused, value);
    }

    /// <summary>
    /// Оставшиеся время в строковом формате
    /// </summary>
    public string TimeText => timeLeft.ToString("m\\:ss");

    public string RoundEndNextButtonText { get; set; } = "Следующая подколка";

    public RelayCommand CorrectAnswerCommand { get; set; }
    public RelayCommand WrongAnswerCommand { get; set; }
    public RelayCommand BankCommand { get; set; }
    public RelayCommand MeasureAnswerTimeCommand { get; set; }
    public RelayCommand PauseRoundCommand { get; set; }
    public RelayCommand ResumeRoundCommand { get; set; }
    public RelayCommand PreviousQuestionCommand { get; set; }
    public RelayCommand NextQuestionCommand { get; set; }
    public RelayCommand StartRoundCommand => new(_ => StartRound(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand RoundEndCommand { get; set; }

    public RegularRoundPanelVM() {
        logger.SignedDebug();
        SoundManager.PlayWithVolumeFade(SoundName.GENERAL_STING, SoundName.GENERAL_BED, SoundConst.GENERAL_BED_FADE_VOLUME,
            SoundConst.GENERAL_BED_GENERAL_STING_FADE_VOLUME_DURATION, SoundConst.GENERAL_BED_GENERAL_STING_FADE_VOLUME_AWAIT_TIME);

        // Реверс для отображения
        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;

        CorrectAnswerCommand = new RelayCommand(async _ => await MarkCorrectAnswer(), _ => IsRoundPlayingNow && IsAnswerTimeMeasuring && !mainWindowViewModel.IsMessageBoxVisible);
        WrongAnswerCommand = new RelayCommand(_ => MarkWrongAnswer(), _ => IsRoundPlayingNow && IsAnswerTimeMeasuring && !mainWindowViewModel.IsMessageBoxVisible);
        BankCommand = new RelayCommand(async _ => await BankMoney(), _ => IsRoundPlayingNow && MoneyTree.FirstOrDefault(x => x.InChain) != null && !mainWindowViewModel.IsMessageBoxVisible);
        MeasureAnswerTimeCommand = new RelayCommand(_ => StartAnswerMeasuring(), _ => IsRoundPlayingNow && !IsAnswerTimeMeasuring && !IsRoundPaused && !mainWindowViewModel.IsMessageBoxVisible);
        PauseRoundCommand = new RelayCommand(_ => PauseRound(), _ => IsRoundPlayingNow && !IsRoundPaused && !mainWindowViewModel.IsMessageBoxVisible);
        ResumeRoundCommand = new RelayCommand(_ => ResumeRound(), _ => IsRoundPlayingNow && IsRoundPaused && !mainWindowViewModel.IsMessageBoxVisible);
        PreviousQuestionCommand = new RelayCommand(_ => PreviousQuestion(), _ => IsRoundPlayingNow && QuestionIndex > 0 && !mainWindowViewModel.IsMessageBoxVisible);
        NextQuestionCommand = new RelayCommand(_ => NextQuestion(), _ => IsRoundPlayingNow && !mainWindowViewModel.IsMessageBoxVisible);

        RoundEndCommand = new RelayCommand(_ => NextJoke(), _ => !mainWindowViewModel.IsMessageBoxVisible);
        CurrentRound = WeakestLinkLogic.NextRound();
        TimeLeft = CurrentRound.Timer!.Value;
        timer.Tick += async (_, _) => await TimerTick();

        ChangePWPage<RegularRoundPage>();
        playerDataContext = GetPlayerPageDataContext<RegularRoundVM>();
        AwaitedShowRoundPanel();
    }

    /// <summary>
    /// Показывает панель раунда на экране игрока с небольшой задержкой
    /// </summary>
    private async Task AwaitedShowRoundPanel() {
        logger.SignedDebug();
        await Task.Delay(PLAYER_SHOW_PANEL_DELAY);
        playerDataContext.ShowRoundPanel();
        CanStartRound = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Обрабатывает прошедшую секунду раунда, отображая это на экраны ведущего и игрока
    /// </summary>
    private async Task TimerTick() {
        TimeLeft -= TimeSpan.FromSeconds(1);
        logger.Debug($"Time left: {TimeLeft.TotalSeconds} seconds");
        playerDataContext.TimerTick();

        if (TimeLeft.TotalSeconds < 1) {
            logger.Info("Time is up!");
            timer.Stop();
            await Task.Delay(END_ROUND_DELAY);
            SoundManager.Resume(SoundName.GENERAL_BED);
            playerDataContext.HideRoundPanel();
            CompleteRound();
        }
    }

    /// <summary>
    /// Начинает раунд
    /// </summary>
    private void StartRound() {
        logger.Info("Start round");
        SoundManager.Play(SoundName.FromRound(CurrentRound.Timer!.Value));
        SoundManager.Pause(SoundName.GENERAL_BED);
        NextPlayerQuestion();
        WeakestLinkLogic.ResetTempPlayerParams();
        IsRoundStarted = true;
        IsRoundPlayingNow = true;
        timer.Start();
    }

    /// <summary>
    /// Заканчивает раунд
    /// </summary>
    private void CompleteRound() {
        logger.Info("Complete round");
        IsRoundPlayingNow = false;
        CommandManager.InvalidateRequerySuggested();
        IsRoundEnded = true;

        if (WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) {
            logger.Info("Pre final round ended");
            RoundEndNextButtonText = "К финалу";
            RoundEndCommand = new RelayCommand(_ => EndRound(), _ => !mainWindowViewModel.IsMessageBoxVisible);

            OnPropertyChanged(nameof(RoundEndNextButtonText));
            OnPropertyChanged(nameof(RoundEndCommand));
        }
        else NextJoke();
    }

    /// <summary>
    /// Продолжает раунд после остановки
    /// </summary>
    private void ResumeRound() {
        logger.Debug("Resume round");
        timer.Resume();
        IsRoundPaused = false;
        SoundManager.Play(SoundName.START_TIMER);
        SoundManager.Resume(SoundName.FromRound(CurrentRound.Timer!.Value));
    }

    /// <summary>
    /// Останавливает раунд
    /// </summary>
    private void PauseRound() {
        logger.Debug("Pause round");
        timer.Pause();
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        IsRoundPaused = true;
        SoundManager.Play(SoundName.STOP_TIMER);
        SoundManager.Pause(SoundName.FromRound(CurrentRound.Timer!.Value));
    }

    /// <summary>
    /// Фиксирует верный ответ игрока
    /// </summary>
    private async Task MarkCorrectAnswer() {
        logger.Debug($"{CurrentPlayer.Name} give correct answer");
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.CorrectAnswer(currentPlayer, answerTime);

        Task.Run(async () => await playerDataContext.MarkCorrectAnswer());

        var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
        if (chainIndex != -1) {
            MoneyTree[chainIndex].InChain = true;
            await Task.Delay(CORRECT_ANSWER_ANIMATION_DELAY);
            MoneyTree[chainIndex].IsActive = false;
            if (chainIndex > 0) {
                MoneyTree[chainIndex - 1].IsActive = true;
            }
        }

        NextPlayerQuestion();
    }

    /// <summary>
    /// Фиксирует неверный ответ игрока
    /// </summary>
    private void MarkWrongAnswer() {
        logger.Debug($"{CurrentPlayer.Name} give wrong answer");
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.WrongAnswer(currentPlayer, answerTime);
        playerDataContext.MarkWrongAnswer();
        ResetMoneyChain();
        NextPlayerQuestion();
    }

    /// <summary>
    /// Переключает игрока и вопрос
    /// </summary>
    private void NextPlayerQuestion() {
        logger.SignedDebug();
        QuestionIndex = 0;
        CurrentPlayer = IsRoundPlayingNow ? WeakestLinkLogic.GetNextPlayer() : WeakestLinkLogic.GetStartRoundPlayer();
        CurrentQuestion = WeakestLinkLogic.NextQuestion(true);
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// Останавливает, сбрасывает таймер и возвращает затраченное на ответ время
    /// </summary>
    private double StopAndGetAnswerTime() {
        logger.SignedDebug();
        answerStopwatch.Stop();
        var answerTime = answerStopwatch.Elapsed.TotalSeconds;
        answerStopwatch.Reset();

        IsAnswerTimeMeasuring = false;

        return answerTime;
    }

    /// <summary>
    /// Сохраняет накопленные в цепочке ответов деньги в банк
    /// </summary>
    private async Task BankMoney() {
        var money = MoneyTree.FirstOrDefault(x => x.InChain)?.Value ?? 0;

        if (money > 0) {
            logger.Debug($"Bank money {money} rub");
            if (Bank + money >= MoneyTree.First().Value) money = MoneyTree.First().Value - Bank;

            playerDataContext.BankMoney(money);
            Bank += money;
            WeakestLinkLogic.BankMoney(currentPlayer, money);
            ResetMoneyChain();

            if (Bank == MoneyTree.First().Value) {
                IsRoundPlayingNow = false;
                logger.Info("Reached max bank");

                if (TimeLeft.TotalSeconds >= 1) {
                    logger.Info("Reached max bank before time is up");
                    SoundManager.Resume(SoundName.GENERAL_BED);
                    SoundManager.PlayWithVolumeFade(SoundName.TARGET_STING, SoundName.GENERAL_BED, SoundConst.GENERAL_BED_FADE_VOLUME,
                        SoundConst.GENERAL_BED_TARGET_STING_FADE_VOLUME_DURATION, SoundConst.GENERAL_BED_TARGET_STING_FADE_VOLUME_AWAIT_TIME);
                    SoundManager.Stop(SoundName.FromRound(CurrentRound.Timer!.Value));
                    timer.Stop();
                    await Task.Delay(SoundConst.TARGET_STING_AWAIT);
                    playerDataContext.HideRoundPanel();
                    CompleteRound();
                }
            }
        }
    }

    /// <summary>
    /// Сбрасывает текущую денежную цепь
    /// </summary>
    private void ResetMoneyChain() {
        logger.SignedDebug();
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }

    /// <summary>
    /// Начинает замер времени 
    /// </summary>
    private void StartAnswerMeasuring() {
        logger.SignedDebug();
        if (!answerStopwatch.IsRunning) {
            IsAnswerTimeMeasuring = true;
            answerStopwatch.Start();
        }
    }

    /// <summary>
    /// Меняет вопрос на следующий без отметки, что вопрос использован
    /// </summary>
    private void NextQuestion() {
        logger.SignedDebug();
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        QuestionIndex++;
        CurrentQuestion = WeakestLinkLogic.NextQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// Меняет вопрос на предыдущий
    /// </summary>
    private void PreviousQuestion() {
        logger.SignedDebug();
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        QuestionIndex--;
        CurrentQuestion = WeakestLinkLogic.PreviousQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// Меняет подколку на следующую
    /// </summary>
    private void NextJoke() {
        logger.SignedDebug();
        jokesUsedCount++;

        if (jokesUsedCount == 3) {
            RoundEndNextButtonText = "Запустить голосование";
            RoundEndCommand = new RelayCommand(_ => EndRound(), _ => !mainWindowViewModel.IsMessageBoxVisible);

            OnPropertyChanged(nameof(RoundEndNextButtonText));
            OnPropertyChanged(nameof(RoundEndCommand));

            CurrentJoke = new Joke { Text = "Один из вас должен уйти ни с чем. Пришло время определить самое слабое звено" };
        }
        else {
            CurrentJoke = WeakestLinkLogic.NextJoke();
        }
    }

    /// <summary>
    /// Завершает раунд
    /// </summary>
    private void EndRound() {
        logger.Info("Round is ended");
        WeakestLinkLogic.EndRegularRound();
        WeakestLinkLogic.SaveEditableData();
        if (WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) ChangeMWPage<FinalRoundInstructionPage>();
        else {
            ChangeMWPage<VotingPanelPage>();
            ChangePWPage<VotingPage>();
        }
    }
}