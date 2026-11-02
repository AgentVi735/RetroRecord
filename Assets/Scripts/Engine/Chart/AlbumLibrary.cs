using System.Linq;
using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album Library", fileName = "Album Library")]
    public class AlbumLibrary : ScriptableObject
    {
        [field:SerializeField]
        public Album[] Albums { get; set; }

        public void ReorderAlbums()
        {
            Albums = Albums.OrderBy(album => album.ArtistName)
                .ThenBy(album => album.AlbumName)
                .ToArray();
        }
    }
}