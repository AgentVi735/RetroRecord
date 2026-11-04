using System.Collections;
using RetroRecord.Engine.Player.Renderer;
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
        private NotesManager m_notesManager;

        [SerializeField]
        private HighwayRenderer m_highwayRenderer;

        [SerializeField]
        private SongPlayer m_songPlayer;

        private void Start()
        {
            Initialize(true);
        }

        public void Initialize(bool shouldAutoStart)
        {
            s_Instance = this;
            m_songPlayer.LoadSong();
            m_highwayRenderer.Initialize();
            m_notesManager.Initialize();

            if (shouldAutoStart)
                Invoke(nameof(StartSong), 0.1f);
        }

        public void StartSong()
        {
            m_songPlayer.StartMusic();
        }

        public void Finished()
        {
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