using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TempoChange : SongItem
    {
        public TempoChange(uint tick, double time, ulong bpm) : base(tick, time)
        {
            Bpm = bpm;
        }

        [field:SerializeField]
        public ulong Bpm { get; private set; }
        
        public override SongItemType GetItemType() => SongItemType.BpmChange;
    }
}