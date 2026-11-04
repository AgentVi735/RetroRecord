using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class GlobalEventsTrack
    {
        public EventItem[] Items { get; private set; }
        
        [SerializeField]
        public ShowSongInfoEvent[] ShowSongInfoEvents;

        public void SetItems()
        {
            List<EventItem> items = new();
            if (ShowSongInfoEvents is { Length: > 0 })
                items.AddRange(ShowSongInfoEvents.Select(EventItem.CreateItem));

            Items = items.ToArray();
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