using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongInfo
    {
        [SerializeField]
        public string SongName;
        
        [SerializeField]
        public Album Album;
        
        [SerializeField]
        public string OverrideArtist;
        
        [SerializeField]
        public string Charter;
        
        [SerializeField]
        public ushort Difficulty;
        
        [SerializeField]
        public AudioClip Audio;
    }
}