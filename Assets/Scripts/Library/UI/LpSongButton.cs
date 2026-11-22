using RetroRecord.Engine.Chart;
using RetroRecord.General.UI;
using TMPro;
using UnityEngine;

namespace RetroRecord.Library.UI
{
    public class LpSongButton : UIButton
    {
        [SerializeField]
        private TMP_Text m_text;
        
        private Song m_song;

        private Difficulty m_difficulty;

        private UILpManager m_uiLpManager;
        
        public void Setup(UILpManager lpManager)
        {
            gameObject.SetActive(false);
            m_uiLpManager = lpManager;
        }

        public void Load(Song song)
        {
            m_song = song;

            if (m_song != null)
            {
                interactable = true;
                m_text.text = m_song.SongInfo.SongName;
            }
            else
            {
                interactable = false;
                m_text.text = "???";
            }
            
            gameObject.SetActive(true);
        }

        public void Load(Difficulty difficulty)
        {
            m_song = null;
            m_difficulty = difficulty;
            
            interactable = true;
            m_text.text = difficulty.ToString();

            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void OnPress()
        {
            if (m_song != null)
                m_uiLpManager.OnSelectSong(m_song);
            else
                m_uiLpManager.OnSelectDifficulty(m_difficulty);
        }
    }
}