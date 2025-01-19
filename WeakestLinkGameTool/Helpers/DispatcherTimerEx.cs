using System.Windows.Threading;

namespace WeakestLinkGameTool.Helpers;

/// <summary>
/// Расширенный интервальный таймер
/// </summary>
/// <remarks>
/// Основное расширение состоит в методах <see cref="Pause"/> и <see cref="Resume"/>, который позволяет ставить таймер на паузу, фиксируя время остановки.<br/><br/>
/// Например, таймер с интервалом 1 секунда, таймер остановлен на 0,5 секунде интервала, после использования метода <see cref="Resume"/>, таймер запустится с 0,5 секунды интервала
/// и через 0,5 секунд, в рамках интервала, будет вызвано событие <see cref="DispatcherTimer.Tick"/>
/// </remarks>
public class DispatcherTimerEx : DispatcherTimer {
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Интервал таймера
    /// </summary>
    public new TimeSpan Interval
    {
        get => maxInterval;
        set => base.Interval = maxInterval = value;
    }

    /// <summary>
    /// Запущен ли таймер
    /// </summary>
    public new bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            if (value == base.IsEnabled) return;
            if (value) Start();
            else Stop();
        }
    }

    TimeSpan maxInterval;
    DateTime startTime = DateTime.MinValue;
    DateTime stopTime = DateTime.MinValue;

    public DispatcherTimerEx()
    {
        Tick += OnTick;
    }

    public DispatcherTimerEx(DispatcherPriority priority) : base(priority)
    {
        Tick += OnTick;
    }

    public DispatcherTimerEx(DispatcherPriority priority, Dispatcher dispatcher) : base(priority, dispatcher)
    {
        Tick += OnTick;
    }

    public DispatcherTimerEx(TimeSpan interval, DispatcherPriority priority, EventHandler callback,
        Dispatcher dispatcher) : base(interval, priority, callback, dispatcher)
    {
        Tick += OnTick;
    }
    
    /// <summary>
    /// Запускает таймер
    /// </summary>
    public new void Start()
    {
        base.Start();
        startTime = DateTime.Now;
        stopTime = DateTime.MinValue;
        logger.Trace($"OnTick start: {startTime} stop: {stopTime}");
    }

    /// <summary>
    /// Событие очередного истечения интервала времени
    /// </summary>
    /// <param name="sender">Инициатор события</param>
    /// <param name="e">Аргумент события</param>
    private void OnTick(object sender, EventArgs e) {
        startTime = DateTime.Now;
        logger.Trace($"OnTick start: {startTime}");
        if (base.Interval == maxInterval) return;

        Stop();
        base.Interval = maxInterval;
        base.Start();
    }

    /// <summary>
    /// Ставит таймер на паузу
    /// </summary>
    public void Pause()
    {
        stopTime = DateTime.Now;
        if (stopTime - startTime > maxInterval) {
            startTime += maxInterval;
            return;
        }
        
        Stop();
        logger.Trace($"Pause stop: {stopTime}");
    }

    /// <summary>
    /// Продолжает работу таймера
    /// </summary>
    public void Resume()
    {
        if (startTime == DateTime.MinValue)
            startTime = DateTime.Now;

        if (stopTime == DateTime.MinValue)
        {
            base.Interval = maxInterval;
        }
        else
        {
            logger.Trace($"Resume start: {startTime} stop: {stopTime}");
            
            while (maxInterval < stopTime - startTime) {
                startTime += maxInterval;
            }
            
            base.Interval = maxInterval - (stopTime - startTime);
            stopTime = DateTime.MinValue;
        }

        logger.Trace($"Resume start: {startTime} stop: {stopTime}");
        base.Start();
    }
}