using RetroRecord.Engine.Chart.Events;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Song", fileName = "New Song")]
    public class Song : ScriptableObject
    {
        [SerializeField]
        public SongInfo SongInfo;

        [SerializeField]
        public BeatTrack BeatTrack;

        [SerializeField]
        public Chart[] Charts;

        [SerializeField]
        public GlobalEventsTrack GlobalEvents;
    }
}
