namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongItem
    {
        public SongItem(uint tick, double time)
        {
            Tick = tick;
            Time = time;
        }
        
        public uint Tick;
        public double Time;

        public virtual SongItemType GetItemType() => SongItemType.SongItem;
    }
}