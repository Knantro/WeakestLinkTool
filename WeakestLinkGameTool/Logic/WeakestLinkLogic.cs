using WeakestLinkGameTool.Helpers;
using WeakestLinkGameTool.Models;
using WeakestLinkGameTool.Models.Statistics;

namespace WeakestLinkGameTool.Logic; 

/// <summary>
/// 
/// </summary>
public class WeakestLinkLogic {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private static readonly Logger gameLogger = LogManager.GetLogger("fileGameLog");
    private const string DEFAULT_MONEY_TREE_STRING = "1000;2000;5000;10000;20000;30000;40000;50000";
    private TimeSpan firstRoundTimer = new(0, 3, 0);
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
    // public bool CanStartNewGame => RegularQuestions.Count >= 100 && FinalQuestions.Count >= 20 && Jokes.Count >= 20;
    
    /// <summary>
    /// Можно ли начать игру
    /// </summary>
    public bool CanStartGame => CurrentSession?.AllPlayers.Count.InRange(3, 11) == true && // TODO: TEST_REMOVE
    // public bool CanStartGame => CurrentSession?.AllPlayers.Count.InRange(7, 11) == true && 
        CurrentSession?.AllPlayers.All(x => !string.IsNullOrEmpty(x.Name)) == true; 

    /// <summary>
    /// Максимально возможный выигрыш
    /// </summary>
    public int MaxPossibleGain => (CurrentSession?.AllPlayers.Count ?? 0) * MoneyTree.Max(x => x.Value); 
    
    public WeakestLinkLogic() {
        LoadData();
    }

    /// <summary>
    /// Создаёт игровую сессию
    /// </summary>
    public void InitSession() {
        CurrentSession = new GameSession();
    }

    /// <summary>
    /// Начинает игру записью в истории игры
    /// </summary>
    public void StartGame() {
        gameLogger.GameLog(CurrentSession.SessionID, $"Игра начинается. Участвуют игроки: {string.Join(" ", CurrentSession.AllPlayers.Select(x => $"{x.Number}. {x.Name}"))}");
    }

    /// <summary>
    /// Создаёт новую сессию с теми же игроками
    /// </summary>
    public void NewSessionSamePlayers() {
        var players = CurrentSession.AllPlayers.Select(x => new Player { Number = x.Number, Name = x.Name }).ToList();
        CurrentSession = new GameSession { AllPlayers = players };
        gameLogger.GameLog(CurrentSession.SessionID, $"Начинается новая игра с теми же игроками: {string.Join(" ", CurrentSession.AllPlayers.Select(x => $"{x.Number}. {x.Name}"))}");
    }

