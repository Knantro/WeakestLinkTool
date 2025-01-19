using System.Collections.ObjectModel;

namespace WeakestLinkGameTool.Extensions;

/// <summary>
/// Расширения для коллекций
/// </summary>
public static class CollectionExtensions {
    
    /// <summary>
    /// Расширенный метод <see cref="Enumerable.Any{T}(IEnumerable{T})"/> с проверкой на null
    /// </summary>
    /// <param name="collection">Исходная коллекция</param>
    /// <param name="predicate">Предикат</param>
    /// <typeparam name="T">Тип элемента коллекции</typeparam>
    /// <returns>True, если коллекция не null и не пуста и, если предикат задан, то хотя бы одно вхождение удовлетворяет условию предиката, иначе False</returns>
    public static bool IsAny<T>(this IEnumerable<T> collection, Func<T, bool> predicate = null) =>
        collection != null && (predicate == null ? collection.Any() : collection.Any(predicate));
    
    /// <summary>
    /// Приводит исходную коллекцию в <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <param name="collection">Исходная коллекция</param>
    /// <typeparam name="T">Тип элемента коллекции</typeparam>
    /// <returns>Коллекция, преобразованная в <see cref="ObservableCollection{T}"/></returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) => new(collection);
    
    /// <summary>
    /// Расширенный <see cref="List{T}.ForEach(Action{T})"/> для <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <param name="collection">Исходная коллекция</param>
    /// <param name="action">Действие над элементами коллекции</param>
    /// <typeparam name="T">Тип элемента коллекции</typeparam>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) => collection.ToList().ForEach(action);
}