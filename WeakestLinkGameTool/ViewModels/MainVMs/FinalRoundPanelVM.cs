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

/// <summary>
/// Модель-представление экрана с панелью финального раунда
/// </summary>
public class FinalRoundPanelVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private const int SHOW_HIDE_PLAYER_FINAL_ROUND_PANEL_DELAY = 2000;
    private const int SUDDEN_DEATH_QUESTION_TIE_DELAY = 2000;

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
    /// Текущий игрок
    /// </summary>
    public Player CurrentPlayer {
        get => currentPlayer;
        set => SetField(ref currentPlayer, value);
    }

    /// <summary>
    /// Панель ответов первого финалиста
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> FirstPlayerAnswersPanel { get; set; }

    /// <summary>
    /// Панель ответов второго финалиста
    /// </summary>
    public ObservableCollection<FinalQuestionVisual> SecondPlayerAnswersPanel { get; set; }

    /// <summary>
    /// Начат ли финальный раунд
    /// </summary>
    public bool IsFinalRoundStarted {
        get => isFinalRoundStarted;
        set => SetField(ref isFinalRoundStarted, value);
    }

    /// <summary>
    /// Идёт ли сейчас финальный раунд
    /// </summary>
    public bool IsFinalRoundPlaying {
        get => isFinalRoundPlaying;
        set => SetField(ref isFinalRoundPlaying, value);
    }

    /// <summary>
    /// Текущий вопрос
    /// </summary>
    public Question CurrentQuestion {
        get => currentQuestion;
        set => SetField(ref currentQuestion, value);
    }

    /// <summary>
    /// Информационный текст
    /// </summary>
    public string InfoText {
        get => infoText;
        set => SetField(ref infoText, value);
    }

    /// <summary>
    /// Текст конца игры
    /// </summary>
    public string EndGameText {
        get => endGameText;
        set => SetField(ref endGameText, value);
    }

    /// <summary>
    /// Есть ли сейчас ничья после 5 пар вопросов
    /// </summary>
    public bool IsTie {
        get => isTie;
        set => SetField(ref isTie, value);
    }

    /// <summary>
    /// Идёт ли сейчас игра "до первого проигрыша"
    /// </summary>
    public bool IsSuddenDeath {
        get => isSuddenDeath;
        set => SetField(ref isSuddenDeath, value);
    }

    /// <summary>
    /// Закончилась ли игра
    /// </summary>
    public bool IsGameEnd {
        get => isGameEnd;
        set => SetField(ref isGameEnd, value);
    }

    /// <summary>
    /// Текущий номер вопроса для смены вопросов
    /// </summary>
    public int QuestionIndex {
        get => questionIndex;
        set => SetField(ref questionIndex, value);
    }

    /// <summary>
    /// Можно ли закончить игру
    /// </summary>
    public bool CanEndGame {
        get => canEndGame;
        set => SetField(ref canEndGame, value);
    }

    public RelayCommand<Player> StartFinalRoundCommand => new(StartFinalRound, _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand CorrectAnswerCommand => new(async _ => await CorrectAnswer(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand WrongAnswerCommand => new(async _ => await WrongAnswer(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand NextQuestionCommand => new(_ => NextQuestion(), _ => (!IsTie || IsSuddenDeath) && IsFinalRoundPlaying && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand PreviousQuestionCommand => new(_ => PreviousQuestion(), _ => QuestionIndex > 0 && IsFinalRoundPlaying && !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand StartSuddenDeathCommand => new(_ => StartSuddenDeath(), _ => !mainWindowViewModel.IsMessageBoxVisible);
    public RelayCommand EndGameCommand => new(_ => EndGame(), _ => CanEndGame && !mainWindowViewModel.IsMessageBoxVisible);

    public FinalRoundPanelVM() {
        logger.SignedDebug();
        SoundManager.PlayWithVolumeFade(SoundName.FINAL_BEGIN_STING, SoundName.GENERAL_BED, SoundConst.GENERAL_BED_FADE_VOLUME,
            SoundConst.GENERAL_BED_FINAL_ROUND_FADE_VOLUME_DURATION, SoundConst.GENERAL_BED_FINAL_ROUND_FADE_VOLUME_AWAIT_TIME);

        WeakestLinkLogic.NextRound();
        FirstPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[0];
        SecondPlayer = WeakestLinkLogic.CurrentSession.ActivePlayers[1];

        WeakestLinkLogic.CurrentSession.FirstFinalist = FirstPlayer;
        WeakestLinkLogic.CurrentSession.SecondFinalist = SecondPlayer;

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
    /// Начинает финальный раунд
    /// </summary>
    /// <param name="player">Игрок, который отвечает первым</param>
    private void StartFinalRound(Player player) {
        logger.Info("Starting final round");
        SoundManager.FadeWith(SoundName.GENERAL_BED, SoundName.PENALTY_SHOOTOUT_BED, fadeOutMilliseconds: SoundConst.GENERAL_BED_FADE_OUT_TO_FINAL_ROUND_BED,
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

    /// <summary>
    /// Показывает на экране игрока панель ответов финального раунда
    /// </summary>
    private async Task ShowPlayerFinalRoundPanel() {
        logger.SignedDebug();
        await Task.Delay(SHOW_HIDE_PLAYER_FINAL_ROUND_PANEL_DELAY);
        playerDataContext.ShowFinalRoundPanel();
    }

    /// <summary>
    /// Фиксирует правильный ответ финалиста
    /// </summary>
    private async Task CorrectAnswer() {
        logger.Info($"{(turnSwitch ? $"{FirstPlayer.Name}" : $"{SecondPlayer.Name}")} finalist give correct answer");
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
                    await Task.Delay(SUDDEN_DEATH_QUESTION_TIE_DELAY);
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
    /// Фиксирует неправильный ответ финалиста
    /// </summary>
    private async Task WrongAnswer() {
        logger.Info($"{(turnSwitch ? $"{FirstPlayer.Name}" : $"{SecondPlayer.Name}")} finalist give wrong answer");
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
                    await Task.Delay(SUDDEN_DEATH_QUESTION_TIE_DELAY);
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
    /// Меняет вопрос на следующий из базы вопросов без отметки, что текущий вопрос использован
    /// </summary>
    private void NextQuestion() {
        logger.SignedDebug();
        QuestionIndex++;
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion();
    }

    /// <summary>
    /// Меняет вопрос на следующий из базы вопросов с отметкой, что текущий вопрос использован
    /// </summary>
    private void NextUsedQuestion() {
        logger.SignedDebug();
        QuestionIndex = 0;
        CurrentQuestion = WeakestLinkLogic.NextFinalQuestion(true);
    }

    /// <summary>
    /// Меняет вопрос на предыдущий
    /// </summary>
    private void PreviousQuestion() {
        logger.SignedDebug();
        QuestionIndex--;
        CurrentQuestion = WeakestLinkLogic.PreviousFinalQuestion();
    }

    /// <summary>
    /// Начинает игру "до первого проигрыша"
    /// </summary>
    private void StartSuddenDeath() {
        logger.Info("Starting sudden death");
        SoundManager.FadeWith(SoundName.PENALTY_SHOOTOUT_BED, SoundName.SUDDEN_DEATH_BED, fadeOutMilliseconds: SoundConst.GENERAL_BED_FADE_OUT_TO_FINAL_ROUND_BED,
            soundInPositionA: SoundConst.SUDDEN_DEATH_BED_LOOP_POSITION_A, soundInPositionB: SoundConst.SUDDEN_DEATH_BED_LOOP_POSITION_B);
        IsTie = false;
        CurrentPlayer = FirstPlayer;
        InfoText = string.Empty;
        IsSuddenDeath = true;
        NextUsedQuestion();
        AddSuddenDeathQuestion();
    }

    /// <summary>
    /// Добавляет ещё один вопрос игры "до первого проигрыша"
    /// </summary>
    private void AddSuddenDeathQuestion() {
        logger.SignedDebug();
        FirstPlayerAnswersPanel.Clear();
        SecondPlayerAnswersPanel.Clear();
        FirstPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = currentQuestionNumber, IsActive = true });
        SecondPlayerAnswersPanel.Add(new FinalQuestionVisual { QuestionNumber = currentQuestionNumber, IsActive = false });

        playerDataContext.AddSuddenDeathQuestion(currentQuestionNumber);
    }

    /// <summary>
    /// Проверяет, что кто-то своим ответом может обеспечить себе победу или проигрыш
    /// </summary>
    private void CheckWarning() {
        logger.Debug("Check warning of current player losing or winning");
        InfoText = string.Empty;

        if (!IsSuddenDeath) {
            if (turnSwitch) {
                if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                    FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                    logger.Info($"{SecondPlayer.Name} can win by next correct answer");
                    InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
                }

                if (SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                    FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) + FirstPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                    logger.Info($"{SecondPlayer.Name} can lose by next wrong answer");
                    InfoText = $"{SecondPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
                }
            }
            else {
                if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) ==
                    SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
                    logger.Info($"{FirstPlayer.Name} can win by next correct answer");
                    InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите верно, вы выиграете";
                }

                if (FirstPlayerAnswersPanel.Count(x => x.IsWrong == true) ==
                    SecondPlayerAnswersPanel.Count(x => x.IsWrong == true) + SecondPlayerAnswersPanel.Count(x => x.IsWrong == null)) {
                    logger.Info($"{FirstPlayer.Name} can lose by next wrong answer");
                    InfoText = $"{FirstPlayer.Name}, если сейчас вы ответите неверно, вы проиграете";
                }
            }
        }
    }

    /// <summary>
    /// Проверяет игру на ничью
    /// </summary>
    private void CheckTie() {
        logger.SignedDebug();
        IsTie = FirstPlayerAnswersPanel.All(x => x.IsRight != null && x.IsWrong != null)
            && SecondPlayerAnswersPanel.All(x => x.IsRight != null && x.IsWrong != null)
            && FirstPlayerAnswersPanel.Count(x => x.IsRight == true) == SecondPlayerAnswersPanel.Count(x => x.IsRight == true);

        if (IsTie && !IsSuddenDeath) {
            logger.Info("Is tie. Sudden death is coming");
            InfoText = $"После пяти пар вопросов счёт равный. Мы продолжаем игру до первого проигрыша. Вопросы по-прежнему будут задаваться парами.{Environment.NewLine}" +
                $"{FirstPlayer.Name} если вы правильно отвечаете на вопрос, {SecondPlayer.Name} тоже должен ответить правильно, иначе он проиграет.{Environment.NewLine}" +
                $"{FirstPlayer.Name} если вы неверно отвечаете на вопрос, а {SecondPlayer.Name} даёт правильный ответ, он выигрывает.{Environment.NewLine}" +
                $"Итак, {FirstPlayer.Name}, {SecondPlayer.Name}, играем до первого проигрыша";
        }
    }

    /// <summary>
    /// Проверяет игру на победу какого-либо финалиста
    /// </summary>
    private bool CheckWin() {
        logger.SignedDebug();
        if (FirstPlayerAnswersPanel.Count(x => x.IsRight == true) >
            SecondPlayerAnswersPanel.Count(x => x.IsRight == true) + SecondPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            logger.Info($"{FirstPlayer.Name} win the game");
            WinSound();
            FirstPlayer.IsWinner = true;
            IsGameEnd = true;
            IsSuddenDeath = false;
            IsFinalRoundPlaying = false;
            EndGameText =
                $"{FirstPlayer.Name}, сегодня вы - самое сильное звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {SecondPlayer.Name}, вы уходите ни с чем.{Environment.NewLine}Вы смотрели \"Слабое звено\", это была всего лишь игра! До встречи!";
            HidePlayerFinalRoundPanel();
            WeakestLinkLogic.SetupWinner(FirstPlayer);
            return true;
        }

        if (SecondPlayerAnswersPanel.Count(x => x.IsRight == true) >
            FirstPlayerAnswersPanel.Count(x => x.IsRight == true) + FirstPlayerAnswersPanel.Count(x => x.IsRight == null)) {
            logger.Info($"{SecondPlayer.Name} win the game");
            WinSound();
            SecondPlayer.IsWinner = true;
            IsGameEnd = true;
            IsSuddenDeath = false;
            IsFinalRoundPlaying = false;
            EndGameText =
                $"{SecondPlayer.Name}, сегодня вы - самое сильное звено, и уходите домой с суммой {WeakestLinkLogic.CurrentSession.FullBank.Decline("рубль", "рубля", "рублей")}. {FirstPlayer.Name}, вы уходите ни с чем.{Environment.NewLine}Вы смотрели \"Слабое звено\", это была всего лишь игра! До встречи!";
            HidePlayerFinalRoundPanel();
            WeakestLinkLogic.SetupWinner(SecondPlayer);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Скрывает панель ответов финального раунда на экране игрока
    /// </summary>
    private async Task HidePlayerFinalRoundPanel() {
        logger.SignedDebug();
        await Task.Delay(SHOW_HIDE_PLAYER_FINAL_ROUND_PANEL_DELAY);
        playerDataContext.HideFinalRoundPanel();
        CanEndGame = true;
        CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>
    /// Проигрывает звук победы
    /// </summary>
    private void WinSound() {
        logger.SignedDebug();
        SoundManager.FadeWith(IsSuddenDeath ? SoundName.SUDDEN_DEATH_BED : SoundName.PENALTY_SHOOTOUT_BED, SoundName.WINNER_THEME, fadeOutMilliseconds: SoundConst.FINAL_BED_FADE_OUT_TO_WINNER_THEME,
            soundInPositionA: SoundConst.WINNER_THEME_LOOP_POSITION_A,
            soundInPositionB: SoundConst.WINNER_THEME_LOOP_POSITION_B);
    }

    /// <summary>
    /// Завершает игру
    /// </summary>
    private void EndGame() {
        logger.Debug("Game is ended");
        WeakestLinkLogic.EndGame();
        ChangeMWPage<EndGamePanelPage>();
    }
}