namespace WeakestLinkGameTool.Models.Statistics;

/// <summary>
/// Статистика игрока за раунд
/// </summary>
public class PlayerStatistics {
    /// <summary>
    /// Номер раунда
    /// </summary>
    public string RoundName { get; set; }

    /// <summary>
    /// Игрок
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// Количество верных ответов
    /// </summary>
    public int CorrectAnswers { get; set; }

    /// <summary>
    /// Количество неверных ответов
    /// </summary>
    public int WrongAnswers { get; set; }

    /// <summary>
    /// Количество денег, положенных игроком в банк
    /// </summary>
    public int BankedMoney { get; set; }

    /// <summary>
    /// Скорости ответов на вопросы
    /// </summary>
    public List<double> AnswerSpeeds { get; set; } = [];

    /// <summary>
    /// Средняя скорость ответов
    /// </summary>
    /// <remarks>
    /// Вернёт null если не было ни одного ответа за раунд
    /// </remarks>
    public double? AverageSpeed => AnswerSpeeds.IsAny() ? AnswerSpeeds.Average() : null;

    /// <summary>
    /// Был ли игрок 'сильным звеном'
    /// </summary>
    public bool IsStrongestLink { get; set; }

    /// <summary>
    /// Был ли игрок 'слабым звеном'
    /// </summary>
    public bool IsWeakestLink { get; set; }

    /// <summary>
    /// Ответы на вопросы финала по порядку (true - игрок дал верный ответ, false - игрок дал неверный ответ)
    /// </summary>
    /// <remarks>
    /// Заполняется только в финальном раунде
    /// </remarks>
    public List<bool> FinalRoundAnswers { get; set; } = [];
}