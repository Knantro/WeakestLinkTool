namespace WeakestLinkGameTool.Extensions; 

/// <summary>
/// Расширения для класса Random
/// </summary>
public static class RandomExtensions {
    
    /// <summary>
    /// Перемешивает коллекцию случайным образом
    /// </summary>
    /// <param name="source">Исходная коллекция</param>
    /// <typeparam name="T">Тип коллекции</typeparam>
    /// <returns>Перемешанная коллекция</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => source.OrderBy(_ => Guid.NewGuid());
}