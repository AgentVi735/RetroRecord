using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Song", fileName = "New Song")]
    public class Song : ScriptableObject
    {
        [field:SerializeField]
        public SongInfo SongInfo { get; set; }
        [field:SerializeField]
        public SyncTrack SyncTrack { get; set; }
        [field:SerializeField]
        public Chart[] Charts { get; set; }
        [field:SerializeField]
        public SongItem[] Events { get; set; }
    }
}
