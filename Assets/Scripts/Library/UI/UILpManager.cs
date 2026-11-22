using System.Linq;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Player;
using RetroRecord.Engine.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroRecord.Library.UI
{
    public class UILpManager : MonoBehaviour
    {
        [SerializeField]
        private Camera m_camera;

        [SerializeField]
        private GameObject m_gameTexture;
        
        [SerializeField]
        private LpSongButton m_buttonPrefab;
        
        private LpSongButton[] m_songButtons;

        [SerializeField]
        private EngineSettingsObj m_engineSettings;

        public void Setup(AlbumLibrary library)
        {
            int highestSongCount = library.Albums.Select(album => album.Songs.Length).Prepend(0).Max();

            if (highestSongCount < 3)
                highestSongCount = 3;
            
            m_songButtons = new LpSongButton[highestSongCount];

            for (int i = 0; i < m_songButtons.Length; i++)
            {
                LpSongButton button = Instantiate(m_buttonPrefab, transform);
                button.Setup(this);
                m_songButtons[i] = button;
            }
        }

        public bool SelectAlbum(Album album)
        {
            if (album.Songs == null || album.Songs.Length == 0)
            {
                foreach (LpSongButton button in m_songButtons)
                    button.Disable();
                return false;
            }
            
            for (int i = 0; i < m_songButtons.Length; i++)
            {
                if (album.Songs.Length <= i)
                    break;
                LpSongButton button = m_songButtons[i];
                button.Load(album.Songs.Length <= m_songButtons.Length ? album.Songs[i] : null);
            }

            return true;
        }
        
        public void DeselectAlbum()
        {
            gameObject.SetActive(false);
            foreach (LpSongButton button in m_songButtons)
                button.Disable();
        }

        public void OnSelectSong(Song song)
        {
            foreach (LpSongButton button in m_songButtons)
                button.Disable();
            m_engineSettings.Song = song;
            m_songButtons[0].Load(Difficulty.Hard);
            m_songButtons[1].Load(Difficulty.Normal);
            m_songButtons[2].Load(Difficulty.Easy);
        }

        public void OnSelectDifficulty(Difficulty difficulty)
        {
            gameObject.SetActive(false);
            m_engineSettings.Difficulty = difficulty;
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            m_gameTexture.SetActive(true);
        }
    }
}