using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class EffectItem : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.Effect;
        
        public EffectType EffectType;

        [SerializeField]
        public uint TickDuration;

        [SerializeField]
        public double TimeDuration;

        public new static EffectItem Clone(SongItem item)
        {
            EffectItem newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }

        public static EffectItem Clone(EffectItem item)
        {
            EffectItem newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                EffectType = item.EffectType,
                TickDuration = item.TickDuration,
                TimeDuration = item.TimeDuration
            };
            return newItem;
        }
    }
}