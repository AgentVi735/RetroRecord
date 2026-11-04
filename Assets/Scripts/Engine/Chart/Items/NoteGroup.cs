using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class NoteGroup : SongItem
    {
        [SerializeField]
        public Note[] Notes;

        public override SongItemType GetItemType() => SongItemType.NoteGroup;
        
        public new static NoteGroup Clone(SongItem item)
        {
            NoteGroup newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time
            };
            return newItem;
        }
        
        public static NoteGroup Clone(NoteGroup item)
        {
            NoteGroup newItem = new()
            {
                Tick = item.Tick,
                Time = item.Time,
                Notes = item.Notes
            };
            return newItem;
        }
    }
}