using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class LocalEventsTrack
    {
        public EventItem[] Items { get; private set; }
        
        [SerializeField]
        public LaneSwitchEvent[] LaneSwitchEvents;
        
        public void SetItems()
        {
            List<EventItem> items = new();
            if (LaneSwitchEvents is { Length: > 0 })
                items.AddRange(LaneSwitchEvents.Select(EventItem.CreateItem));

            Items = items.OrderBy(item => item.Tick).ToArray();
        }
        
        public int GetCurrentTimeIndex(double spawnTime)
        {
            int currentIdx = 0;
            
            for (int i = 0; i < Items.Length; i++)
            {
                EventItem item = Items[i];
                if (item.Time <= spawnTime)
                    currentIdx = i;
                else
                    break;
            }

            return currentIdx;
        }
    }
}