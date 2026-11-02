using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SyncTrack
    {
        [field:SerializeField]
        public TempoChange[] TempoChanges { get; set; }
        [field:SerializeField]
        public TimeSignatureChange[] TimeSignatureChanges { get; set; }
    }
}