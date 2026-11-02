using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Chart
    {
        [field:SerializeField]
        public Difficulty Difficulty { get; set; }
        [field:SerializeField]
        public NoteGroup[] Notes { get; set; }
        [field:SerializeField]
        public SongItem[] LocalEvents { get; set; }
        [field:SerializeField]
        public uint LastTick { get; set; }
    }
}