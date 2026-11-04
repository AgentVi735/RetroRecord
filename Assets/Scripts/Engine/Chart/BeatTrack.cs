using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class BeatTrack
    {
        [SerializeField]
        public TempoChange[] TempoChanges;
        
        [SerializeField]
        public TimeSignatureChange[] TimeSignatureChanges;
    }
}