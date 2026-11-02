namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongItem
    {
        public uint Tick;
        public double Time;

        public virtual SongItemType GetItemType() => SongItemType.SongItem;
    }
}