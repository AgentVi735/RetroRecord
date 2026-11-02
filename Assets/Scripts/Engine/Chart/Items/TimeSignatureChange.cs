namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TimeSignatureChange : SongItem
    {
        public uint Numerator { get; set; }
        public uint Denominator { get; set; }
        
        public override SongItemType GetItemType() => SongItemType.TimeSignature;
    }
}