using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongItem
    {
        [SerializeField]
        public uint Tick;
        [SerializeField]
        public double Time;
        
        public virtual SongItemType GetItemType() => SongItemType.SongItem;

        public static SongItem Clone(SongItem item)
        {
            SongItem newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
    }
}