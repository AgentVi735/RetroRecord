using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class TempoChange : SongItem
    {
        [field: SerializeField]
        public float Tempo { get; set; }

        public override SongItemType GetItemType() => SongItemType.TempoChange;
    }
}