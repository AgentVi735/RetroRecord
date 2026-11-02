using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Note : SongItem
    {
        public Note(uint tick, double time, Lanes lanes) : base(tick, time)
        {
            Lanes = lanes;
        }

        [field:SerializeField]
        public Lanes Lanes { get; private set; }

        public override SongItemType GetItemType() => SongItemType.Note;
    }
}