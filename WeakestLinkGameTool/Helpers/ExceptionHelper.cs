namespace WeakestLinkGameTool.Helpers;

/// <summary>
/// Расширение для исключений
/// </summary>
public static class ExceptionHelper {

    /// <summary>
    /// Выбрасывает исключение, если <see cref="condition"/> равняется False
    /// </summary>
    /// <param name="condition">Условие для проверки</param>
    /// <exception cref="Exception">Исключение по умолчанию, которое выбрасывается в случае невыполнения <see cref="condition"/></exception>
    public static void ThrowOnFail(bool condition) {
        if (!condition) throw new Exception("Condition failed");
    }
}