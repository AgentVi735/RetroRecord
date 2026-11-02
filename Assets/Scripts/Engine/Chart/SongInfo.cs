using UnityEngine;

namespace RetroRecord.Engine.Chart
{
    [System.Serializable]
    public class SongInfo
    {
        public SongInfo(string songName, Album album, string overrideArtist, ushort difficulty, AudioClip audio)
        {
            SongName = songName;
            Album = album;
            OverrideArtist = overrideArtist;
            Difficulty = difficulty;
            Audio = audio;
        }

        [field:SerializeField]
        public string SongName { get; private set; }
        [field:SerializeField]
        public Album Album { get; private set; }
        [field:SerializeField]
        public string OverrideArtist { get; private set; }
        [field:SerializeField]
        public ushort Difficulty { get; private set; }
        [field:SerializeField]
        public AudioClip Audio { get; private set; }
    }
}