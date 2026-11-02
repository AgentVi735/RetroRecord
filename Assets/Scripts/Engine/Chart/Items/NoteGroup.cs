using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class NoteGroup : SongItem
    {
        [field:SerializeField]
        public Note[] Notes { get; set; }

        public override SongItemType GetItemType() => SongItemType.NoteGroup;
    }
}