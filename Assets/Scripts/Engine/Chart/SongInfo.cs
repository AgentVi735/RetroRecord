using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongInfo
    {
        public SongInfo(string songName, Album album, string overrideArtist, ushort difficulty)
        {
            SongName = songName;
            Album = album;
            OverrideArtist = overrideArtist;
            Difficulty = difficulty;
        }

        [field:SerializeField]
        public string SongName { get; private set; }
        [field:SerializeField]
        public Album Album { get; private set; }
        [field:SerializeField]
        public string OverrideArtist { get; private set; }
        [field:SerializeField]
        public ushort Difficulty { get; private set; }
    }
}