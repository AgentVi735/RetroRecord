using RetroRecord.Audio;
using RetroRecord.Engine.Statistics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace RetroRecord.Engine.Player
{
    public class SongPlayer : MonoBehaviour
    {
        [SerializeField]
        private BassAudioPlayer m_audioPlayer;

        public static SongPlayer s_Instance;

        /// <summary>
        /// The current Audio Time, used for visual times.
        /// </summary>
        public static double CurrentAudioTime { get; private set; }

        public static double CurrentInputTime { get; private set; }
        public static double InputTimeOffset { get; private set; }

        /// <summary>
        /// The current Input Time, used for checking input times.
        /// </summary>
        public static double ConvertedCurrentInputTime { get; private set; }

        public double AudioOffset { get; private set; }

        public bool HasStarted { get; private set; }
        public bool HasFinished { get; private set; }

        public static double SongTime;

        public void Initialize()
        {
            if (s_Instance != null)
            {
                Destroy(s_Instance.gameObject);
            }

            s_Instance = this;
        }

        public void LoadSong()
        {
            HasStarted = false;
            CurrentAudioTime = 0;
            CurrentInputTime = 0;
            InputTimeOffset = 0;
            ConvertedCurrentInputTime = 0;
            HasFinished = false;
            m_audioPlayer.Load(EngineSettings.Song.SongInfo.Audio);

            SongTime = m_audioPlayer.Length;
        }

        public void StartMusic()
        {
            HasStarted = true;
            m_audioPlayer.Play();

            InputTimeOffset = InputState.currentTime - EngineSettings.StartTime;
            AudioOffset = EngineSettings.StartTime + EngineSettings.AudioOffset;
            UpdateTimes();
        }

        private void UpdateTimes()
        {
            CurrentInputTime = InputState.currentTime;
            ConvertedCurrentInputTime = CurrentInputTime - InputTimeOffset;
            CurrentAudioTime = m_audioPlayer.PlaybackPosition + AudioOffset;
        }

        private void Update()
        {
            if (!HasStarted)
                return;

            UpdateTimes();
            
            if (CurrentAudioTime >= SongTime)
                Finished();
        }

        private void Finished()
        {
            HasFinished = true;
            HasStarted = false;
            EngineManager.s_Instance.Finished();
        }
    }
}