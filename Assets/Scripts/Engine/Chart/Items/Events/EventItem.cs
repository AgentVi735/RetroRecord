namespace RetroRecord.Engine.Chart.Events
{
    public class EventItem
    {
        public uint Tick;
        public double Time;
        public SongItemType ItemType;
        public int Index;

        public static EventItem CreateItem(SongItem item, int index)
        {
            EventItem eventItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                ItemType = item.GetItemType(),
                Index = index
            };
            return eventItem;
        }

        public static EventItem CreateItem(LaneSwitchEvent item, int index)
        {
            EventItem eventItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                ItemType = item.GetItemType(),
                Index = index
            };
            return eventItem;
        }

        public static EventItem CreateItem(ShowSongInfoEvent item, int index)
        {
            EventItem eventItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                ItemType = item.GetItemType(),
                Index = index
            };
            return eventItem;
        }
    }
}