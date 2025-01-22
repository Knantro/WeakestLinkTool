using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Logic;

/// <summary>
/// Общая логика игры
/// </summary>
public class WeakestLinkLogic {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private static readonly Logger gameLogger = LogManager.GetLogger("fileGameLog");
    private const string DEFAULT_MONEY_TREE_STRING = "1000;2000;5000;10000;20000;30000;40000;50000";
    private const int MINIMUM_REQUIRED_REGULAR_QUESTIONS = 200;
    private const int MINIMUM_REQUIRED_FINAL_QUESTIONS = 20;
    private const int MINIMUM_REQUIRED_JOKES = 20;
    private const int MIN_AVAILABLE_PLAYERS = 3; // TODO: Test value
    private const int MAX_AVAILABLE_PLAYERS = 11;
    private TimeSpan firstRoundTimer = new(0, 3, 0);
    private TimeSpan preFinalRoundTimer = new(0, 1, 30);
    private int currentQuestionIndex = -1;
    private int currentFinalQuestionIndex = -1;
    private int currentJokeIndex = -1;
    private int currentPlayerIndex;

    /// <summary>
    /// Текущая игровая сессия
    /// </summary>
    public GameSession CurrentSession { get; private set; }

    /// <summary>
    /// Вопросы регулярного раунда
    /// </summary>
    public List<Question> RegularQuestions { get; private set; } = [];

    /// <summary>
    /// Вопросы финального раунда
    /// </summary>
    public List<Question> FinalQuestions { get; private set; } = [];

    /// <summary>
    /// Подколки
    /// </summary>
    public List<Joke> Jokes { get; private set; } = [];

    /// <summary>
    /// Неиспользованные вопросы регулярного раунда
    /// </summary>
    public List<Question> UnusedRegularQuestions => RegularQuestions.Where(x => !x.IsUsed).ToList();

    /// <summary>
    /// Неиспользованные вопросы финального раунда
    /// </summary>
    public List<Question> UnusedFinalQuestions => FinalQuestions.Where(x => !x.IsUsed).ToList();

    /// <summary>
    /// Неиспользованные подколки
    /// </summary>
    public List<Joke> UnusedJokes => Jokes.Where(x => !x.IsUsed).ToList();

    /// <summary>
    /// Денежное дерево
    /// </summary>
    public List<MoneyTreeNode> MoneyTree { get; private set; } = [];

    public bool CanNewGame => true; // TODO: TEST_REMOVE

    /// <summary>
    /// Можно ли начать новую игру
    /// </summary>
    // public bool CanStartNewGame => RegularQuestions.Count >= MINIMUM_REQUIRED_REGULAR_QUESTIONS &&
    // FinalQuestions.Count >= MINIMUM_REQUIRED_FINAL_QUESTIONS &&
    // Jokes.Count >= MINIMUM_REQUIRED_JOKES;

    /// <summary>
    /// Можно ли начать игру
    /// </summary>
    public bool CanStartGame => CurrentSession?.AllPlayers.Count.InRange(MIN_AVAILABLE_PLAYERS, MAX_AVAILABLE_PLAYERS) == true &&
        CurrentSession?.AllPlayers.All(x => !string.IsNullOrEmpty(x.Name)) == true;

    /// <summary>
    /// Максимально возможный выигрыш
    /// </summary>
    public int MaxPossibleGain => (CurrentSession?.AllPlayers.Count ?? 0) * MoneyTree.Max(x => x.Value);

    public WeakestLinkLogic() {
        logger.Debug("Init main logic class");
        LoadData();
    }

    /// <summary>
    /// Создаёт игровую сессию
    /// </summary>
    public void InitSession() {
        logger.Info("Init new game session");
        CurrentSession = new GameSession();
    }

    /// <summary>
    /// Начинает игру записью в истории игры
    /// </summary>
    public void StartGame() {
        logger.Info($"Starting game with players: {string.Join(" ", CurrentSession.AllPlayers.Select(x => $"{x.Number}. {x.Name}"))}");
        gameLogger.GameLog(CurrentSession.SessionID, $"Игра начинается. Участвуют игроки: {string.Join(" ", CurrentSession.AllPlayers.Select(x => $"{x.Number}. {x.Name}"))}");
    }

