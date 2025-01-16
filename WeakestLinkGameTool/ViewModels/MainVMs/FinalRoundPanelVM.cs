using System.Collections.ObjectModel;
using System.Windows.Input;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.ViewModels.PlayerVMs;
using WeakestLinkGameTool.Views.MainPages;
using WeakestLinkGameTool.Views.PlayerPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class FinalRoundPanelVM : ViewModelBase {

    private Player firstPlayer;
    private Player secondPlayer;
    private Player currentPlayer;
    private bool isFinalRoundPlaying;
    private bool isFinalRoundStarted;
    private Question currentQuestion;
    private string infoText;
    private string endGameText;
    private bool isTie;
    private bool isSuddenDeath;
    private bool isGameEnd;
    private int questionIndex;
    private bool canEndGame;
    private FinalRoundVM playerDataContext;

    private bool turnSwitch = true; // true - ход первого игрока, false - второго
    private int currentQuestionNumber = 1;
    
    /// <summary>
    /// 
    /// </summary>
    public Player FirstPlayer {
        get => firstPlayer;
        set => SetField(ref firstPlayer, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public Player SecondPlayer {
        get => secondPlayer;
        set => SetField(ref secondPlayer, value);
    }

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
    public ObservableCollection<FinalQuestionVisual> FirstPlayerAnswersPanel { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> SecondPlayerAnswersPanel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsFinalRoundStarted {
        get => isFinalRoundStarted;
        set => SetField(ref isFinalRoundStarted, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsFinalRoundPlaying {
        get => isFinalRoundPlaying;
        set => SetField(ref isFinalRoundPlaying, value);
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
    public string InfoText {
        get => infoText;
        set => SetField(ref infoText, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public string EndGameText {
        get => endGameText;
        set => SetField(ref endGameText, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsTie {
        get => isTie;
        set => SetField(ref isTie, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSuddenDeath {
        get => isSuddenDeath;
        set => SetField(ref isSuddenDeath, value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsGameEnd {
        get => isGameEnd;
        set => SetField(ref isGameEnd, value);
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
    public bool CanEndGame {
        get => canEndGame;
        set => SetField(ref canEndGame, value);
    }

    public RelayCommand<Player> StartFinalRoundCommand => new(StartFinalRound);
    public RelayCommand CorrectAnswerCommand => new(async _ => await CorrectAnswer(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying);
    public RelayCommand WrongAnswerCommand => new(async _ => await WrongAnswer(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying);
    public RelayCommand NextQuestionCommand => new(_ => NextQuestion(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying);
    public RelayCommand PreviousQuestionCommand => new(_ => PreviousQuestion(), _ => QuestionIndex > 0 && IsFinalRoundPlaying);
    public RelayCommand StartSuddenDeathCommand => new(_ => StartSuddenDeath());
    public RelayCommand EndGameCommand => new(_ => EndGame(), _ => CanEndGame);

    public FinalRoundPanelVM() {
        SoundManager.PlayWithVolumeFade(SoundName.FINAL_BEGIN_STING, SoundName.GENERAL_BED, 0.1f, 100, 700); // TODO: Magic const
        
        WeakestLinkLogic.NextRound();
        FirstPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[0];
        SecondPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[1];

        WeakestLinkLogic.CurrentSession.FirstFinalist = FirstPlayer;
        WeakestLinkLogic.CurrentSession.SecondFinalist = SecondPlayer;
        // FirstPlayer = DesignData.Player1;
        // SecondPlayer = DesignData.Player2;
        
        InfoText = $"{(FirstPlayer.IsStrongestLink ? FirstPlayer.Name : SecondPlayer.Name)}, как сильное звено по итогам прошлого раунда, вы выбираете, кто будет первым отвечать на вопросы";
            
        FirstPlayerAnswersPanel = [
            new FinalQuestionVisual { QuestionNumber = 1, IsActive = true },
            new FinalQuestionVisual { QuestionNumber = 2, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 3, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 4, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 5, IsActive = false },
        ];
        
        SecondPlayerAnswersPanel = [
            new FinalQuestionVisual { QuestionNumber = 1, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 2, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 3, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 4, IsActive = false },
            new FinalQuestionVisual { QuestionNumber = 5, IsActive = false },
        ];
        
        ChangePWPage<FinalRoundPage>();

        playerDataContext = GetPlayerPageDataContext<FinalRoundVM>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void StartFinalRound(Player player) {
        SoundManager.FadeWith(SoundName.GENERAL_BED, SoundName.PENALTY_SHOOTOUT_BED, fadeOutMilliseconds: 200, // TODO: Magic const
            soundInPositionA: SoundConst.PENALTY_SHOOTOUT_BED_LOOP_POSITION_A, soundInPositionB: SoundConst.PENALTY_SHOOTOUT_BED_LOOP_POSITION_B);      
        
        if (player != FirstPlayer) {
            (FirstPlayer, SecondPlayer) = (SecondPlayer, FirstPlayer);
        }

        playerDataContext.SetupPlayers(FirstPlayer, SecondPlayer);

        ShowPlayerFinalRoundPanel();
        
        IsFinalRoundStarted = true;
        CurrentPlayer = FirstPlayer;
        InfoText = null;
        NextQuestion();
        QuestionIndex = 0;

        IsFinalRoundPlaying = true;
    }

    private async Task ShowPlayerFinalRoundPanel() {
        await Task.Delay(2000);
        playerDataContext.ShowFinalRoundPanel();
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task CorrectAnswer() {
        WeakestLinkLogic.CorrectAnswer(CurrentPlayer);
        
        playerDataContext.CorrectAnswer(currentQuestionNumber);
        
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = true;
            CurrentPlayer = SecondPlayer;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = false;
            if (currentQuestionNumber < FirstPlayerAnswersPanel.Count) FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber + 1).IsActive = true;
            currentQuestionNumber++;
            CurrentPlayer = FirstPlayer;
        }

        if (!CheckWin()) {
            CheckWarning();
            CheckTie();
            if (IsTie) {
                if (IsSuddenDeath) {
                    NextUsedQuestion();
                    await Task.Delay(2000);
                    AddSuddenDeathQuestion();
                }
                else {
                    CurrentQuestion = null;
                    CurrentPlayer = null;
                }
            }
            else {
                NextUsedQuestion();
            }
            turnSwitch = !turnSwitch;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task WrongAnswer() {
        WeakestLinkLogic.WrongAnswer(CurrentPlayer);
        
        playerDataContext.WrongAnswer(currentQuestionNumber);
        
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = true;
            CurrentPlayer = SecondPlayer;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = false;
            if (currentQuestionNumber < FirstPlayerAnswersPanel.Count) FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber + 1).IsActive = true;
            currentQuestionNumber++;
            CurrentPlayer = FirstPlayer;
        }
        
        if (!CheckWin()) {
            CheckWarning();
            CheckTie();
            if (IsTie) {
                if (IsSuddenDeath) {
                    NextUsedQuestion();
                    await Task.Delay(2000);
                    AddSuddenDeathQuestion();
                }
                else {
                    CurrentQuestion = null;
                    CurrentPlayer = null;
                }
            }
            else {
                NextUsedQuestion();
            }
            turnSwitch = !turnSwitch;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextQuestion() {
        QuestionIndex++;
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextUsedQuestion() {
        QuestionIndex = 0;
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PreviousQuestion() {
        QuestionIndex--;
        CurrentQuestion = WeakestLinkLogic.PreviousFinalQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartSuddenDeath() {
        SoundManager.FadeWith(SoundName.PENALTY_SHOOTOUT_BED, SoundName.SUDDEN_DEATH_BED, fadeOutMilliseconds: 200, // TODO: Magic const
            soundInPositionA: SoundConst.SUDDEN_DEATH_BED_LOOP_POSITION_A, soundInPositionB: SoundConst.SUDDEN_DEATH_BED_LOOP_POSITION_B);
        IsTie = false;
        CurrentPlayer = FirstPlayer;
        InfoText = string.Empty;
        IsSuddenDeath = true;
        NextUsedQuestion();
        AddSuddenDeathQuestion();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void AddSuddenDeathQuestion() {
        FirstPlayerAnswersPanel.Clear();
        SecondPlayerAnswersPanel.Clear();
        FirstPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = currentQuestionNumber, IsActive = true });
        SecondPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = currentQuestionNumber, IsActive = false });

        playerDataContext.AddSuddenDeathQuestion(currentQuestionNumber);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckWarning() {
        InfoText = string.Empty;

        if (!IsSuddenDeath) {
            if (turnSwitch) {
                if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                    FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                    InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
                }

                if (SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                    FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) + FirstPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                    InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
                }
            }
            else {
                if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                    SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                    InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
                }
                if (FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                    SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) + SecondPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                    InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckTie() {
        IsTie = FirstPlayerAnswersPanel.All(x => x.IsRight != null && x.IsWrong != null) 
            && SecondPlayerAnswersPanel.All(x => x.IsRight != null && x.IsWrong != null)
            && FirstPlayerAnswersPanel.Count(x => x.IsRight == true) == SecondPlayerAnswersPanel.Count(x => x.IsRight == true);

        if (IsTie && !IsSuddenDeath)
        {
            InfoText = $"После пяти пар вопросов счёт равный. Мы продолжаем игру до первого проигрыша. Вопросы по-прежнему будут задаваться парами.{Environment.NewLine}" +
                       $"{FirstPlayer.Name} если вы правильно отвечаете на вопрос, {SecondPlayer.Name} тоже должен ответить правильно, иначе он проиграет.{Environment.NewLine}" +
                       $"{FirstPlayer.Name} если вы неверно отвечаете на вопрос, а {SecondPlayer.Name} даёт правильный ответ, он выигрывает.{Environment.NewLine}" +
                       $"Итак, {FirstPlayer.Name}, {SecondPlayer.Name}, играем до первого проигрыша";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private bool CheckWin() {
        if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) > 
            SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            WinSound();
            FirstPlayer.IsWinner = true;
            IsGameEnd = true;
            IsSuddenDeath = false;
            IsFinalRoundPlaying = false;
            EndGameText = $"{FirstPlayer.Name}, сегодня вы - самое сильное звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {SecondPlayer.Name}, вы уходите ни с чем.{Environment.NewLine}Вы смотрели \"Слабое звено\", это была всего лишь игра! До встречи!";
            HidePlayerFinalRoundPanel();
            WeakestLinkLogic.SetupWinner(FirstPlayer);
            return true;
        }

        if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) >
            FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            WinSound();
            SecondPlayer.IsWinner = true;
            IsGameEnd = true;
            IsSuddenDeath = false;
            IsFinalRoundPlaying = false;
            EndGameText = $"{SecondPlayer.Name}, сегодня вы - самое сильное звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {FirstPlayer.Name}, вы уходите ни с чем.{Environment.NewLine}Вы смотрели \"Слабое звено\", это была всего лишь игра! До встречи!";
            HidePlayerFinalRoundPanel();
            WeakestLinkLogic.SetupWinner(SecondPlayer);
            return true;
        }

        return false;
    }

    private async Task HidePlayerFinalRoundPanel() {
        await Task.Delay(2000);
        playerDataContext.HideFinalRoundPanel();
        CanEndGame = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// 
    /// </summary>
    private void WinSound() {
        SoundManager.FadeWith(IsSuddenDeath ? SoundName.SUDDEN_DEATH_BED : SoundName.PENALTY_SHOOTOUT_BED, SoundName.WINNER_THEME, fadeOutMilliseconds: 1500, // TODO: Magic const
            soundInPositionA: SoundConst.WINNER_THEME_LOOP_POSITION_A, // TODO: Magic const
            soundInPositionB: SoundConst.WINNER_THEME_LOOP_POSITION_B);
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndGame() {
        WeakestLinkLogic.EndGame();
        ChangeMWPage<EndGamePanelPage>();
    }
}