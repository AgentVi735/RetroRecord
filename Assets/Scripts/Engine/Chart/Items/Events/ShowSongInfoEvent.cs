using UnityEngine;

namespace RetroRecord.Engine.Chart.Events
{
    [System.Serializable]
    public class ShowSongInfoEvent : SongItem
    {
        [field: SerializeField]
        public int Lanes { get; set; }

        public override SongItemType GetItemType() => SongItemType.LaneSwitch;
    }
}