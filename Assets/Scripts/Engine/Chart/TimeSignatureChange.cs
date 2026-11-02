namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TimeSignatureChange : SongItem
    {
        public TimeSignatureChange(uint tick, double time, uint numerator, uint denominator) : base(tick, time)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public uint Numerator { get; private set; }
        public uint Denominator { get; private set; }
        
        public override SongItemType GetItemType() => SongItemType.TempoChange;
    }
}