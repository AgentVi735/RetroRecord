using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SyncTrack
    {
        private SyncTrack(TempoChange[] tempoChanges, TimeSignatureChange[] timeSignatureChanges)
        {
            TempoChanges = tempoChanges;
            TimeSignatureChanges = timeSignatureChanges;
        }

        [field:SerializeField]
        public TempoChange[] TempoChanges { get; private set; }
        [field:SerializeField]
        public TimeSignatureChange[] TimeSignatureChanges { get; private set; }
    }
}