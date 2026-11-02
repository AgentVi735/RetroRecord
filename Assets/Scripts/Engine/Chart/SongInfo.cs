using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongInfo
    {
        [field:SerializeField]
        public string SongName { get; set; }
        [field:SerializeField]
        public Album Album { get; set; }
        [field:SerializeField]
        public string OverrideArtist { get; set; }
        [field:SerializeField]
        public ushort Difficulty { get; set; }
        [field:SerializeField]
        public AudioClip Audio { get; set; }
    }
}