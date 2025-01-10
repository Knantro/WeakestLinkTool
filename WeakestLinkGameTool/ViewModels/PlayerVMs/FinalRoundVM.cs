using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

public class FinalRoundVM : ViewModelBase {
    private Player firstPlayer;
    private Player secondPlayer;
    private bool turnSwitch = true; // true - ход первого игрока, false - второго
    
    public event EventHandler<bool> FinalRoundPanelVisibilityChanged;
    
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
    public ObservableCollection<FinalQuestionVisual> FirstPlayerAnswersPanel { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> SecondPlayerAnswersPanel { get; set; }

    public FinalRoundVM() {
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
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    public void SetupPlayers(Player player1, Player player2) {
        FirstPlayer = player1;
        SecondPlayer = player2;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void CorrectAnswer(int questionNumber) {
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsRight = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsWrong = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = true;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsRight = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsWrong = false;
        }
        
        turnSwitch = !turnSwitch;
    }

    /// <summary>
    /// 
    /// </summary>
    public void WrongAnswer(int questionNumber) {
        if (turnSwitch) {
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = false;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsWrong = true;
            FirstPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsRight = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = true;
        }
        else { 
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsActive = false;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsWrong = true;
            SecondPlayerAnswersPanel.First(x => x.QuestionNumber == questionNumber).IsRight = false;
        }
        
        turnSwitch = !turnSwitch;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void AddSuddenDeathQuestion(int questionNumber) {
        FirstPlayerAnswersPanel.Clear();
        SecondPlayerAnswersPanel.Clear();
        FirstPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = questionNumber, IsActive = true });
        SecondPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = questionNumber, IsActive = false });
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void ShowFinalRoundPanel() {
        FinalRoundPanelVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideFinalRoundPanel() {
        FinalRoundPanelVisibilityChanged?.Invoke(this, false);
    }
}