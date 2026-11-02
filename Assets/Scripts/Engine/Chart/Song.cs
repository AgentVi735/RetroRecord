using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Song", fileName = "New Song")]
    public class Song : ScriptableObject
    {
        public static Song Create(SongInfo SongInfo, SyncTrack SyncTrack, Chart[] Charts)
        {
            Song newSong = CreateInstance<Song>();
            newSong.SongInfo = SongInfo;
            newSong.SyncTrack = SyncTrack;
            newSong.Charts = Charts;
            return newSong;
        }
        
        [field:SerializeField]
        public SongInfo SongInfo { get; private set; }
        [field:SerializeField]
        public SyncTrack SyncTrack { get; private set; }
        [field:SerializeField]
        public Chart[] Charts { get; private set; }
    }
}
