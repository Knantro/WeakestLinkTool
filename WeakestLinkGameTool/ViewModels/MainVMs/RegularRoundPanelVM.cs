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

public class RegularRoundPanelVM : ViewModelBase {
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
    /// 
    /// </summary>
    public Round CurrentRound { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Player CurrentPlayer {
        get => currentPlayer;
        set => SetField(ref currentPlayer, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool CanStartRound {
        get => canStartRound;
        set => SetField(ref canStartRound, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundStarted {
        get => isRoundStarted;
        set => SetField(ref isRoundStarted, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundPlayingNow {
        get => isRoundPlayingNow;
        set => SetField(ref isRoundPlayingNow, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundEnded {
        get => isRoundEnded;
        set => SetField(ref isRoundEnded, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Question CurrentQuestion {
        get => currentQuestion;
        set => SetField(ref currentQuestion, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Joke CurrentJoke {
        get => currentJoke;
        set => SetField(ref currentJoke, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Question FollowingQuestion {
        get => followingQuestion;
        set => SetField(ref followingQuestion, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public int Bank {
        get => bank;
        set => SetField(ref bank, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public int QuestionIndex {
        get => questionIndex;
        set => SetField(ref questionIndex, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan TimeLeft {
        get => timeLeft;
        set {
            SetField(ref timeLeft, value);
            OnPropertyChanged(nameof(TimeText));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAnswerTimeMeasuring {
        get => isAnswerTimeMeasuring;
        set => SetField(ref isAnswerTimeMeasuring, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRoundPaused {
        get => isRoundPaused;
        set => SetField(ref isRoundPaused, value);
    }

    /// <summary>
    /// 
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

    public RelayCommand StartRoundCommand => new(_ => StartRound());
    public RelayCommand RoundEndCommand { get; set; }

    public RegularRoundPanelVM() {
        SoundManager.Play(SoundName.GENERAL_STING);

        // Реверс для отображения
        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;

        CorrectAnswerCommand = new RelayCommand(async _ => await MarkCorrectAnswer(), _ => IsRoundPlayingNow && IsAnswerTimeMeasuring);
        WrongAnswerCommand = new RelayCommand(_ => MarkWrongAnswer(), _ => IsRoundPlayingNow && IsAnswerTimeMeasuring);
        BankCommand = new RelayCommand(async _ => await BankMoney(), _ => IsRoundPlayingNow && MoneyTree.FirstOrDefault(x => x.InChain) != null);
        MeasureAnswerTimeCommand = new RelayCommand(_ => StartAnswerMeasuring(), _ => IsRoundPlayingNow && !IsAnswerTimeMeasuring && !IsRoundPaused);
        PauseRoundCommand = new RelayCommand(_ => PauseRound(), _ => IsRoundPlayingNow && !IsRoundPaused);
        ResumeRoundCommand = new RelayCommand(_ => ResumeRound(), _ => IsRoundPlayingNow && IsRoundPaused);
        PreviousQuestionCommand = new RelayCommand(_ => PreviousQuestion(), _ => IsRoundPlayingNow && QuestionIndex > 0);
        NextQuestionCommand = new RelayCommand(_ => NextQuestion(), _ => IsRoundPlayingNow);

        RoundEndCommand = new RelayCommand(_ => NextJoke());
        CurrentRound = WeakestLinkLogic.NextRound();
        TimeLeft = CurrentRound.Timer!.Value;
        timer.Tick += async (_, _) => await TimerTick();

        ChangePWPage<RegularRoundPage>();
        playerDataContext = GetPlayerPageDataContext<RegularRoundVM>();
        AwaitedShowRoundPanel();
    }

    private async Task AwaitedShowRoundPanel() {
        await Task.Delay(2000);
        playerDataContext.ShowRoundPanel();
        CanStartRound = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task TimerTick() {
        TimeLeft -= TimeSpan.FromSeconds(1);
        playerDataContext.TimerTick();

        if (TimeLeft.TotalSeconds < 1) {
            timer.Stop();
            await Task.Delay(3000);
            SoundManager.Resume(SoundName.GENERAL_BED);
            playerDataContext.HideRoundPanel();
            CompleteRound();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CompleteRound() {
        IsRoundPlayingNow = false;
        CommandManager.InvalidateRequerySuggested();
        IsRoundEnded = true;

        if (WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) {
            RoundEndNextButtonText = "К финалу";
            RoundEndCommand = new RelayCommand(_ => EndRound());

            OnPropertyChanged(nameof(RoundEndNextButtonText));
            OnPropertyChanged(nameof(RoundEndCommand));
        }
        else NextJoke();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartRound() {
        SoundManager.Play(SoundName.FromRound(CurrentRound.Timer!.Value));
        SoundManager.Pause(SoundName.GENERAL_BED);
        NextPlayerQuestion();
        WeakestLinkLogic.ResetTempPlayerParams();
        IsRoundStarted = true;
        IsRoundPlayingNow = true;
        timer.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResumeRound() {
        timer.Resume();
        IsRoundPaused = false;
        SoundManager.Play(SoundName.START_TIMER);
        SoundManager.Resume(SoundName.FromRound(CurrentRound.Timer!.Value));
    }

    /// <summary>
    /// 
    /// </summary>
    private void PauseRound() {
        timer.Pause();
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        IsRoundPaused = true;
        SoundManager.Play(SoundName.STOP_TIMER);
        SoundManager.Pause(SoundName.FromRound(CurrentRound.Timer!.Value));
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task MarkCorrectAnswer() {
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.CorrectAnswer(currentPlayer, answerTime);

        Task.Run(async () => await playerDataContext.MarkCorrectAnswer());

        var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
        if (chainIndex != -1) {
            MoneyTree[chainIndex].InChain = true;
            await Task.Delay(200);
            MoneyTree[chainIndex].IsActive = false;
            if (chainIndex > 0) {
                MoneyTree[chainIndex - 1].IsActive = true;
            }
        }

        NextPlayerQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void MarkWrongAnswer() {
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.WrongAnswer(currentPlayer, answerTime);
        playerDataContext.MarkWrongAnswer();
        ResetMoneyChain();
        NextPlayerQuestion();
    }

    /// <summary>
    ///
    /// </summary>
    private void NextPlayerQuestion() {
        QuestionIndex = 0;
        CurrentPlayer = IsRoundPlayingNow ? WeakestLinkLogic.GetNextPlayer() : WeakestLinkLogic.GetStartRoundPlayer();
        CurrentQuestion = WeakestLinkLogic.NextQuestion(true);
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// Останавливает, сбрасывает таймер и возвращает затраченное на ответ время
    /// </summary>
    private double StopAndGetAnswerTime() {
        answerStopwatch.Stop();
        var answerTime = answerStopwatch.Elapsed.TotalSeconds;
        answerStopwatch.Reset();

        IsAnswerTimeMeasuring = false;

        return answerTime;
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task BankMoney() {
        var money = MoneyTree.FirstOrDefault(x => x.InChain)?.Value ?? 0;

        if (money > 0) {
            if (Bank + money >= MoneyTree.First().Value) money = MoneyTree.First().Value - Bank;

            playerDataContext.BankMoney(money);
            Bank += money;
            WeakestLinkLogic.BankMoney(currentPlayer, money);
            ResetMoneyChain();

            if (Bank == MoneyTree.First().Value) {
                IsRoundPlayingNow = false;

                if (TimeLeft.TotalSeconds >= 1) {
                    SoundManager.Play(SoundName.TARGET_STING);
                    SoundManager.Resume(SoundName.GENERAL_BED);
                    SoundManager.Stop(SoundName.FromRound(CurrentRound.Timer!.Value));
                    timer.Stop();
                    await Task.Delay(2000);
                    playerDataContext.HideRoundPanel();
                    CompleteRound();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResetMoneyChain() {
        MoneyTree.ForEach(x => {
            x.InChain = false;
            x.IsActive = false;
        });

        MoneyTree.Last().IsActive = true;
    }

    /// <summary>
    /// Начать замер времени 
    /// </summary>
    private void StartAnswerMeasuring() {
        if (!answerStopwatch.IsRunning) {
            IsAnswerTimeMeasuring = true;
            answerStopwatch.Start();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextQuestion() {
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        QuestionIndex++;
        CurrentQuestion = WeakestLinkLogic.NextQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PreviousQuestion() {
        answerStopwatch.Reset();
        IsAnswerTimeMeasuring = false;
        QuestionIndex--;
        CurrentQuestion = WeakestLinkLogic.PreviousQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextJoke() {
        jokesUsedCount++;

        if (jokesUsedCount == 3) {
            RoundEndNextButtonText = "Запустить голосование";
            RoundEndCommand = new RelayCommand(_ => EndRound());

            OnPropertyChanged(nameof(RoundEndNextButtonText));
            OnPropertyChanged(nameof(RoundEndCommand));

            CurrentJoke = new Joke { Text = "Один из вас должен уйти ни с чем. Пришло время определить самое слабое звено" };
        }
        else {
            CurrentJoke = WeakestLinkLogic.NextJoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndRound() {
        WeakestLinkLogic.EndRegularRound();
        WeakestLinkLogic.SaveEditableData();
        if (WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) ChangeMWPage<FinalRoundInstructionPage>();
        else {
            ChangeMWPage<VotingPanelPage>();
            ChangePWPage<VotingPage>();
        }
    }
}