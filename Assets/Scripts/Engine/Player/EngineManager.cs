using System.Collections;
using RetroRecord.Audio;
using RetroRecord.Engine.Player.Renderer;
using RetroRecord.Engine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RetroRecord.Engine.Player
{
    public class EngineManager : MonoBehaviour
    {
        public static EngineManager s_Instance;
        
        [SerializeField]
        private EngineSettings m_engineSettings;
        
        [SerializeField]
        private BassManager m_bassManager;
        
        [SerializeField]
        private NotesManager m_notesManager;

        [SerializeField]
        private HighwayRenderer m_highwayRenderer;

        [SerializeField]
        private SongPlayer m_songPlayer;

        [SerializeField]
        private StatisticsManager m_statisticsManager;

        [SerializeField]
        private SongInfoDisplay m_songInfoDisplay;

        [SerializeField]
        private SongDurationDisplay m_songDurationDisplay;

        [SerializeField]
        private bool m_autoStart;

        [SerializeField]
        private float m_delayUntilStart;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            s_Instance = this;
            m_engineSettings.Initialize();
            m_bassManager.Initialize();
            m_songPlayer.Initialize();
            m_songPlayer.LoadSong();
            m_highwayRenderer.Initialize();
            m_statisticsManager.Initialize();
            m_notesManager.Initialize();
            StartCoroutine(m_songInfoDisplay.Initialize(EngineSettings.s_Song.SongInfo));
        }

        private void Start()
        {
            if (m_autoStart)
                StartCoroutine(StartSong());
        }

        private IEnumerator StartSong()
        {
            yield return new WaitForSeconds(m_delayUntilStart);
            m_songDurationDisplay.IsEnabled = true;
            m_songPlayer.StartMusic();
        }

        public void Finished()
        {
            StatisticsManager.Dispose();
            StartCoroutine(FinishWait());
        }

        private IEnumerator FinishWait()
        {
            yield return new WaitForSeconds(EngineSettings.s_TimeAfterSong);

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }
}