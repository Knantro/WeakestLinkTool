using NAudio.Wave;

namespace WeakestLinkGameTool.Helpers;

/// <summary>
/// Расширение для чтения аудио с возможностью использования режима A-B (цикличное проигрывание отрезка аудио)
/// </summary>
/// <remarks>
/// Режим A-B означает, что когда аудио дойдёт до позиции B, то аудио продолжит проигрываться с позиции A до позиции B циклично
/// </remarks>
public class LoopAudioReader : MediaFoundationReader 
{
    private static Logger logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Позиция A (левая позиция аудио)
    /// </summary>
    public long PositionA { get; private set; }
    
    /// <summary>
    /// Позиция B (правая позиция аудио)
    /// </summary>
    public long PositionB { get; private set; }
    
    /// <summary>
    /// Включён ли режим цикла (режим A-B)
    /// </summary>
    public bool LoopEnabled { get; private set; }

    public LoopAudioReader(string audioFilePath) : base(audioFilePath) { }

    /// <summary>
    /// Устанавливает интервалы A-B и включает цикличное проигрывание аудио
    /// </summary>
    /// <param name="positionA">Позиция A</param>
    /// <param name="positionB">Позиция B</param>
    public void SetABMode(long positionA, long positionB) {
        if (positionA >= positionB) {
            logger.Warn("Position A must be less than Position B");
            return;
        }
        
        PositionA = positionA;
        PositionB = positionB;
        LoopEnabled = true;
    }

    /// <summary>
    /// Отключает режим (A-B)
    /// </summary>
    public void DisableLoop() => LoopEnabled = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override int Read(byte[] buffer, int offset, int count) {
        if (LoopEnabled) {
            var restartLoop = false;
            var diff = count;
        
            if (Position + count >= PositionB * WaveFormat.BlockAlign) {
                count = (int)(PositionB * WaveFormat.BlockAlign - Position);
                diff -= count;
                restartLoop = true;
            }
        
            var res = base.Read(buffer, offset, count);
        
            if (restartLoop) {
                Position = PositionA * WaveFormat.BlockAlign;
                res += base.Read(buffer, offset + count, diff);
            }
        
            return res;
        }

        return base.Read(buffer, offset, count);
    }
}