using NAudio.Wave;

namespace WeakestLinkGameTool.Helpers;

/// <summary>
/// 
/// </summary>
public class LoopAudioReader : MediaFoundationReader 
{
    /// <summary>
    /// 
    /// </summary>
    public long PositionA { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public long PositionB { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool LoopEnabled { get; set; }

    public LoopAudioReader(string audioFilePath) : base(audioFilePath) { }
    
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