using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class RegularRoundPanelVM : ViewModelBase {
    private Player currentPlayer;
    private TimeSpan timeLeft;
    private Stopwatch answerStopwatch = new();
    private bool isRoundStarted;
    private bool isRoundPlayingNow;
    private bool isRoundEnded;
    private Question currentQuestion;
    private Question followingQuestion;
    private Joke currentJoke;
    private int bank;
    private int jokesUsedCount;
    private int questionIndex;

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
    public string TimeText => timeLeft.ToString("m\\:ss");

    public string RoundEndNextButtonText { get; set; } = "Следующая подколка";

    public RelayCommand CorrectAnswerCommand { get; set; }
    public RelayCommand WrongAnswerCommand { get; set; }
    public RelayCommand BankCommand { get; set; }
    public RelayCommand MeasureTimeCommand { get; set; }
    public RelayCommand PauseRoundCommand { get; set; }
    public RelayCommand ContinueRoundCommand { get; set; }
    public RelayCommand PreviousQuestionCommand { get; set; }
    public RelayCommand NextQuestionCommand { get; set; }
    
    public RelayCommand StartRoundCommand => new(_ => StartRound());
    public RelayCommand RoundEndCommand { get; set; }

    public RegularRoundPanelVM() {
        // Реверс для отображения
        MoneyTree = new ObservableCollection<MoneyTreeNodeVisual>(WeakestLinkLogic.MoneyTree.Select(x => x.ConvertToVisual()).Reverse());
        MoneyTree.Last().IsActive = true;
        var firstElem = MoneyTree.First();
        firstElem.Width = 288;
        firstElem.Height = 108;
        firstElem.FontSize = 60;

        CorrectAnswerCommand = new RelayCommand(async _ => await MarkCorrectAnswer(), _ => IsRoundPlayingNow);
        WrongAnswerCommand = new RelayCommand(_ => MarkWrongAnswer(), _ => IsRoundPlayingNow);
        BankCommand = new RelayCommand(_ => BankMoney(), _ => IsRoundPlayingNow && MoneyTree.FirstOrDefault(x => x.InChain) != null);
        MeasureTimeCommand = new RelayCommand(_ => StartAnswerMeasuring(), _ => IsRoundPlayingNow);
        PauseRoundCommand = new RelayCommand(_ => PauseRound(), _ => IsRoundPlayingNow);
        ContinueRoundCommand = new RelayCommand(_ => ResumeRound(), _ => IsRoundPlayingNow);
        PreviousQuestionCommand = new RelayCommand(_ => PreviousQuestion(), _ => IsRoundPlayingNow && QuestionIndex > 0);
        NextQuestionCommand = new RelayCommand(_ => NextQuestion(), _ => IsRoundPlayingNow);
        
        RoundEndCommand = new RelayCommand(_ => NextJoke());
        CurrentRound = WeakestLinkLogic.NextRound();
        TimeLeft = CurrentRound.Timer!.Value;
        timer.Tick += async (_, _) => await TimerTick();
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task TimerTick() {
        TimeLeft -= TimeSpan.FromSeconds(1);
        if (TimeLeft.TotalSeconds < 1) {
            timer.Stop();
            await Task.Delay(3000);
            
            IsRoundPlayingNow = false;
            CommandManager.InvalidateRequerySuggested();
            IsRoundEnded = true;
            
            if (!WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) NextJoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartRound() {
        // TODO: Музыка
        NextPlayerQuestion();
        WeakestLinkLogic.ResetStrongestWeakestLinks();
        IsRoundStarted = true;
        IsRoundPlayingNow = true;
        StartTimer();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartTimer() {
        // TODO: Музыка
        timer.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResumeRound() {
        // TODO: Музыка
        timer.Resume();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PauseRound() {
        // TODO: Музыка
        timer.Pause();
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task MarkCorrectAnswer() {
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.CorrectAnswer(currentPlayer, answerTime);
        
        await UIDispatcherInvokeAsync(async () => {
            var chainIndex = MoneyTree.IndexOf(MoneyTree.LastOrDefault(x => x.IsActive));
            if (chainIndex != -1) {
                MoneyTree[chainIndex].InChain = true;
                await Task.Delay(200);
                MoneyTree[chainIndex].IsActive = false;
                if (chainIndex > 0) {
                    MoneyTree[chainIndex - 1].IsActive = true;
                }
            }
        });

        NextPlayerQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void MarkWrongAnswer() {
        var answerTime = StopAndGetAnswerTime();
        WeakestLinkLogic.WrongAnswer(currentPlayer, answerTime);
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

        return answerTime;
    }

    /// <summary>
    /// 
    /// </summary>
    private void BankMoney() {
        var money = MoneyTree.FirstOrDefault(x => x.InChain)?.Value ?? 0;

        if (money > 0) {
            if (Bank + money >= MoneyTree.First().Value) {
                money = MoneyTree.First().Value - Bank;
            }
            
            Bank += money;
            WeakestLinkLogic.BankMoney(currentPlayer, money);
            ResetMoneyChain();
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
            answerStopwatch.Start();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextQuestion() {
        QuestionIndex++;
        CurrentQuestion = WeakestLinkLogic.NextQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PreviousQuestion() {
        QuestionIndex--;
        CurrentQuestion = WeakestLinkLogic.PreviousQuestion();
        FollowingQuestion = WeakestLinkLogic.NextFollowingQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextJoke() {
        CurrentJoke = WeakestLinkLogic.NextJoke();
        jokesUsedCount++;
        
        if (jokesUsedCount == 3) {
            RoundEndNextButtonText= "Запустить голосование";
            RoundEndCommand = new RelayCommand(_ => EndRound());

            OnPropertyChanged(nameof(RoundEndNextButtonText));
            OnPropertyChanged(nameof(RoundEndCommand));
            
            CurrentJoke = new Joke { Text = "Один из вас должен уйти ни с чем. Пришло время определить самое слабое звено" };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndRound() {
        WeakestLinkLogic.FormRegularRoundStatistics();
        WeakestLinkLogic.SaveEditableData();
        if (WeakestLinkLogic.CurrentSession.CurrentRound.IsPreFinal) ChangeMWPage<FinalRoundInstructionPage>();
        else ChangeMWPage<VotingPanelPage>();
    }
}