    /// <summary>
    /// Создаёт новую сессию с теми же игроками
    /// </summary>
    public void NewSessionSamePlayers() {
        logger.Info("Starting game with same players");
        var players = CurrentSession.AllPlayers.Select(x => new Player { Number = x.Number, Name = x.Name }).ToList();
        CurrentSession = new GameSession { AllPlayers = players };
        gameLogger.GameLog(CurrentSession.SessionID, $"Начинается новая игра с теми же игроками: {string.Join(" ", CurrentSession.AllPlayers.Select(x => $"{x.Number}. {x.Name}"))}");
    }

    /// <summary>
    /// Переключает раунд на следующий
    /// </summary>
    public Round NextRound() {
        logger.Debug("Get next round");
        if ((CurrentSession.CurrentRound?.Number + 1 ?? 1) != CurrentSession.AllPlayers.Count) {
            CurrentSession.CurrentRound = new Round {
                Number = CurrentSession.CurrentRound?.Number + 1 ?? 1,
                IsPreFinal = (CurrentSession.CurrentRound?.Number + 1 ?? 1) == CurrentSession.AllPlayers.Count - 1,
                BankedMoney = 0,
                Timer = (CurrentSession.CurrentRound?.Number + 1 ?? 1) == CurrentSession.AllPlayers.Count - 1
                    ? preFinalRoundTimer
                    : firstRoundTimer.Add(TimeSpan.FromSeconds(-10 * CurrentSession.CurrentRound?.Number ?? 0))
            };
        }
        else {
            CurrentSession.CurrentRound = new FinalRound {
                Number = CurrentSession.AllPlayers.Count,
            };
        }

        CurrentSession.Rounds.Add(CurrentSession.CurrentRound);

        CurrentSession.CurrentRound.Statistics = new RoundStatistics {
            RoundNumber = CurrentSession.CurrentRound.Number,
            PlayersStatistics = CurrentSession.ActivePlayers.ToDictionary(p => p,
                p => new PlayerStatistics {
                    RoundName = CurrentSession.CurrentRound.IsFinal ? "Финал" : CurrentSession.CurrentRound.Number.ToString(),
                    Player = p
                }),
        };

        logger.Info($"Starting new round - {(CurrentSession.CurrentRound.Number == 0 ? "Финал" : $"{CurrentSession.CurrentRound.Number}")}");
        gameLogger.GameLog(CurrentSession.SessionID, $"Начался {(CurrentSession.CurrentRound.IsFinal ? "финал" : $"раунд - {CurrentSession.CurrentRound.Number}")}");

        return CurrentSession.CurrentRound;
    }

    /// <summary>
    /// Обнуляет временные параметры игрока
    /// </summary>
    public void ResetTempPlayerParams() {
        logger.Debug("Reset temp player params");
        CurrentSession.ActivePlayers.ForEach(x => {
            x.IsStrongestLink = false;
            x.IsWeakestLink = false;
            x.VotesCount = 0;
        });
    }

    /// <summary>
    /// Проверяет, что у всех вопросов и подколок заполнены поля
    /// </summary>
    /// <returns>True, если условия выполняются и поля заполнены, иначе False</returns>
    public bool ValidateEditableData() => RegularQuestions.All(x => !string.IsNullOrEmpty(x.Text) && !string.IsNullOrEmpty(x.Answer)) &&
        FinalQuestions.All(x => !string.IsNullOrEmpty(x.Text) && !string.IsNullOrEmpty(x.Answer)) &&
        Jokes.All(x => !string.IsNullOrEmpty(x.Text));
    
    /// <summary>
    /// Делает вопросы и подколки неиспользованными
    /// </summary>
    public void ResetAll() {
        logger.Info("Reset questions and jokes as unused");
        
        RegularQuestions.ForEach(x => x.IsUsed = false);
        FinalQuestions.ForEach(x => x.IsUsed = false);
        Jokes.ForEach(x => x.IsUsed = false);
        
        SaveEditableData();
    }

