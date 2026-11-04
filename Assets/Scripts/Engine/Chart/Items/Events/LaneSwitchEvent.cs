using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class LaneSwitchEvent : SongItem
    {
        [SerializeField]
        public string Lanes;

        public override SongItemType GetItemType() => SongItemType.LaneSwitch;
        
        public new static LaneSwitchEvent Clone(SongItem item)
        {
            LaneSwitchEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static LaneSwitchEvent Clone(LaneSwitchEvent item)
        {
            LaneSwitchEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Lanes = item.Lanes
            };
            return newItem;
        }
    }
}