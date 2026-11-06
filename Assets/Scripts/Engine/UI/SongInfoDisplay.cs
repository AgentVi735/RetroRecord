using System.Collections;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRecord.Engine.UI
{
    public class SongInfoDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_songNameText;

        [SerializeField]
        private TMP_Text m_artistNameText;

        [SerializeField]
        private TMP_Text m_albumNameText;

        [SerializeField]
        private Behaviour[] m_displayComponents;

        [SerializeField]
        private CanvasGroup m_canvasGroup;

#if UNITY_EDITOR
        [ContextMenu("Find Display Components")]
        public void FindComponents()
        {
            ContentSizeFitter[] sizeFitters = gameObject.GetComponentsInChildren<ContentSizeFitter>();
            VerticalLayoutGroup verticalLayoutGroup = gameObject.GetComponent<VerticalLayoutGroup>();
            m_displayComponents = new Behaviour[sizeFitters.Length + 1];
            for (int i = 0; i < sizeFitters.Length; i++)
                m_displayComponents[i] = sizeFitters[i];
            m_displayComponents[^1] = verticalLayoutGroup;
        }
#endif
        
        public IEnumerator Initialize(SongInfo songInfo)
        {
            m_canvasGroup.alpha = 0;
            foreach (Behaviour component in m_displayComponents)
                component.enabled = true;
            gameObject.SetActive(true);
            m_songNameText.text = songInfo.SongName;
            m_artistNameText.text = songInfo.Album.ArtistName;
            m_albumNameText.text = songInfo.Album.AlbumName;
            yield return null;
            yield return null;
            yield return null;
            gameObject.SetActive(false);
        }
        
        public void ShowSongNameEvent(double eventTime)
        {
            m_canvasGroup.alpha = 0;
            m_canvasGroup.gameObject.SetActive(true);
            StartCoroutine(ShowSongNameLoop(eventTime));
        }

        private IEnumerator ShowSongNameLoop(double eventTime)
        {

            float startTime = (float)eventTime;
            float totalTime = 60 * EngineSettings.s_ShowSongNameBeatCount / NotesManager.s_CurrentTempo;
            float time = startTime + totalTime;
            while (SongPlayer.CurrentAudioTime < time)
            {
                float amt = Mathf.InverseLerp(startTime, time, (float)SongPlayer.CurrentAudioTime);
                m_canvasGroup.alpha = amt;
                yield return null;
            }

            m_canvasGroup.alpha = 1;
            float waitTime = (float)SongPlayer.CurrentAudioTime +
                             60 * EngineSettings.s_ShowSongNameStayBeatCount / NotesManager.s_CurrentTempo;
            while (SongPlayer.CurrentAudioTime < waitTime)
                yield return null;
        
            startTime = (float)SongPlayer.CurrentAudioTime;
            totalTime = 60 * EngineSettings.s_ShowSongNameBeatCount / NotesManager.s_CurrentTempo;
            time = startTime + totalTime;
        
            while (SongPlayer.CurrentAudioTime < time)
            {
                float amt = Mathf.InverseLerp(time, startTime, (float)SongPlayer.CurrentAudioTime);
                m_canvasGroup.alpha = amt;
                yield return null;
            }

            m_canvasGroup.alpha = 0;
            m_canvasGroup.gameObject.SetActive(false);
        }
    }
}