    /// <summary>
    /// Сохраняет редактируемые игровые данные
    /// </summary>
    public void SaveEditableData() {
        logger.Info("Saving editable data");
        ExceptionHelper.ThrowOnFail(FileStorage.Save(RegularQuestions.Union(FinalQuestions), FilePaths.GetFullDataPath(FilePaths.QUESTIONS)));
        ExceptionHelper.ThrowOnFail(FileStorage.Save(Jokes, FilePaths.GetFullDataPath(FilePaths.JOKES)));
    }

    /// <summary>
    /// Загружает игровые данные
    /// </summary>
    private void LoadData() {
        logger.Debug("Load data from data storage");
        var questions = FileStorage.Load<List<Question>>(FilePaths.GetFullDataPath(FilePaths.QUESTIONS));
        RegularQuestions = questions == null ? [] : questions.Where(x => !x.IsFinal).Shuffle().ToList();
        FinalQuestions = questions == null ? [] : questions.Where(x => x.IsFinal).Shuffle().ToList();
        Jokes = FileStorage.Load<List<Joke>>(FilePaths.GetFullDataPath(FilePaths.JOKES))?.Shuffle().ToList() ?? [];

        var split = App.Settings.MoneyTree.Split(';', StringSplitOptions.RemoveEmptyEntries);
        // Длина массива должна быть равна 8 (размер цепочки), все элементы должны быть целочисленные и идти в порядке возрастания
        if (split.Length != 8 || split.Any(x => int.TryParse(x, out _) == false) || !split.Select(int.Parse).SequenceEqual(split.Select(int.Parse).OrderBy(x => x))) {
            split = DEFAULT_MONEY_TREE_STRING.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }

        logger.Info($"Current money tree is: {string.Join(" ", split)}");

        for (var i = 0; i < split.Length; i++) {
            MoneyTree.Add(new MoneyTreeNode {
                ChainNumber = i + 1,
                Value = int.Parse(split[i])
            });
        }
    }

    /// <summary>
    /// Возвращает игрока, который начинает регулярный раунд
    /// </summary>
    /// <returns>Игрок, который начинает регулярный раунд</returns>
    public Player GetStartRoundPlayer() {
        logger.Debug("Get start round player");
        var player = CurrentSession.CurrentRound.Number == 1
            ? CurrentSession.ActivePlayers.OrderBy(x => x.Name).First()
            : CurrentSession.ActivePlayers.FirstOrDefault(x => x.IsStrongestLink) ?? CurrentSession.CurrentRound.Statistics.PlayersStatistics.Values.First(x => !x.Player.IsKicked).Player;

        currentPlayerIndex = CurrentSession.ActivePlayers.IndexOf(player);
        logger.Trace($"Current player index: {currentPlayerIndex}");
        logger.Debug($"Start player: {player.Number}. {player.Name}");
        gameLogger.GameLog(CurrentSession.SessionID, $"Раунд начинается с игрока - {player.Name}");

        return player;
    }

    /// <summary>
    /// Возвращает следующего игрока регулярного раунда
    /// </summary>
    /// <returns>Следующий игрок регулярного раунда для ответа на вопрос</returns>
    public Player GetNextPlayer() {
        logger.Debug("Get next player");
        var player = currentPlayerIndex < CurrentSession.ActivePlayers.Count - 1 ? CurrentSession.ActivePlayers[++currentPlayerIndex] : CurrentSession.ActivePlayers[currentPlayerIndex = 0];

        logger.Trace($"Current player index: {currentPlayerIndex}");
        logger.Debug($"Next player: {player.Number}. {player.Name}");
        gameLogger.GameLog(CurrentSession.SessionID, $"Отвечает игрок - {player.Name}");

        return player;
    }

