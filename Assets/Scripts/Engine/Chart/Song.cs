using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Song", fileName = "New Song")]
    public class Song : ScriptableObject
    {
        [field:SerializeField]
        public SongInfo SongInfo { get; private set; }
        [field:SerializeField]
        public SyncTrack SyncTrack { get; private set; }
        [field:SerializeField]
        public Chart[] Charts { get; private set; }
    }
}
