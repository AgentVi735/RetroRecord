using System;
using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [Serializable]
    public class LightsEvent : SongItem
    {
        [SerializeField]
        public uint Index;

        [SerializeField]
        public uint TickUntilNextFlicker;
        
        [SerializeField]
        public double TimeUntilNextFlicker;

        [SerializeField]
        public LightType Type;

        [SerializeField]
        public LightTime TimeType;

        [NonSerialized]
        public NoteGroup NoteGroup;

        public override SongItemType GetItemType() => SongItemType.Light;
        
        public new static LightsEvent Clone(SongItem item)
        {
            LightsEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static LightsEvent Clone(LightsEvent item)
        {
            LightsEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Index = item.Index
            };
            return newItem;
        }
    }
}