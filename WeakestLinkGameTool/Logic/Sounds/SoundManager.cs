using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Logic.Sounds;

/// <summary>
/// Менеджер управления воспроизведением звуков
/// </summary>
public static class SoundManager {
    public static Logger logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Коллекция пар ключ-значение, где:<br/>
    /// • Ключ - название звука
    /// • Значение - модель звука с необходимыми свойствами для управления воспроизведением звука
    /// </summary>
    private static Dictionary<string, Sound> audios { get; set; } = [];

    static SoundManager() => Init();

    /// <summary>
    /// Инициализирует звуки, сохраняя их в коллекцию
    /// </summary>
    public static void Init() {
        logger.SignedInfo();
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
    /// Устанавливает громкость звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    /// <param name="volume">Новая громкость</param>
    public static void SetVolume(string soundName, float volume) {
        logger.Debug($"Set volume of '{soundName}' to {volume:F2}");
        if (audios.TryGetValue(soundName, out var audio)) {
            audio.SetVolume(volume);
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// Устанавливает коэффициент громкости звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    /// <param name="coefficient">Новый коэффициент громкости</param>
    public static void SetVolumeCoefficient(string soundName, float coefficient) {
        logger.Debug($"Set volume coefficient of '{soundName}' to {coefficient:F2}");
        if (audios.TryGetValue(soundName, out var audio)) {
            audio.SetVolumeCoefficient(coefficient);
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// Устанавливает громкость для всех звуков
    /// </summary>
    /// <param name="volume">Новая громкость</param>
    public static void SetVolumeAll(float volume) {
        logger.Info($"Set all sounds volume to {volume:F2}");
        audios.Values.ForEach(x => x.SetVolume(volume));
    }

    /// <summary>
    /// Плавно меняет громкость звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    /// <param name="destVolume">Конечный уровень громкости</param>
    /// <param name="duration">Период изменения в миллисекундах (мс)</param>
    public static async Task FadeVolume(string soundName, float destVolume, int duration) {
        logger.Info($"Fade volume of '{soundName}' to '{destVolume:F2}' by {duration}ms");
        if (duration < 0) {
            logger.Warn("Duration must by a positive value");
            return;
        }

        if (!destVolume.InRange(0f, 1f)) {
            logger.Warn($"Destination volume '{destVolume}' is out of range");
            return;
        }

        if (audios.TryGetValue(soundName, out var audio)) {
            if (Math.Abs(destVolume - audio.VolumeCoefficient) < 10e-9) {
                logger.Warn("Destination volume is equal to audio volume");
                return;
            }

            var startVolume = audio.VolumeCoefficient;
            const int steps = 100; // Количество шагов
            var volumeStep = (destVolume - startVolume) / steps;
            var stepDuration = duration / steps == 0 ? 1 : duration / steps;

            for (var i = 1; i <= steps; i++) {
                audio.SetVolumeCoefficient(startVolume + i * volumeStep);
                await Task.Delay(stepDuration);
            }

            audio.SetVolumeCoefficient(destVolume);
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// Проигрывает звук с временным изменением громкости другого звука
    /// </summary>
    /// <param name="soundName">Название звука для проигрывания</param>
    /// <param name="soundToFadeName">Название звука, которому будет на время изменена громкость</param>
    /// <param name="fadeVolume">Временная новая громкость</param>
    /// <param name="fadeDuration">Период в мс, за который звук должен достичь новой громкости</param>
    /// <param name="awaitTime">Период в мс, ожидание до восстановления исходного уровня громкости</param>
    public static async Task PlayWithVolumeFade(string soundName, string soundToFadeName, float fadeVolume, int fadeDuration, int awaitTime) {
        logger.Info($"Playing sound '{soundName}' with volume fade sound '{soundToFadeName}' to '{fadeVolume}'");
        if (fadeDuration < 0 || awaitTime < 0) {
            logger.Warn("Fade Duration and Await Time must be a positive values");
            return;
        }

        if (!fadeVolume.InRange(0f, 1f)) {
            logger.Warn($"Destination volume '{fadeVolume}' is out of range");
            return;
        }

        if (!audios.ContainsKey(soundName) || !audios.TryGetValue(soundToFadeName, out var soundToFade)) {
            logger.Warn($"One of the sounds: '{soundName}', {soundToFadeName} doesn't exist");
            return;
        }

        Play(soundName);
        var recoverVolume = soundToFade.VolumeCoefficient;
        await FadeVolume(soundToFadeName, fadeVolume, fadeDuration);
        await Task.Delay(awaitTime);
        await FadeVolume(soundToFadeName, recoverVolume, fadeDuration);
    }

    /// <summary>
    /// Запускает проигрывание звука в режиме A-B
    /// </summary>
    /// <param name="soundName">Название звука</param>
    /// <param name="positionA">Позиция A</param>
    /// <param name="positionB">Позиция B</param>
    /// <param name="restoreFade">Флаг необходимости восстановления режима затухания (чтобы громкость не была нулевой)</param>
    public static void LoopPlay(string soundName, long positionA, long positionB, bool restoreFade = true) {
        logger.Info($"Loop playing sound '{soundName}' with restore fade = {restoreFade}");
        if (positionA >= positionB) {
            logger.Warn("Position A must be less than position B");
            return;
        }

        if (audios.TryGetValue(soundName, out var audio)) {
            audio.Media.SetABMode(positionA, positionB);

            audio.AudioOut.Stop();
            if (restoreFade) audio.Fade.BeginFadeIn(0);
            audio.Media.Position = 0;
            audio.AudioOut.Play();
        }
        else logger.Warn($"Audio '{soundName}' does not exist");
    }

    /// <summary>
    /// Запускает проигрывание звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    /// <param name="restoreFade">Флаг необходимости восстановления режима затухания (чтобы громкость не была нулевой)</param>
    public static void Play(string soundName, bool restoreFade = true) {
        try {
            logger.Debug($"Playing sound '{soundName}' with restoreFade = {restoreFade}");
            if (audios.TryGetValue(soundName, out var audio)) {
                audio.Media.DisableLoop();

                audio.AudioOut.Stop();
                if (restoreFade) audio.Fade.BeginFadeIn(0);
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
    /// Приостанавливает воспроизведение звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    public static void Pause(string soundName) {
        try {
            logger.Debug($"Pause sound '{soundName}'");
            if (audios.TryGetValue(soundName, out var audio)) audio.AudioOut.Pause();
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to pause sound");
        }
    }

    /// <summary>
    /// Продолжает воспроизведение звука
    /// </summary>
    /// <param name="soundName">Название звука</param>
    public static void Resume(string soundName) {
        try {
            logger.Debug($"Resume sound '{soundName}'");
            if (audios.TryGetValue(soundName, out var audio)) audio.AudioOut.Play();
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to resume sound");
        }
    }

    /// <summary>
    /// Останавливает воспроизведение звука, сбрасывая позицию на начальное значение
    /// </summary>
    /// <param name="soundName">Название звука</param>
    public static void Stop(string soundName) {
        try {
            logger.Debug($"Stop sound '{soundName}'");
            if (audios.TryGetValue(soundName, out var audio)) {
                audio.AudioOut.Stop();
                audio.Fade.BeginFadeIn(0);
                audio.Media.Position = 0;
            }
            else logger.Warn($"Audio '{soundName}' does not exist or played before");
        }
        catch (Exception e) {
            logger.Error(e, "Failed to stop sound");
        }
    }

    /// <summary>
    /// Затухает один звук и одновременно начинает плавное воспроизведение другого звука
    /// </summary>
    /// <param name="soundFadeOut">Название звука для плавного затухания</param>
    /// <param name="soundFadeIn">Название звука для плавного воспроизведения</param>
    /// <param name="fadeInOutMilliseconds">Время в мс, за которое должен произойти одновременный плавный переход по звукам затухания и воспроизведения</param>
    /// <param name="soundFadeInStartPosition">Позиция, с которого начинается воспроизводимый звук</param>
    public static async Task FadeWith(string soundFadeOut, string soundFadeIn, int fadeInOutMilliseconds, TimeSpan? soundFadeInStartPosition = null) =>
        await FadeWith(soundFadeOut, soundFadeIn, fadeInOutMilliseconds, fadeInOutMilliseconds, soundFadeInStartPosition);

    /// <summary>
    /// Затухает один звук и одновременно начинает плавное воспроизведение другого звука 
    /// </summary>
    /// <param name="soundFadeOut">Название звука для плавного затухания</param>
    /// <param name="soundFadeIn">Название звука для плавного воспроизведения</param>
    /// <param name="fadeOutMilliseconds">Время в мс, за которое должно произойти плавное затухание</param>
    /// <param name="fadeInMilliseconds">Время в мс, за которое должно произойти плавное воспроизведение</param>
    /// <param name="soundFadeInStartPosition">Позиция, с которого начинается воспроизводимый звук</param>
    /// <param name="soundInPositionA">Позиция A для воспроизводимого звука</param>
    /// <param name="soundInPositionB">Позиция B для воспроизводимого звука</param>
    public static async Task FadeWith(string soundFadeOut, string soundFadeIn, int? fadeOutMilliseconds = null, int? fadeInMilliseconds = null,
        TimeSpan? soundFadeInStartPosition = null, long? soundInPositionA = null, long? soundInPositionB = null) {
        logger.Debug($"Fade sound '{soundFadeOut}' with '{soundFadeIn}'");
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

            if (soundInPositionA >= soundInPositionB) {
                logger.Warn("Position A must be less than position B");
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

                if (soundInPositionA.HasValue && soundInPositionB.HasValue) LoopPlay(soundFadeIn, soundInPositionA.Value, soundInPositionB.Value, !fadeInMilliseconds.HasValue);
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
    /// Освобождает ресурсы от всех звуков
    /// </summary>
    public static void DisposeAll() {
        logger.SignedInfo();
        audios.Values.ForEach(x => x.Dispose());
    }
}