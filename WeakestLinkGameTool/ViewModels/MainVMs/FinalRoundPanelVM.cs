using System.Collections.ObjectModel;
using WeakestLinkGameTool.Commands;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;
using WeakestLinkGameTool.Views.MainPages;

namespace WeakestLinkGameTool.ViewModels.MainVMs;

public class FinalRoundPanelVM : ViewModelBase {

    private Player firstPlayer;
    private Player secondPlayer;
    private Player currentPlayer;
    private bool isFinalRoundPlaying;
    private Question currentQuestion;
    private string infoText;
    private bool isTie;
    private bool isSuddenDeath;
    private bool isGameEnd;

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

    public RelayCommand<Player> StartFinalRoundCommand => new(StartFinalRound);
    public RelayCommand CorrectAnswerCommand => new(_ => CorrectAnswer());
    public RelayCommand WrongAnswerCommand => new(_ => WrongAnswer());
    public RelayCommand NextQuestionCommand => new(_ => NextQuestion());
    public RelayCommand PreviousQuestionCommand => new(_ => PreviousQuestion());
    public RelayCommand StartSuddenDeathCommand => new(_ => StartSuddenDeath());
    public RelayCommand EndGameCommand => new(_ => EndGame());

    public FinalRoundPanelVM() {
        // TODO: Музыка
        FirstPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[0];
        SecondPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[1];
        
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
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void StartFinalRound(Player player) {
        // TODO: Музыка
        if (player != FirstPlayer) {
            (FirstPlayer, SecondPlayer) = (SecondPlayer, FirstPlayer);
        }

        IsFinalRoundPlaying = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void CorrectAnswer() {
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = true;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = false;
            if (currentQuestionNumber < FirstPlayerAnswersPanel.Count) FirstPlayerAnswersPanel.First(x => x.QuestionNumber == ++currentQuestionNumber).IsActive = true;
        }

        if (!CheckWin()) {
            CheckWarning();
            CheckTie();
            if (IsTie) {
                if (IsSuddenDeath) {
                    NextUsedQuestion();
                    AddSuddenDeathQuestion();
                }
                else CurrentQuestion = null;
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
    private void WrongAnswer() {
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = true;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsWrong = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == currentQuestionNumber).IsRight = false;
            if (currentQuestionNumber < FirstPlayerAnswersPanel.Count) FirstPlayerAnswersPanel.First(x => x.QuestionNumber == ++currentQuestionNumber).IsActive = true;
        }
        
        if (!CheckWin()) {
            CheckWarning();
            CheckTie();
            if (IsTie) {
                if (IsSuddenDeath) {
                    NextUsedQuestion();
                    AddSuddenDeathQuestion();
                }
                else CurrentQuestion = null;
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
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void NextUsedQuestion() {
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void PreviousQuestion() {
        CurrentQuestion = WeakestLinkLogic.PreviousFinalQuestion();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartSuddenDeath() {
        // TODO: Музыка
        IsTie = false;
        InfoText = string.Empty;
        IsSuddenDeath = true;
        currentQuestionNumber++;

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
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckWarning() {
        InfoText = string.Empty;
        
        if (turnSwitch) {
            if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
            }
            if (FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) + SecondPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
            }
        }
        else {
            if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
            }

            if (SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) + FirstPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
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

        InfoText = $"После пяти пар вопросов счёт равный. Мы продолжаем игру до первого проигрыша. Вопросы по-прежнему будут задаваться парами.{Environment.NewLine}" +
            $"{FirstPlayer.Name} если вы правильно отвечаете на вопрос, {SecondPlayer.Name} тоже должен ответить правильно, иначе он проиграет.{Environment.NewLine}" +
            $"{FirstPlayer.Name} если вы неверно отвечаете на вопрос, а {SecondPlayer.Name} даёт правильный ответ, он выигрывает.{Environment.NewLine}" +
            $"Итак, {FirstPlayer.Name}, {SecondPlayer.Name}, играем до первого проигрыша";
    }

    /// <summary>
    /// 
    /// </summary>
    private bool CheckWin() {
        // TODO: Музыка
        
        if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) > 
            SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            FirstPlayer.IsWinner = true;
            WeakestLinkLogic.CurrentSession.Winner = FirstPlayer;
            IsGameEnd = true;
            IsFinalRoundPlaying = false;
            InfoText = $"{FirstPlayer.Name}, сегодня вы - самое сильно звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {SecondPlayer.Name}, вы уходите ни с чем";
            return true;
        }

        if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) >
            FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            SecondPlayer.IsWinner = true;
            WeakestLinkLogic.CurrentSession.Winner = SecondPlayer;
            IsGameEnd = true;
            IsFinalRoundPlaying = false;
            InfoText = $"{SecondPlayer.Name}, сегодня вы - самое сильно звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {FirstPlayer.Name}, вы уходите ни с чем";
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void EndGame() {
        WeakestLinkLogic.FormFinalRoundStatistics();
        ChangeMWPage<EndGamePage>();
    }
}