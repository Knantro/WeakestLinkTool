using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeakestLinkGameTool.Helpers;

public class INPCBase : INotifyPropertyChanged {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Событие, что свойство поменяло своё значение
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Обрабатывает событие изменения свойства
    /// </summary>
    /// <param name="propertyName">Имя свойства</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        logger.SignedTrace($"Called for {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Устанавливает значение свойства с вызовом события об его изменении
    /// </summary>
    /// <param name="field">Поддерживающее поле свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <typeparam name="T">Тип свойства</typeparam>
    /// <returns>True, если свойство поменяло значение, иначе False</returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
        logger.SignedTrace($"Called for {propertyName}");
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}