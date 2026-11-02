namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongItem
    {
        public ulong Tick;
        public double Time;

        public virtual SongItemType GetItemType() => SongItemType.SongItem;
    }
}