using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TempoChange : SongItem
    {
        [SerializeField]
        public float Tempo;

        public override SongItemType GetItemType() => SongItemType.TempoChange;
        
        public double TickToTime(uint givenTick)
        {
            double tickDelta = givenTick - Tick;
            double beatDelta = tickDelta / EngineSettings.k_RESOLUTION;
            double timeDelta = beatDelta * (60 / Tempo);

            return Time + timeDelta;
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