    /// <summary>
    /// Возвращает следующий вопрос 
    /// </summary>
    /// <param name="isUsed">Пометить ли вопрос использованным</param>
    /// <returns>Следующий вопрос из списка</returns>
    public Question NextQuestion(bool isUsed = false) {
        logger.Debug($"Get next question: isUsed = {isUsed}");
        if (isUsed && currentQuestionIndex >= 0) {
            UnusedRegularQuestions[currentQuestionIndex].IsUsed = true;
            currentQuestionIndex--;
        }

        if (UnusedRegularQuestions.Count == 0) {
            logger.Info("Refill questions. All now are not used");
            RegularQuestions.ForEach(x => x.IsUsed = false);
        }

        var question = currentQuestionIndex < UnusedRegularQuestions.Count - 1 ? UnusedRegularQuestions[++currentQuestionIndex] : UnusedRegularQuestions[currentQuestionIndex = 0];

        logger.Trace($"Current question index: {currentQuestionIndex}");
        logger.Debug($"Next question: {question.Text}. Answer: {question.Answer}");
        gameLogger.GameLog(CurrentSession.SessionID, isUsed
            ? $"Текущий вопрос: {question.Text} (ответ: {question.Answer})"
            : $"Вопрос изменён на следующий: {question.Text} (ответ: {question.Answer})");

        return question;
    }

    /// <summary>
    /// Возвращает следующий через один вопрос 
    /// </summary>
    /// <returns>Следующий через один вопрос из списка</returns>
    public Question NextFollowingQuestion() {
        logger.Debug("Get following next question");
        Question question;
        if (UnusedRegularQuestions.Count <= 1) {
            logger.Info("Refill questions. All now are not used");
            question = UnusedRegularQuestions[currentQuestionIndex];
            RegularQuestions.ForEach(x => x.IsUsed = false);
            currentQuestionIndex = UnusedRegularQuestions.IndexOf(question);
        }

        question = currentQuestionIndex < UnusedRegularQuestions.Count - 1 ? UnusedRegularQuestions[currentQuestionIndex + 1] : UnusedRegularQuestions.First();
        logger.Debug($"Next following question: {question.Text}. Answer: {question.Answer}");
        return question;
    }

    /// <summary>
    /// Возвращает предыдущий вопрос 
    /// </summary>
    /// <returns>Предыдущий вопрос из списка</returns>
    public Question PreviousQuestion() {
        logger.Debug("Get prev question");
        if (UnusedRegularQuestions.Count == 0) {
            logger.Info("Refill questions. All now are not used");
            RegularQuestions.ForEach(x => x.IsUsed = false);
        }

        var question = currentQuestionIndex != 0 ? UnusedRegularQuestions[--currentQuestionIndex] : UnusedRegularQuestions[currentQuestionIndex = UnusedRegularQuestions.Count - 1];

        logger.Debug($"Previous question: {question.Text}. Answer: {question.Answer}");
        gameLogger.GameLog(CurrentSession.SessionID, $"Вопрос изменён на следующий: {question.Text} (ответ: {question.Answer})");

        return question;
    }

    /// <summary>
    /// Возвращает следующую подколку 
    /// </summary>
    /// <returns>Следующая подколка из списка</returns>
    public Joke NextJoke() {
        logger.Debug("Get next joke");
        if (UnusedJokes.Count == 0) {
            logger.Info("Refill jokes. All now are not used");
            Jokes.ForEach(x => x.IsUsed = false);
        }

        var joke = currentQuestionIndex < UnusedJokes.Count - 1 ? UnusedJokes[++currentQuestionIndex] : UnusedJokes[currentQuestionIndex = 0];
        joke.IsUsed = true;
        logger.Debug($"Next joke: {joke.Text}");
        gameLogger.GameLog(CurrentSession.SessionID, $"{joke}");
        return joke;
    }

