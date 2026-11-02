using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Note : SongItem
    {
        [field:SerializeField]
        public byte Lane { get; set; }
        [field:SerializeField]
        public uint TickSustainLength { get; set; }
        [field:SerializeField]
        public double TimeSustainLength { get; set; }

        public override SongItemType GetItemType() => SongItemType.Note;
    }
}