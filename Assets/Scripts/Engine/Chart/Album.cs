using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album", fileName = "New Album")]
    public class Album : ScriptableObject
    {
        [SerializeField]
        public string AlbumName;

        [SerializeField]
        public string ArtistName;

        [SerializeField]
        public int Year;

        [SerializeField]
        public string Genre;

        [SerializeField]
        public Sprite Cover;

        [SerializeField]
        public Song[] Songs;
    }
}