    /// <summary>
    /// Возвращает следующий вопрос финала 
    /// </summary>
    /// <param name="isUsed">Пометить ли вопрос использованным</param>
    /// <returns>Следующий вопрос финала из списка</returns>
    public Question NextFinalQuestion(bool isUsed = false) {
        logger.Debug($"Get next final question: isUsed = {isUsed}");
        if (isUsed && currentFinalQuestionIndex >= 0) {
            UnusedFinalQuestions[currentFinalQuestionIndex].IsUsed = true;
            currentFinalQuestionIndex--;
        }

        if (UnusedFinalQuestions.Count == 0) {
            logger.Info("Refill final questions. All now are not used");
            FinalQuestions.ForEach(x => x.IsUsed = false);
        }

        var question = currentFinalQuestionIndex < UnusedFinalQuestions.Count - 1 ? UnusedFinalQuestions[++currentFinalQuestionIndex] : UnusedFinalQuestions[currentFinalQuestionIndex = 0];
        logger.Debug($"Next final question: {question.Text}. Answer: {question.Answer}");
        gameLogger.GameLog(CurrentSession.SessionID, isUsed
            ? $"Текущий вопрос финала: {question.Text} (ответ: {question.Answer})"
            : $"Вопрос финала изменён на следующий: {question.Text} (ответ: {question.Answer})");
        return question;
    }

    /// <summary>
    /// Возвращает предыдущий вопрос финала 
    /// </summary>
    /// <returns>Предыдущий вопрос финала из списка</returns>
    public Question PreviousFinalQuestion() {
        logger.Debug("Get prev final question");
        if (UnusedFinalQuestions.Count == 0) FinalQuestions.ForEach(x => x.IsUsed = false);

        var question = currentFinalQuestionIndex != 0 ? UnusedFinalQuestions[--currentFinalQuestionIndex] : UnusedFinalQuestions[currentFinalQuestionIndex = UnusedFinalQuestions.Count - 1];
        logger.Debug($"Next final question: {question.Text}. Answer: {question.Answer}");
        return question;
    }

