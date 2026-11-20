using UnityEngine;

namespace RetroRecord.Library.Crates
{
    [CreateAssetMenu(menuName = "Settings/Album Objects")]
    public class AlbumObjectSettings : ScriptableObject
    {
        [field: SerializeField]
        public Vector3 DefaultRotation { get; private set; }

        [field: SerializeField]
        public Vector3 FlippedRotation { get; private set; }

        [field: SerializeField]
        public float TimeToFlip { get; private set; }
    }
}