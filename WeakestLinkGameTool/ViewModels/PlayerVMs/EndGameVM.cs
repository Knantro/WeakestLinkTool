using System.Text;
using WeakestLinkGameTool.ViewModels.Base;

namespace WeakestLinkGameTool.ViewModels.PlayerVMs;

/// <summary>
/// Модель-представление экрана игрока с завершающими титрами
/// </summary>
public class EndGameVM : ViewModelBase {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private string credits;

    /// <summary>
    /// Событие немедленного завершения титров
    /// </summary>
    public event EventHandler CompleteCreditsRaised;

    /// <summary>
    /// Титры
    /// </summary>
    public string Credits {
        get => credits;
        set => SetField(ref credits, value);
    }

    public EndGameVM() {
        logger.SignedDebug();
        BuildCredits();
    }

    /// <summary>
    /// Завершает титры немедленно
    /// </summary>
    public void CompleteCreditsImmediately() {
        logger.Debug("Complete credits immediately");
        CompleteCreditsRaised?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Строит титры
    /// </summary>
    private void BuildCredits() {
        logger.Debug("Building Credits");
        var sb = new StringBuilder();
        AddLine(sb, $"Победитель - {WeakestLinkLogic.CurrentSession.Winner.Name}", true);
        // AddLine(sb, $"Победитель - Александр", true); // TODO: Тест

        AddLine(sb, "Ведущий");
        AddLine(sb, "Knantro", true);

        AddLine(sb, "Автор программы");
        AddLine(sb, "Knantro", true);

        AddLine(sb, "Разработчики");
        AddLine(sb, "Knantro", true);

        AddLine(sb, "Дизайнеры");
        AddLine(sb, "Knantro", true);

        AddLine(sb, "Авторы вопросов");
        AddLine(sb, "Knantro");
        AddLine(sb, "Банк вопросов \"Слабого Звена\"");
        AddLine(sb, "NULL"); // TODO: Указать автора 
        AddLine(sb, "NULL", true); // TODO: Указать автора 

        AddLine(sb, "Авторы подколок");
        AddLine(sb, "Knantro");
        AddLine(sb, "Банк вопросов \"Слабого Звена\"");
        AddLine(sb, "NULL"); // TODO: Указать автора 
        AddLine(sb, "NULL", true); // TODO: Указать автора 

        AddLine(sb, "Музыка");
        AddLine(sb, "Paul Farrer", true);

        AddLine(sb, "Основано на");
        AddLine(sb, "телевизионном");
        AddLine(sb, "формате");
        AddLine(sb, "BBC \"The Weakest Link\"", true);

        Credits = sb.ToString();
    }

    /// <summary>
    /// Добавляет строку с переносом на новую строку(-и)
    /// </summary>
    /// <param name="sb">Объект <see cref="StringBuilder"/></param>
    /// <param name="value">Строка на добавление</param>
    /// <param name="isEnd">Признак конца блока титров</param>
    private void AddLine(StringBuilder sb, string value, bool isEnd = false) {
        logger.SignedTrace(value);
        sb.Append(value);
        AppendLinesCredits(sb, isEnd ? 3 : 1);
    }

    /// <summary>
    /// Добавляет пустые строки
    /// </summary>
    /// <param name="sb">Объект <see cref="StringBuilder"/></param>
    /// <param name="count">Количество пустых строк</param>
    private void AppendLinesCredits(StringBuilder sb, int count) {
        logger.SignedTrace(count.ToString());
        for (var i = 0; i < count; i++) {
            sb.AppendLine();
        }
    }
}