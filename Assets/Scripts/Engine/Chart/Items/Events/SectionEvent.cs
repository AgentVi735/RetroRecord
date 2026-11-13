using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class SectionEvent : SongItem
    {
        [SerializeField]
        public string SectionName;
        
        public override SongItemType GetItemType() => SongItemType.Section;
        
        public new static SectionEvent Clone(SongItem item)
        {
            SectionEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static SectionEvent Clone(SectionEvent item)
        {
            SectionEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
    }
}