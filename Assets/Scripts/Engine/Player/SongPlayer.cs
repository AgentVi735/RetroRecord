using RetroRecord.Audio;
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
        public double CurrentAudioTime { get; private set; }

        public double CurrentInputTime { get; private set; }
        public double InputTimeOffset { get; private set; }

        /// <summary>
        /// The current Input Time, used for checking input times.
        /// </summary>
        public double ConvertedCurrentInputTime { get; private set; }

        public double AudioOffset { get; private set; }

        public bool HasStarted { get; private set; }
        public bool HasFinished { get; private set; }

        private double m_songTime;

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
            m_audioPlayer.Load(EngineSettings.s_Song.SongInfo.Audio);

            m_songTime = m_audioPlayer.Duration;
        }

        public void StartMusic()
        {
            HasStarted = true;
            m_audioPlayer.Play();

            InputTimeOffset = InputState.currentTime - 0;
            UpdateTimes();
        }

        private void UpdateTimes()
        {
            CurrentInputTime = InputState.currentTime;
            ConvertedCurrentInputTime = CurrentInputTime - InputTimeOffset;
            CurrentAudioTime = m_audioPlayer.PlaybackPosition - AudioOffset;
        }

        private void Update()
        {
            if (!HasStarted)
                return;

            UpdateTimes();

            if (!(CurrentAudioTime >= m_songTime)) return;
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