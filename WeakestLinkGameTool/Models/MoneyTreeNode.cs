using WeakestLinkGameTool.Models.Visual;

namespace WeakestLinkGameTool.Models;

/// <summary>
/// Элемент денежной цепочки
/// </summary>
public class MoneyTreeNode {
    
    /// <summary>
    /// Денежное значение
    /// </summary>
    public int Value { get; set; }
    
    /// <summary>
    /// Порядковый номер в цепочке
    /// </summary>
    public int ChainNumber { get; set; }
    
    /// <summary>
    /// Преобразует модель в её визуальную вариацию
    /// </summary>
    /// <returns></returns>
    public MoneyTreeNodeVisual ConvertToVisual() => new() {
        ChainNumber = ChainNumber,
        Value = Value
    };
}