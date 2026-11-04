using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine
{
    [CreateAssetMenu(menuName = "Settings/Engine")]
    public class EngineSettingsObj : ScriptableObject
    {
        public string CoverFileName = "Cover.png";

#if UNITY_EDITOR
        public bool OverrideSettings;
        public Song Song;
        public Difficulty difficulty = Difficulty.Hard;

        [Range(0f, 1f)]
        public float Volume = 1f;

        public double StartTime = 0;
#endif

        public DifficultySettings HardSettings;
        public DifficultySettings NormalSettings;
        public DifficultySettings EasySettings;

        public float MinSecUntilNoteDespawn = 1;

        public double NoteHitboxInSec = 0.07;
        public double PerfectHitboxInSec = 0.035;
        public float HighwaySpeed = 8;
        public float NoteSpawnDistance = 2;
        public double MinSecShowStopSustain = 0.15;

        public float TimeAfterSong = 1;

        public double AudioOffset;

        public float LaneColourUpdateTimeCount = 1;
        public float ShowSongNameTimeCount = 1;
        public float ShowSongNameStayBeatCount = 4;
    }
}