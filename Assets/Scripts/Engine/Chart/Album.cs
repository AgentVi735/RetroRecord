using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album", fileName = "New Album")]
    public class Album : ScriptableObject
    {
        public static Album Create(string AlbumName, string ArtistName, ushort Year, Sprite Cover, Song[] Songs)
        {
            Album newAlbum = CreateInstance<Album>();
            newAlbum.AlbumName = AlbumName;
            newAlbum.ArtistName = ArtistName;
            newAlbum.Year = Year;
            newAlbum.Cover = Cover;
            newAlbum.Songs = Songs;
            return newAlbum;
        }
        
        [field:SerializeField]
        public string AlbumName { get; private set; }
        [field:SerializeField]
        public string ArtistName { get; private set; }
        [field:SerializeField]
        public ushort Year { get; private set; }
        [field:SerializeField]
        public Sprite Cover { get; private set; }
        [field:SerializeField]
        public Song[] Songs { get; private set; }
    }
}