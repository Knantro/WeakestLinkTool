using System.Windows.Threading;

namespace WeakestLinkGameTool.Helpers;

/// <summary>
/// 
/// </summary>
public class DispatcherTimerEx : DispatcherTimer {
    
    private readonly Logger logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// 
    /// </summary>
    public new TimeSpan Interval
    {
        get => maxInterval;
        set => base.Interval = maxInterval = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public new bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            if (value == base.IsEnabled)
                return;
            if (value)
                this.Start();
            else
                this.Stop();
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
    /// 
    /// </summary>
    public new void Start()
    {
        base.Start();
        startTime = DateTime.Now;
        stopTime = DateTime.MinValue;
        logger.Debug($"OnTick start: {startTime} stop: {stopTime}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTick(object sender, EventArgs e) {
        startTime = DateTime.Now;
        logger.Debug($"OnTick start: {startTime}");
        if (base.Interval == maxInterval) return;

        Stop();
        base.Interval = maxInterval;
        base.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Pause()
    {
        stopTime = DateTime.Now;
        if (stopTime - startTime > maxInterval) {
            startTime += maxInterval;
            return;
        }
        
        Stop();
        logger.Debug($"Pause stop: {stopTime}");
    }

    /// <summary>
    /// 
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
            logger.Debug($"Resume start: {startTime} stop: {stopTime}");
            
            while (maxInterval < stopTime - startTime) {
                startTime += maxInterval;
            }
            
            base.Interval = maxInterval - (stopTime - startTime);
            stopTime = DateTime.MinValue;
        }

        logger.Debug($"Resume start: {startTime} stop: {stopTime}");
        base.Start();
    }
}