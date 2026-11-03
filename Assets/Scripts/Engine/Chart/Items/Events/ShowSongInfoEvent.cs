namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class ShowSongInfoEvent : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.LaneSwitch;
    }
}