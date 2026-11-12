using System;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [Serializable]
    public class Note : SongItem
    {
        [SerializeField]
        public byte Lane;

        [SerializeField]
        public uint TickSustainLength;

        [SerializeField]
        public double TimeSustainLength;
        
        [NonSerialized]
        public bool WasHit = false;

        public override SongItemType GetItemType() => SongItemType.Note;
        
        public new static Note Clone(SongItem item)
        {
            Note newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static Note Clone(Note item)
        {
            Note newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Lane = item.Lane,
                TickSustainLength = item.TickSustainLength,
                TimeSustainLength = item.TimeSustainLength,
                WasHit = item.WasHit
            };
            return newItem;
        }
    }
}