﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WeakestLinkGameTool.Helpers;

namespace WeakestLinkGameTool.Logic.Sounds;

public class Sound : IDisposable {
    
    /// <summary>
    /// 
    /// </summary>
    public LoopAudioReader Media { get; private set; }
    
    /// <summary>
    /// 
    /// </summary>
    public WasapiOut AudioOut { get; private set; }
    
    /// <summary>
    /// 
    /// </summary>
    public FadeInOutSampleProvider Fade { get; private set; }
    
    /// <summary>
    /// 
    /// </summary>
    public VolumeWaveProvider16 VolumeProvider { get; private set; }
    
    /// <summary>
    /// 
    /// </summary>
    public float VolumeCoefficient { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public float Volume {
        get => VolumeProvider.Volume; 
        set => VolumeProvider.Volume = value * VolumeCoefficient;
    }

    public Sound(LoopAudioReader media, WasapiOut audioOut, FadeInOutSampleProvider fade, VolumeWaveProvider16 volumeProvider) {
        Media = media;
        AudioOut = audioOut;
        Fade = fade;
        VolumeProvider = volumeProvider;
        VolumeCoefficient = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coefficient"></param>
    public void SetVolumeCoefficient(float coefficient) {
        VolumeCoefficient = coefficient;
        Volume = VolumeProvider.Volume;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void Dispose() {
        AudioOut?.Dispose();
        Media?.Dispose();
    }
}