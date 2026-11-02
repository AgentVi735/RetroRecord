using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class Note : SongItem
    {
        public override SongItemType GetItemType() => SongItemType.Note;
        
        [field:SerializeField]
        public Lanes Lanes { get; private set; }
    }
}