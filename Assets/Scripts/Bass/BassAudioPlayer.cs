using System;
using ManagedBass;
using RetroRecord.Engine;
using UnityEngine;

namespace RetroRecord.Audio
{
    public class BassAudioPlayer : MonoBehaviour
    {
        private AudioClip m_audioClip;
        private int m_streamHandle;
        private float[] m_samples;

        private int m_sampleRate;
        private int m_channels;
        private int m_totalFrames;

        public bool IsLoaded => m_streamHandle != 0;

        public double PlaybackPosition
        {
            get
            {
                if (!IsLoaded)
                    return -1;

                long position = Bass.ChannelGetPosition(m_streamHandle);
                return Bass.ChannelBytes2Seconds(m_streamHandle, position);
            }
        }

        public double Length
        {
            get
            {
                if (m_streamHandle == 0)
                    return -1;

                return (double)m_samples.Length /
                       (m_sampleRate * m_channels);
            }
        }

        public void Load(AudioClip clip)
        {
            FreeStream();

            if (BassManager.s_Instance == null || !BassManager.s_Instance.IsInitialised)
            {
                Debug.LogError("BASS has not been initialized");
                return;
            }

            if (clip == null)
            {
                Debug.LogError("AudioClip is null");
                return;
            }
            
            clip.LoadAudioData();

            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                Debug.LogError("AudioClip is not loaded. Call LoadAudioData() first");
                return;
            }

            m_audioClip = clip;

            m_sampleRate = clip.frequency;
            m_channels = clip.channels;
            m_totalFrames = clip.samples;

            m_samples = new float[m_totalFrames * m_channels];

            if (!clip.GetData(m_samples, 0))
            {
                Debug.LogError("Failed to retrieve AudioClip samples");
                m_samples = null;
                return;
            }

            CreateStreamFromSamples(0);
        }

        private void CreateStreamFromSamples(int startFrame)
        {
            FreeStream();

            if (m_samples == null)
                return;

            m_streamHandle = Bass.CreateStream(
                m_sampleRate,
                m_channels,
                BassFlags.Float,
                StreamProcedureType.Push
            );

            if (m_streamHandle == 0)
            {
                Debug.LogError("Failed to create BASS stream: " + Bass.LastError);
                return;
            }

            int startSample = startFrame * m_channels;
            int remainingSamples = m_samples.Length - startSample;

            if (remainingSamples <= 0)
                return;

            float[] remainingData = new float[remainingSamples];

            Array.Copy(m_samples, startSample, remainingData, 0, remainingSamples);

            int byteLength = remainingData.Length * sizeof(float);

            int result = Bass.StreamPutData(m_streamHandle, remainingData, byteLength);
            
            if (result >= 0) return;

            Debug.LogError("Failed to feed BASS stream: " + Bass.LastError);
            FreeStream();
        }

        public void Play()
        {
            if (m_streamHandle == 0)
            {
                if (m_audioClip == null)
                    return;

                Load(m_audioClip);
            }

            SetPosition(EngineSettings.AudioOffset + EngineSettings.StartTime);
            if (m_streamHandle != 0)
                Bass.ChannelPlay(m_streamHandle);
            
            SetVolume(EngineSettings.Volume);
        }

        public void Pause()
        {
            if (m_streamHandle != 0)
                Bass.ChannelPause(m_streamHandle);
        }

        public void Stop()
        {
            if (m_streamHandle != 0)
                Bass.ChannelStop(m_streamHandle);
        }

        public void SetPosition(double seconds)
        {
            if (m_samples == null || m_sampleRate == 0)
                return;

            int frame = Mathf.Clamp(
                (int)(seconds * m_sampleRate),
                0,
                m_totalFrames
            );

            bool wasPlaying =
                m_streamHandle != 0 &&
                Bass.ChannelIsActive(m_streamHandle) == PlaybackState.Playing;
            
            CreateStreamFromSamples(frame);

            if (wasPlaying && m_streamHandle != 0)
                Bass.ChannelPlay(m_streamHandle);
        }

        public void SetVolume(float volume)
        {
            if (m_streamHandle == 0) return;

            Bass.ChannelSetAttribute(m_streamHandle, ChannelAttribute.Volume, Mathf.Clamp01(volume));
        }
        
        private void FreeStream()
        {
            if (m_streamHandle == 0) return;
            
            Bass.StreamFree(m_streamHandle);
            m_streamHandle = 0;
        }

        private void OnDestroy()
        {
            FreeStream();
            m_samples = null;
        }
    }
}