using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine
{
    public class EngineSettings : MonoBehaviour
    {
        [SerializeField]
        private EngineSettingsObj m_settings;

        public static string s_CoverFileName = "Cover.png";

        public const int k_RESOLUTION = 192;

        public static Song s_Song;

        public static Difficulty s_Difficulty = Difficulty.Hard;
        public static DifficultySettings s_CurrentDifficultySettings;

        public static float s_MinSecUntilNoteDespawn = 1;

        public static double s_NoteHitboxSec = 0.07;
        public static double s_PerfectHitboxSec = 0.035;
        public static float s_HighwaySpeed = 8;

        /// <summary>
        /// Ahead of time in seconds notes should spawn.
        /// </summary>
        public static float s_NoteSpawnDistance = 2;

        /// <summary>
        /// Threshold in seconds for when to show a cancelled sustain.
        /// </summary>
        public static double s_MinSecShowStopSustain = 0.1;

        public static float s_Volume = 1;

        public static double s_StartTime;

        public static float s_TimeAfterSong = 1;

        public static double s_AudioOffset;

        /// <summary>
        /// Beat count for lane colour change effects.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float s_LaneColorUpdateBeatCount = 1;

        /// <summary>
        /// Beat count for ShowSongName event.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float s_ShowSongNameBeatCount = 1;

        /// <summary>
        /// Beat count for ShowSongName event.
        /// 1 = Quarter beat, 2 = half beat, 4 = whole beat
        /// </summary>
        public static float s_ShowSongNameStayBeatCount = 4;

        public void Initialize()
        {
            if (!m_settings) return;

            s_CoverFileName = m_settings.CoverFileName;

#if UNITY_EDITOR
            if (m_settings.OverrideSettings)
            {
                s_Song = m_settings.Song;
                s_Difficulty = m_settings.difficulty;
                s_Volume = m_settings.Volume;
                s_StartTime = m_settings.StartTime;
            }
#endif

            s_CurrentDifficultySettings = s_Difficulty switch
            {
                Difficulty.Hard => m_settings.HardSettings,
                Difficulty.Normal => m_settings.NormalSettings,
                Difficulty.Easy => m_settings.EasySettings,
                _ => s_CurrentDifficultySettings
            };

            if (s_CurrentDifficultySettings != null)
            {
                s_NoteHitboxSec = s_CurrentDifficultySettings.NoteHitboxSec > 0
                    ? s_CurrentDifficultySettings.NoteHitboxSec
                    : m_settings.NoteHitboxInSec;
                s_PerfectHitboxSec = s_CurrentDifficultySettings.PerfectHitboxSec > 0
                    ? s_CurrentDifficultySettings.PerfectHitboxSec
                    : m_settings.PerfectHitboxInSec;
                s_HighwaySpeed = s_CurrentDifficultySettings.HighwaySpeed > 0
                    ? s_CurrentDifficultySettings.HighwaySpeed
                    : m_settings.HighwaySpeed;
            }
            else
            {
                s_NoteHitboxSec = m_settings.NoteHitboxInSec;
                s_PerfectHitboxSec = m_settings.PerfectHitboxInSec;
                s_HighwaySpeed = m_settings.HighwaySpeed;
            }

            s_NoteSpawnDistance = m_settings.NoteSpawnDistance;
            s_MinSecShowStopSustain = m_settings.MinSecShowStopSustain;

            s_MinSecUntilNoteDespawn = m_settings.MinSecUntilNoteDespawn;

            s_TimeAfterSong = m_settings.TimeAfterSong;

            s_AudioOffset = m_settings.AudioOffset;

            s_LaneColorUpdateBeatCount = m_settings.LaneColourUpdateTimeCount;
            s_ShowSongNameBeatCount = m_settings.ShowSongNameTimeCount;
            s_ShowSongNameStayBeatCount = m_settings.ShowSongNameStayBeatCount;
        }
    }
}