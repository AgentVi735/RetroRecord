using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine
{
    public class EngineSettings : MonoBehaviour
    {
        [SerializeField]
        private EngineSettingsObj m_settings;

        public static string CoverFileName = "Cover.png";

        public const int RESOLUTION = 192;

        public static Song Song;

        public static Difficulty Difficulty = Difficulty.Hard;
        public static DifficultySettings CurrentDifficultySettings;

        public static float MinSecUntilNoteDespawn = 1;

        public static double NoteHitboxSec = 0.07;
        public static double PerfectHitboxSec = 0.035;
        public static float HighwaySpeed = 8;

        /// <summary>
        /// Ahead of time in seconds notes should spawn.
        /// </summary>
        public static float NoteSpawnDistance = 2;

        /// <summary>
        /// Threshold in seconds for when to show a cancelled sustain.
        /// </summary>
        public static double MinSecShowStopSustain = 0.1;

        [Range(0, 1)]
        public static float Volume = 1;

        public static double StartTime;

        public static float TimeAfterSong = 1;

        public static double AudioOffset;

        /// <summary>
        /// Beat count for lane colour change effects.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float LaneColorUpdateBeatCount = 1;

        /// <summary>
        /// Beat count for ShowSongName event.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float ShowSongNameBeatCount = 1;

        /// <summary>
        /// Beat count for ShowSongName event.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float ShowSongNameStayBeatCount = 4;

        public void Initialize()
        {
            if (!m_settings) return;

            CoverFileName = m_settings.CoverFileName;

#if UNITY_EDITOR
            if (m_settings.OverrideSettings)
            {
                Song = m_settings.Song;
                Difficulty = m_settings.difficulty;
                Volume = m_settings.Volume;
                StartTime = m_settings.StartTime;
            }
#endif

            CurrentDifficultySettings = Difficulty switch
            {
                Difficulty.Hard => m_settings.HardSettings,
                Difficulty.Normal => m_settings.NormalSettings,
                Difficulty.Easy => m_settings.EasySettings,
                _ => CurrentDifficultySettings
            };

            if (CurrentDifficultySettings != null)
            {
                NoteHitboxSec = CurrentDifficultySettings.NoteHitboxSec > 0
                    ? CurrentDifficultySettings.NoteHitboxSec
                    : m_settings.NoteHitboxInSec;
                PerfectHitboxSec = CurrentDifficultySettings.PerfectHitboxSec > 0
                    ? CurrentDifficultySettings.PerfectHitboxSec
                    : m_settings.PerfectHitboxInSec;
                HighwaySpeed = CurrentDifficultySettings.HighwaySpeed > 0
                    ? CurrentDifficultySettings.HighwaySpeed
                    : m_settings.HighwaySpeed;
            }
            else
            {
                NoteHitboxSec = m_settings.NoteHitboxInSec;
                PerfectHitboxSec = m_settings.PerfectHitboxInSec;
                HighwaySpeed = m_settings.HighwaySpeed;
            }

            NoteSpawnDistance = m_settings.NoteSpawnDistance;
            MinSecShowStopSustain = m_settings.MinSecShowStopSustain;

            MinSecUntilNoteDespawn = m_settings.MinSecUntilNoteDespawn;

            TimeAfterSong = m_settings.TimeAfterSong;

            AudioOffset = m_settings.AudioOffset;

            LaneColorUpdateBeatCount = m_settings.LaneColourUpdateTimeCount;
            ShowSongNameBeatCount = m_settings.ShowSongNameTimeCount;
            ShowSongNameStayBeatCount = m_settings.ShowSongNameStayBeatCount;
        }
    }
}