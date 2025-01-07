using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Logic.Sounds;

/// <summary>
/// 
/// </summary>
public static class SoundManager {
    public static Logger logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 
    /// </summary>
    private static Dictionary<string, Sound> audios { get; set; } = [];

    static SoundManager() => Init();

    /// <summary>
    /// 
    /// </summary>
    public static void Init() {
        DisposeAll();

        var soundPaths = FilePaths.GetSoundPaths();
        foreach (var sp in soundPaths) {
            var media = new LoopAudioReader(sp);
            var waveOut = new WasapiOut();
            var volumeProvider = new VolumeWaveProvider16(media);
            var fade = new FadeInOutSampleProvider(volumeProvider.ToSampleProvider());
            waveOut.Init(fade);

            audios[Path.GetFileNameWithoutExtension(sp)] = new Sound(media, waveOut, fade, volumeProvider);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="volume"></param>
    public static void SetVolume(string soundName, float volume) {
        if (audios.TryGetValue(soundName, out var audio)) {
            audio.Volume = volume;
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="coefficient"></param>
    public static void SetVolumeCoefficient(string soundName, float coefficient) {
        if (audios.TryGetValue(soundName, out var audio)) {
            audio.SetVolumeCoefficient(coefficient);
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="volume"></param>
    public static void SetVolumeAll(float volume) {
        audios.Values.ForEach(x => x.Volume = volume);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="positionA"></param>
    /// <param name="positionB"></param>
    public static void LoopPlay(string soundName, long positionA, long positionB) {
        if (audios.TryGetValue(soundName, out var audio)) {
            audio.Media.LoopEnabled = true;
            audio.Media.PositionA = positionA;
            audio.Media.PositionB = positionB;

            audio.AudioOut.Stop();
            audio.Fade.BeginFadeIn(0);
            audio.Media.Position = 0;
            audio.AudioOut.Play();
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    public static void Play(string soundName) {
        try {
            if (audios.TryGetValue(soundName, out var audio)) {
                audio.Media.LoopEnabled = false;

                audio.AudioOut.Stop();
                audio.Fade.BeginFadeIn(0);
                audio.Media.Position = 0;
                audio.AudioOut.Play();
            }
            else {
                logger.Warn($"Fail to play. Audio '{soundName}' does not exist");
            }
        }
        catch (Exception e) {
            logger.Error(e, "Failed to play sound");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    public static void Pause(string soundName) {
        try {
            if (audios.TryGetValue(soundName, out var audio)) audio.AudioOut.Pause();
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to pause sound");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    public static void Resume(string soundName) {
        try {
            if (audios.TryGetValue(soundName, out var audio)) audio.AudioOut.Play();
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to resume sound");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    public static void Stop(string soundName) {
        try {
            if (audios.TryGetValue(soundName, out var audio)) {
                audio.AudioOut.Stop();
                audio.Media.Position = 0;
            }
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to stop sound");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundFadeOut"></param>
    /// <param name="soundFadeIn"></param>
    /// <param name="fadeInOutMilliseconds"></param>
    /// <param name="soundFadeInStartPosition"></param>
    public static async Task FadeWith(string soundFadeOut, string soundFadeIn, int fadeInOutMilliseconds, TimeSpan? soundFadeInStartPosition = null) =>
        await FadeWith(soundFadeOut, soundFadeIn, fadeInOutMilliseconds, fadeInOutMilliseconds, soundFadeInStartPosition);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundFadeOut"></param>
    /// <param name="soundFadeIn"></param>
    /// <param name="fadeOutMilliseconds"></param>
    /// <param name="fadeInMilliseconds"></param>
    /// <param name="soundFadeInStartPosition"></param>
    /// <param name="soundInPositionA"></param>
    /// <param name="soundInPositionB"></param>
    public static async Task FadeWith(string soundFadeOut, string soundFadeIn, int? fadeOutMilliseconds = null, int? fadeInMilliseconds = null, 
        TimeSpan? soundFadeInStartPosition = null, long? soundInPositionA = null, long? soundInPositionB = null) {
        try {
            if (!fadeOutMilliseconds.HasValue && !fadeInMilliseconds.HasValue) {
                logger.Warn("At least one of 'fadeOutMilliseconds' or 'fadeInMilliseconds' parameters must be defined");
                return;
            }
            
            if (soundFadeInStartPosition.HasValue && (soundInPositionA.HasValue || soundInPositionB.HasValue)) {
                logger.Warn("Can't fade from position in loop stream together");
                return;
            }

            if (soundInPositionA.HasValue != soundInPositionB.HasValue) {
                logger.Warn("Position A and position B should be defined or NOT defined together");
                return;
            }
            
            if (!audios.TryGetValue(soundFadeOut, out var audioLeft)) {
                logger.Warn($"Audio '{soundFadeOut}' does not exist");
                return;
            }

            if (!audios.TryGetValue(soundFadeIn, out var audioRight)) {
                logger.Warn($"Audio '{soundFadeIn}' does not exist");
                return;
            }

            if (audioLeft.AudioOut.PlaybackState == PlaybackState.Playing) {
                Stop(soundFadeIn);
                
                if (soundFadeInStartPosition.HasValue) audioRight.Media.CurrentTime = soundFadeInStartPosition.Value;
                
                if (fadeInMilliseconds.HasValue) audioRight.Fade.BeginFadeIn(fadeInMilliseconds.Value);
                if (fadeOutMilliseconds.HasValue) audioLeft.Fade.BeginFadeOut(fadeOutMilliseconds.Value);

                if (soundInPositionA.HasValue && soundInPositionB.HasValue) LoopPlay(soundFadeIn, soundInPositionA.Value, soundInPositionB.Value);
                else audioRight.AudioOut.Play();

                if (fadeOutMilliseconds.HasValue) {
                    await Task.Delay(fadeOutMilliseconds.Value);
                    Stop(soundFadeOut);
                }
            }
            else logger.Warn($"Audio '{soundFadeOut}' is not playing now");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to fade sounds");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void DisposeAll() {
        audios.Values.ForEach(x => x.Dispose());
    }
}