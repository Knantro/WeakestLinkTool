namespace WeakestLinkGameTool;

/// <summary>
/// Конфигурация приложения
/// </summary>
public class AppSettings {

    /// <summary>
    /// Денежная цепочка
    /// </summary>
    /// <remarks>
    /// Должен содержать 8 значений по возрастанию, разделённых точкой с запятой
    /// </remarks>
    public string MoneyTree { get; set; } = "1000;2000;5000;10000;20000;30000;40000;50000";
}