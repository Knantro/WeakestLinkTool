using System.Collections.ObjectModel;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Visual;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока финального раунда
/// </summary>
public class FinalRoundVM : InfoVM {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private Player firstPlayer;
    private Player secondPlayer;
    private bool turnSwitch = true; // true - ход первого игрока, false - второго

    /// <summary>
    /// Событие изменения видимости панели ответов финального раунда
    /// </summary>
    public event EventHandler<bool> FinalRoundPanelVisibilityChanged;

    /// <summary>
    /// Первый финалист
    /// </summary>
    public Player FirstPlayer {
        get => firstPlayer;
        set => SetField(ref firstPlayer, value);
    }

    /// <summary>
    /// Второй финалист
    /// </summary>
    public Player SecondPlayer {
        get => secondPlayer;
        set => SetField(ref secondPlayer, value);
    }

    /// <summary>
    /// Панель ответов первого финалиста
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> FirstPlayerAnswersPanel { get; set; }

    /// <summary>
    /// Панель ответов второго финалиста
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> SecondPlayerAnswersPanel { get; set; }

    public FinalRoundVM() {
        logger.SignedDebug();
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
    /// Устанавливает порядок финалистов
    /// </summary>
    /// <param name="player1">Первый финалист</param>
    /// <param name="player2">Второй финалист</param>
    public void SetupPlayers(Player player1, Player player2) {
        logger.SignedDebug($"{player1.Name} - {player2.Name}");
        FirstPlayer = player1;
        SecondPlayer = player2;
    }

    /// <summary>
    /// Фиксирует правильный ответ финалиста
    /// </summary>
    public void CorrectAnswer(int questionNumber) {
        logger.SignedDebug($"Mark {questionNumber} by {(turnSwitch ? FirstPlayer.Name : SecondPlayer.Name)} is correct");
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
    /// Фиксирует неправильный ответ финалиста
    /// </summary>
    public void WrongAnswer(int questionNumber) {
        logger.SignedDebug($"Mark {questionNumber} by {(turnSwitch ? FirstPlayer.Name : SecondPlayer.Name)} is wrong");
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
    /// Добавляет ещё один вопрос игры "до первого проигрыша"
    /// </summary>
    public void AddSuddenDeathQuestion(int questionNumber) {
        logger.SignedDebug(questionNumber.ToString());
        FirstPlayerAnswersPanel.Clear();
        SecondPlayerAnswersPanel.Clear();
        FirstPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = questionNumber, IsActive = true });
        SecondPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = questionNumber, IsActive = false });
    }

    /// <summary>
    /// Показывает панель ответов финального раунда
    /// </summary>
    public void ShowFinalRoundPanel() {
        logger.SignedDebug();
        FinalRoundPanelVisibilityChanged?.Invoke(this, true);
    }

    /// <summary>
    /// Скрывает панель ответов финального раунда
    /// </summary>
    public void HideFinalRoundPanel() {
        logger.SignedDebug();
        FinalRoundPanelVisibilityChanged?.Invoke(this, false);
    }
}