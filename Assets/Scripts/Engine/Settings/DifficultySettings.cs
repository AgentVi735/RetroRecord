using UnityEngine;

namespace RetroRecord.Engine.Settings
{
    [CreateAssetMenu(menuName = "Settings/Difficulty")]
    public class DifficultySettings : ScriptableObject
    {
        public int LaneAmount;
        public int StartingLane;
        
        /// <summary>
        /// If true, then there is no left or right lanes.
        /// All lanes will be positioned in the middle and can be hit with either the left or right lane keybinds.
        /// </summary>
        [field: SerializeField]
        public bool AreLanesInMiddle { get; private set; }

        public float HighwaySpeed = 12;
        public double NoteHitboxSec = 0.07;
        public double PerfectHitboxSec = 0.035;
    }
}