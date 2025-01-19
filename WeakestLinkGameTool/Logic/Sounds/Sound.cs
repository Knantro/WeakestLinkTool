using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Logic.Sounds;

/// <summary>
/// Модель игрового звука
/// </summary>
public class Sound : IDisposable {
    /// <summary>
    /// Провайдер информации звука
    /// </summary>
    public LoopAudioReader Media { get; private set; }

    /// <summary>
    /// Провайдер воспроизведения звука
    /// </summary>
    public WasapiOut AudioOut { get; private set; }

    /// <summary>
    /// Провайдер постепенного повышения/затухания звука
    /// </summary>
    public FadeInOutSampleProvider Fade { get; private set; }

    /// <summary>
    /// Провайдер громкости звука
    /// </summary>
    public VolumeWaveProvider16 VolumeProvider { get; private set; }

    /// <summary>
    /// Коэффициент громкости
    /// </summary>
    /// <remarks>
    /// Внутренний параметр громкости, который не редактируется пользователем и используется в произведении с исходной громкостью звука
    /// </remarks>
    public float VolumeCoefficient { get; private set; }

    /// <summary>
    /// Громкость звука
    /// </summary>
    public float Volume { get; private set; }

    public Sound(LoopAudioReader media, WasapiOut audioOut, FadeInOutSampleProvider fade, VolumeWaveProvider16 volumeProvider) {
        Media = media;
        AudioOut = audioOut;
        Fade = fade;
        VolumeProvider = volumeProvider;
        Volume = VolumeProvider.Volume;
        VolumeCoefficient = 1;
    }

    /// <summary>
    /// Устанавливает громкость звука
    /// </summary>
    /// <param name="volume">Громкость звука</param>
    public void SetVolume(float volume) {
        Volume = volume;
        VolumeProvider.Volume = volume * VolumeCoefficient;
    }

    /// <summary>
    /// Устанавливает коэффициент громкости звука
    /// </summary>
    /// <param name="coefficient">Коэффициент громкости звука</param>
    public void SetVolumeCoefficient(float coefficient) {
        VolumeCoefficient = coefficient;
        VolumeProvider.Volume = Volume * coefficient;
    }

    /// <summary>
    /// Освобождает ресурсы, выделенные под звук
    /// </summary>
    public void Dispose() {
        AudioOut?.Dispose();
        Media?.Dispose();
    }
}