using System;
using RetroRecord.Engine.Settings;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [Serializable]
    public class TempoChange : SongItem
    {
        [SerializeField]
        public float Tempo;

        public override SongItemType GetItemType() => SongItemType.TempoChange;
        
        public double TickToTime(uint givenTick)
        {
            double tickDelta = givenTick - Tick;
            double beatDelta = tickDelta / EngineSettings.RESOLUTION;
            double timeDelta = beatDelta * (60 / Tempo);

            return Time + timeDelta;
        }
        

        public uint TimeToTick(double time)
        {
            double timeDelta = time - Time;
            double beatDelta = timeDelta / (60 / Tempo);
            double tickDelta = beatDelta * EngineSettings.RESOLUTION;

            return Tick + (uint) Math.Round(tickDelta);
        }
        
        public new static TempoChange Clone(SongItem item)
        {
            TempoChange newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static TempoChange Clone(TempoChange item)
        {
            TempoChange newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Tempo = item.Tempo
            };
            return newItem;
        }
    }
}