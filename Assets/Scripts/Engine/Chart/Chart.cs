using RetroRecord.Engine.Chart.Events;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Chart
    {
        [SerializeField]
        public Difficulty Difficulty;

        [SerializeField]
        public NoteGroup[] Notes;

        [SerializeField]
        public LocalEventsTrack LocalEvents;

        [SerializeField]
        public uint LastTick;
    }
}