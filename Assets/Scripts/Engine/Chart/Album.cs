using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album", fileName = "New Album")]
    public class Album : ScriptableObject
    {
        [field:SerializeField]
        public string AlbumName { get; private set; }
        [field:SerializeField]
        public string ArtistName { get; private set; }
        [field:SerializeField]
        public ushort Year { get; private set; }
        [field:SerializeField]
        public Song[] Songs { get; private set; }
    }
}