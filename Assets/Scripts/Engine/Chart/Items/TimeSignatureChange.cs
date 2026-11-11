using System;
using RetroRecord.Engine.Settings;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [Serializable]
    public class TimeSignatureChange : SongItem
    {
        [SerializeField]
        public uint Numerator;
        [SerializeField]
        public uint Denominator;

        [NonSerialized]
        public double QuarterNoteCount;
        
        public override SongItemType GetItemType() => SongItemType.TimeSignature;
        
        public double GetQuarterNoteProgress(uint tick)
        {
            return (tick - Tick) / (double) EngineSettings.RESOLUTION;
        }
        
        public new static TimeSignatureChange Clone(SongItem item)
        {
            TimeSignatureChange newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static TimeSignatureChange Clone(TimeSignatureChange item)
        {
            TimeSignatureChange newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Numerator = item.Numerator,
                Denominator = item.Denominator
            };
            return newItem;
        }
    }
}