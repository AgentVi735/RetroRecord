namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class ShowSongInfoEvent : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.ShowSongInfo;
        
        public new static ShowSongInfoEvent Clone(SongItem item)
        {
            ShowSongInfoEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static ShowSongInfoEvent Clone(ShowSongInfoEvent item)
        {
            ShowSongInfoEvent newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
    }
}