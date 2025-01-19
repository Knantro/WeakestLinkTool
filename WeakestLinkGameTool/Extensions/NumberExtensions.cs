using System.Globalization;

namespace WeakestLinkGameTool.Extensions;

/// <summary>
/// Расширения для чисел
/// </summary>
public static class NumberExtensions {

    /// <summary>
    /// Проверяет, что число находится в интервале
    /// </summary>
    /// <param name="number">Исходное число</param>
    /// <param name="left">Левая граница интервала</param>
    /// <param name="right">Правая граница интервала</param>
    /// <returns>True, если число внутри интервала, иначе False</returns>
    public static bool InRange(this int number, int left, int right) =>
        number >= left && number <= right;

    /// <summary>
    /// Проверяет, что число находится в интервале
    /// </summary>
    /// <param name="number">Исходное число</param>
    /// <param name="left">Левая граница интервала</param>
    /// <param name="right">Правая граница интервала</param>
    /// <returns>True, если число внутри интервала, иначе False</returns>
    public static bool InRange(this float number, float left, float right) =>
        number >= left && number <= right;

    /// <summary>
    /// Склоняет существительное в зависимости от числительного перед ним
    /// </summary>
    /// <param name="num">Число</param>
    /// <param name="nominative">Слово в именительном падеже</param>
    /// <param name="singular">Слово в родительном падеже ед. числе</param>
    /// <param name="plural">Слово в множественном числе</param>
    /// <returns>Возвращает число с добавленным существительным в нужном склонении</returns>
    public static string Decline(this int num, string nominative, string singular, string plural) {
        if (num > 10 && num % 100 / 10 == 1) return num.ToString(CultureInfo.InvariantCulture) + " " + plural;

        return (num % 10) switch {
            1 => num.ToString(CultureInfo.InvariantCulture) + " " + nominative,
            2 or 3 or 4 => num.ToString(CultureInfo.InvariantCulture) + " " + singular,
            _ => num.ToString(CultureInfo.InvariantCulture) + " " + plural
        };
    }
}