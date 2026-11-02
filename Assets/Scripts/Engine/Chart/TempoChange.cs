namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TempoChange : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.TempoChange;
    }
}