using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album", fileName = "New Album")]
    public class Album : ScriptableObject
    {
        [field:SerializeField]
        public string AlbumName { get; set; }
        [field:SerializeField]
        public string ArtistName { get; set; }
        [field:SerializeField]
        public int Year { get; set; }
        [field:SerializeField]
        public string Genre { get; set; }
        [field:SerializeField]
        public Sprite Cover { get; set; }
        [field:SerializeField]
        public Song[] Songs { get; set; }
    }
}