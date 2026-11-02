using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SyncTrack
    {
        [field:SerializeField]
        public BpmChange[] BpmChanges { get; private set; }
        [field:SerializeField]
        public TempoChange[] TempoChanges { get; private set; }
    }
}