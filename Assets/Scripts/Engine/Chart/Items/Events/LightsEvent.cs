using System;
using RetroRecord.Engine.Lights;
using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [Serializable]
    public class LightsEvent : SongItem
    {
        [SerializeField]
        public uint Index;

        [SerializeField]
        public uint AltLightTick;
        
        [SerializeField]
        public double AltLightTime;

        [SerializeField]
        public LightType Type;

        [SerializeField]
        public LightTime TimeType;

        [SerializeField]
        public bool ShouldLerp;

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