using System;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [Serializable]
    public class NoteGroup : SongItem
    {
        [SerializeField]
        public Note[] Notes;
        
        /// <summary>
        /// Ticks until next flicker note if this Note Group has a flicker note
        /// </summary>
        [NonSerialized]
        public uint TicksUntilNextNote;
        
        /// <summary>
        /// Time in seconds until next flicker note if this Note Group has a flicker note
        /// </summary>
        [NonSerialized]
        public double TimeUntilNextNote;

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