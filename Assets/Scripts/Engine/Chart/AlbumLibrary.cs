using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [CreateAssetMenu(menuName = "Data/Album Library", fileName = "Album Library")]
    public class AlbumLibrary : ScriptableObject
    {
        [field:SerializeField]
        public Album[] Albums { get; private set; }
    }
}