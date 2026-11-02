namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class BpmChange : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.BpmChange;
    }
}