    /// <summary>
    /// Фиксирует правильный ответ от игрока
    /// </summary>
    /// <param name="player">Игрок, ответивший верно</param>
    /// <param name="answerTime">Время, затраченное на ответ (равно 0, если ответ был в финальном раунде)</param>
    public void CorrectAnswer(Player player, double answerTime = 0) {
        logger.Debug($"Player '{player.Name}' give correct answer {(answerTime == 0 ? string.Empty : $"for {answerTime:F2}s")}");
        var playerStatistics = CurrentSession.CurrentRound.Statistics.PlayersStatistics[player];

        if (CurrentSession.CurrentRound.IsFinal) {
            gameLogger.GameLog(CurrentSession.SessionID, $"Финалист {player.Name} отвечает верно");
            playerStatistics.FinalRoundAnswers.Add(true);
        }
        else {
            gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} отвечает верно");
            playerStatistics.CorrectAnswers++;
            playerStatistics.AnswerSpeeds.Add(answerTime);
        }
    }

    /// <summary>
    /// Фиксирует неправильный ответ от игрока
    /// </summary>
    /// <param name="player">Игрок, ответивший неверно</param>
    /// <param name="answerTime">Время, затраченное на ответ (равно 0, если ответ был в финальном раунде)</param>
    public void WrongAnswer(Player player, double answerTime = 0) {
        logger.Debug($"Player '{player.Name}' give wrong answer {(answerTime == 0 ? string.Empty : $"for {answerTime:F2}s")}");
        var playerStatistics = CurrentSession.CurrentRound.Statistics.PlayersStatistics[player];

        if (CurrentSession.CurrentRound.IsFinal) {
            gameLogger.GameLog(CurrentSession.SessionID, $"Финалист {player.Name} отвечает неверно");
            playerStatistics.FinalRoundAnswers.Add(false);
        }
        else {
            gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} отвечает верно");
            playerStatistics.WrongAnswers++;
            playerStatistics.AnswerSpeeds.Add(answerTime);
        }
    }

    /// <summary>
    /// Добавляет деньги в общий банк игры
    /// </summary>
    /// <param name="player">Игрок, положивший деньги в банк</param>
    /// <param name="money">Деньги, добавленные в банк</param>
    public void BankMoney(Player player, int money) {
        logger.Debug($"Player '{player.Name}' banked {money} rub");
        gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} кладёт в банк {money.Decline("рубль", "рубля", "рублей")}");
        if (CurrentSession.CurrentRound.IsPreFinal) {
            logger.Debug("Banked money are doubled cause round is pre-final");
            money *= 2; // Все деньги предфинального раунда умножаются на 2
        }

        var playerStatistics = CurrentSession.CurrentRound.Statistics.PlayersStatistics[player];
        CurrentSession.CurrentRound.BankedMoney += money;
        playerStatistics.BankedMoney += money;
        CurrentSession.CurrentRound.Statistics.BankedMoney += money;
        CurrentSession.FullBank += money;
    }

    /// <summary>
    /// Завершает регулярный раунд игры, формируя статистику раунда, сильное и слабое звено
    /// </summary>
    public void EndRegularRound() {
        logger.Debug($"Round {CurrentSession.CurrentRound.Number} finished");
        gameLogger.GameLog(CurrentSession.SessionID, "Раунд завершён");

        CurrentSession.CurrentRound.Statistics.PlayersStatistics = CurrentSession.CurrentRound.Statistics.PlayersStatistics
            .OrderByDescending(x => (double)x.Value.CorrectAnswers / (x.Value.CorrectAnswers + x.Value.WrongAnswers))
            .ThenByDescending(x => x.Value.BankedMoney)
            .ThenBy(x => x.Value.AverageSpeed)
            .ToDictionary(x => x.Key, x => x.Value);

        var strongestLink = CurrentSession.CurrentRound.Statistics.PlayersStatistics.First().Value;
        strongestLink.IsStrongestLink = true;
        strongestLink.Player.IsStrongestLink = true;

        var weakestLink = CurrentSession.CurrentRound.Statistics.PlayersStatistics.Last().Value;
        weakestLink.IsWeakestLink = true;
        weakestLink.Player.IsWeakestLink = true;

        logger.Info($"Strongest link is '{strongestLink.Player.Name}', weakest link is '{weakestLink.Player.Name}'");

        CurrentSession.ActivePlayers.ForEach(x => { x.Statistics.Add(CurrentSession.CurrentRound.Statistics.PlayersStatistics[x]); });

        gameLogger.GameLog(CurrentSession.SessionID, $"Сильное звено - {strongestLink.Player.Name}. Слабое звено - {weakestLink.Player.Name}");
    }

    /// <summary>
    /// Завершает игру, формируя финальную статистику и отчёт игры
    /// </summary>
    public void EndGame() {
        logger.Info($"Game is finished. '{CurrentSession.Winner.Name}' won the game");
        gameLogger.GameLog(CurrentSession.SessionID, "Игра завершена");

        SaveEditableData();

        CurrentSession.ActivePlayers.ForEach(x => { x.Statistics.Add(CurrentSession.CurrentRound.Statistics.PlayersStatistics[x]); });

        Task.Run(() => ExcelStatistics.Generate(CurrentSession));
    }

    /// <summary>
    /// Исключает игрока из игры, объявляя его слабым звеном
    /// </summary>
    /// <param name="player">Игрок, объявленный слабым звеном</param>
    public void KickPlayer(Player player) {
        logger.Info($"'{player.Name}' is kicked as a weakest link");
        gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} объявлен слабым звеном и уходит из игры ни с чем");
        CurrentSession.CurrentRound.KickedPlayer = player;
        player.IsKicked = true;
    }

    /// <summary>
    /// Возвращает исключённого игрока
    /// </summary>
    /// <returns>Исключённый игрок</returns>
    public Player GetCurrentKickedPlayer() {
        return CurrentSession.CurrentRound.KickedPlayer;
    }

    /// <summary>
    /// Фиксирует победителя игры
    /// </summary>
    /// <param name="player">Победитель игры</param>
    public void SetupWinner(Player player) {
        logger.Info($"Winner is '{player.Name}'");
        gameLogger.GameLog(CurrentSession.SessionID, $"Финальный раунд завершён{Environment.NewLine}Победу одержал финалист игры - {player.Name}");

        CurrentSession.Winner = player;
    }
}