    /// <summary>
    /// Переключает раунд на следующий
    /// </summary>
    public Round NextRound() {
        if ((CurrentSession.CurrentRound?.Number + 1 ?? 1) != CurrentSession.AllPlayers.Count) {
            CurrentSession.CurrentRound = new Round {
                Number = CurrentSession.CurrentRound?.Number + 1 ?? 1,
                IsPreFinal = (CurrentSession.CurrentRound?.Number + 1 ?? 1) == CurrentSession.AllPlayers.Count - 1,
                BankedMoney = 0,
                Timer = (CurrentSession.CurrentRound?.Number + 1 ?? 1) == CurrentSession.AllPlayers.Count - 1 
                    ? new TimeSpan(0, 1, 30)
                    : firstRoundTimer.Add(TimeSpan.FromSeconds(-10 * CurrentSession.CurrentRound?.Number ?? 0))
            };
            
            CurrentSession.Rounds.Add(CurrentSession.CurrentRound);
        }
        else {
            CurrentSession.CurrentRound = new FinalRound {
                Number = CurrentSession.AllPlayers.Count,
            };
        }
        
        CurrentSession.CurrentRound.Statistics = new RoundStatistics {
            RoundNumber = CurrentSession.CurrentRound.Number,
            PlayersStatistics = CurrentSession.ActivePlayers.ToDictionary(p => p, 
                p => new PlayerStatistics {
                    RoundName = CurrentSession.CurrentRound.IsFinal ? "Финал" : CurrentSession.CurrentRound.Number.ToString(), 
                    Player = p
                }),
        };
        
        gameLogger.GameLog(CurrentSession.SessionID, $"Начался {(CurrentSession.CurrentRound.IsFinal ? "финал" : $"раунд - {CurrentSession.CurrentRound.Number}")}");

        return CurrentSession.CurrentRound;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void ResetTempPlayerParams() {
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
    /// Сохраняет редактируемые игровые данные
    /// </summary>
    public void SaveEditableData() {
        ExceptionHelper.ThrowOnFail(FileStorage.Save(RegularQuestions.Union(FinalQuestions), FilePaths.GetFullDataPath(FilePaths.QUESTIONS)));
        ExceptionHelper.ThrowOnFail(FileStorage.Save(Jokes, FilePaths.GetFullDataPath(FilePaths.JOKES)));
    }

    /// <summary>
    /// Загружает игровые данные
    /// </summary>
    private void LoadData() {
       
        var questions = FileStorage.Load<List<Question>>(FilePaths.GetFullDataPath(FilePaths.QUESTIONS));
        RegularQuestions = questions == null ? [] : questions.Where(x => !x.IsFinal).Shuffle().ToList();
        FinalQuestions = questions == null ? [] : questions.Where(x => x.IsFinal).Shuffle().ToList();
        Jokes = FileStorage.Load<List<Joke>>(FilePaths.GetFullDataPath(FilePaths.JOKES))?.Shuffle().ToList() ?? [];
        
        var split = App.Settings.MoneyTree.Split(';', StringSplitOptions.RemoveEmptyEntries);
        // Длина массива должна быть равна 8 (размер цепочки), все элементы должны быть целочисленные и идти в порядке возрастания
        if (split.Length != 8 || split.Any(x => int.TryParse(x, out _) == false) || !split.Select(int.Parse).SequenceEqual(split.Select(int.Parse).OrderBy(x => x))) {
            split = DEFAULT_MONEY_TREE_STRING.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }

        for (var i = 0; i < split.Length; i++) {
            MoneyTree.Add(new MoneyTreeNode {
                ChainNumber = i + 1,
                Value = int.Parse(split[i])
            });
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Player GetStartRoundPlayer() {
        var player = CurrentSession.ActivePlayers.FirstOrDefault(x => x.IsStrongestLink) ?? CurrentSession.CurrentRound.Statistics.PlayersStatistics.Values.First(x => !x.Player.IsKicked).Player;
        currentPlayerIndex = CurrentSession.ActivePlayers.IndexOf(player);
        
        gameLogger.GameLog(CurrentSession.SessionID, $"Раунд начинается с игрока - {player.Name}");
        
        return player;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Player GetNextPlayer() {
        var player = currentPlayerIndex < CurrentSession.ActivePlayers.Count - 1 ? CurrentSession.ActivePlayers[++currentPlayerIndex] : CurrentSession.ActivePlayers[currentPlayerIndex = 0];
        
        gameLogger.GameLog(CurrentSession.SessionID, $"Отвечает игрок - {player.Name}");

        return player;
    }

    /// <summary>
    /// Возвращает следующий вопрос 
    /// </summary>
    /// <param name="isUsed">Пометить ли вопрос использованным</param>
    /// <returns>Следующий вопрос из списка</returns>
    public Question NextQuestion(bool isUsed = false) {
        if (isUsed && currentQuestionIndex >= 0) {
            UnusedRegularQuestions[currentQuestionIndex].IsUsed = true;
            currentQuestionIndex--;
        }
        
        if (UnusedRegularQuestions.Count == 0) RegularQuestions.ForEach(x => x.IsUsed = false);
        
        var question = currentQuestionIndex < UnusedRegularQuestions.Count - 1 ? UnusedRegularQuestions[++currentQuestionIndex] : UnusedRegularQuestions[currentQuestionIndex = 0];
        
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
        if (UnusedRegularQuestions.Count <= 1) {
            var question = UnusedRegularQuestions[currentQuestionIndex];
            RegularQuestions.ForEach(x => x.IsUsed = false);
            currentQuestionIndex = UnusedRegularQuestions.IndexOf(question);
        }
        
        return currentQuestionIndex < UnusedRegularQuestions.Count - 1 ? UnusedRegularQuestions[currentQuestionIndex + 1] : UnusedRegularQuestions.First();
    }

    /// <summary>
    /// Возвращает предыдущий вопрос 
    /// </summary>
    /// <returns>Предыдущий вопрос из списка</returns>
    public Question PreviousQuestion() {
        if (UnusedRegularQuestions.Count == 0) RegularQuestions.ForEach(x => x.IsUsed = false);
        
        var question = currentQuestionIndex != 0 ? UnusedRegularQuestions[--currentQuestionIndex] : UnusedRegularQuestions[currentQuestionIndex = UnusedRegularQuestions.Count - 1];
        
        gameLogger.GameLog(CurrentSession.SessionID, $"Вопрос изменён на следующий: {question.Text} (ответ: {question.Answer})");

        return question;
    }
    
    /// <summary>
    /// Возвращает следующую подколку 
    /// </summary>
    /// <returns>Следующая подколка из списка</returns>
    public Joke NextJoke() {
        if (UnusedJokes.Count == 0) Jokes.ForEach(x => x.IsUsed = false);

        var joke = currentQuestionIndex < UnusedJokes.Count - 1 ? UnusedJokes[++currentQuestionIndex] : UnusedJokes[currentQuestionIndex = 0];
        joke.IsUsed = true;
        gameLogger.GameLog(CurrentSession.SessionID, $"{joke}");
        return joke;
    }
    
    /// <summary>
    /// Возвращает следующий вопрос финала 
    /// </summary>
    /// <param name="isUsed">Пометить ли вопрос использованным</param>
    /// <returns>Следующий вопрос финала из списка</returns>
    public Question NextFinalQuestion(bool isUsed = false) {
        if (isUsed && currentFinalQuestionIndex >= 0) {
            gameLogger.GameLog(CurrentSession.SessionID, $"Текущий вопрос финала: {UnusedRegularQuestions[currentQuestionIndex].Text}");
            UnusedFinalQuestions[currentFinalQuestionIndex].IsUsed = true;
            currentFinalQuestionIndex--;
        }
        
        if (UnusedFinalQuestions.Count == 0) FinalQuestions.ForEach(x => x.IsUsed = false);
        
        return currentFinalQuestionIndex < UnusedFinalQuestions.Count - 1 ? UnusedFinalQuestions[++currentFinalQuestionIndex] : UnusedFinalQuestions[currentFinalQuestionIndex = 0];
    }
    
    /// <summary>
    /// Возвращает предыдущий вопрос финала 
    /// </summary>
    /// <returns>Предыдущий вопрос финала из списка</returns>
    public Question PreviousFinalQuestion() {
        if (UnusedFinalQuestions.Count == 0) FinalQuestions.ForEach(x => x.IsUsed = false);
        
        return currentFinalQuestionIndex != 0 ? UnusedFinalQuestions[--currentFinalQuestionIndex] : UnusedFinalQuestions[currentFinalQuestionIndex = UnusedFinalQuestions.Count - 1];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="answerTime"></param>
    public void CorrectAnswer(Player player, double answerTime = 0) {
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
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="answerTime"></param>
    public void WrongAnswer(Player player, double answerTime = 0) {
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
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="money"></param>
    public void BankMoney(Player player, int money) {
        gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} кладёт в банк {money.Decline("рубль", "рубля", "рублей")}");
        if (CurrentSession.CurrentRound.IsPreFinal) money *= 2; // Все деньги предфинального раунда умножаются на 2
        
        var playerStatistics = CurrentSession.CurrentRound.Statistics.PlayersStatistics[player];
        CurrentSession.CurrentRound.BankedMoney += money;
        playerStatistics.BankedMoney += money;
        CurrentSession.CurrentRound.Statistics.BankedMoney += money;
        CurrentSession.FullBank += money;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndRegularRound() {
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
        
        CurrentSession.ActivePlayers.ForEach(x => {
            x.Statistics.Add(CurrentSession.CurrentRound.Statistics.PlayersStatistics[x]);
        });
        
        gameLogger.GameLog(CurrentSession.SessionID, $"Сильное звено - {strongestLink.Player.Name}. Слабое звено - {weakestLink.Player.Name}");
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndGame() {
        gameLogger.GameLog(CurrentSession.SessionID, "Игра завершена");
        
        CurrentSession.ActivePlayers.ForEach(x => {
            x.Statistics.Add(CurrentSession.CurrentRound.Statistics.PlayersStatistics[x]);
        });
        
        ExcelStatistics.Generate(CurrentSession);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public void KickPlayer(Player player) {
        gameLogger.GameLog(CurrentSession.SessionID, $"Игрок {player.Name} объявлен слабым звеном и уходит из игры ни с чем");
        CurrentSession.CurrentRound.KickedPlayer = player;
        player.IsKicked = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Player GetCurrentKickedPlayer() {
        return CurrentSession.CurrentRound.KickedPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public void SetupWinner(Player player) {
        gameLogger.GameLog(CurrentSession.SessionID, $"Финальный раунд завершён{Environment.NewLine}Победу одержал финалист игры - {player.Name}");
        
        CurrentSession.Winner = player;
    }
}