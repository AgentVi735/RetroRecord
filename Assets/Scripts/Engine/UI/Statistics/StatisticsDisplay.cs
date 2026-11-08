using System;
using System.Collections;
using RetroRecord.Engine.Statistics;
using TMPro;
using UnityEngine;

namespace RetroRecord.Engine.UI.Statistics
{
    public class StatisticsDisplay : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_canvasGroup;

        [SerializeField]
        private float m_fadeTime;

        [SerializeField]
        private TMP_Text m_notesHitText;

        [SerializeField]
        private TMP_Text m_perfectNotesHitText;

        [SerializeField]
        private TMP_Text m_notesMissedText;
        
        public void Enable()
        {
            m_canvasGroup.alpha = 0;
            LoadStatistics(StatisticsManager.s_Instance.Statistics);
            gameObject.SetActive(true);
            StartCoroutine(LerpFadeCoroutine(true));
        }
        
        private IEnumerator LerpFadeCoroutine(bool toggle)
        {
            float startAlpha = m_canvasGroup.alpha;
            float endAlpha = toggle ? 1 : 0;

            m_canvasGroup.interactable = false;

            for (float i = 0; i < m_fadeTime; i += Time.unscaledDeltaTime)
            {
                m_canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, i / m_fadeTime);
                yield return null;
            }

            m_canvasGroup.alpha = endAlpha;
            if (!toggle) yield break;
            m_canvasGroup.interactable = true;
        }

        private void LoadStatistics(Engine.Statistics.Statistics statistics)
        {
            float hitPercentageFloat = (float)statistics.NotesHit / statistics.TotalNotes * 100;
            float perfectPercentageFloat = (float)statistics.PerfectNotes / statistics.NotesHit * 100;
            int hitPercentage = Mathf.FloorToInt(hitPercentageFloat);
            int perfectPercentage = Mathf.FloorToInt(perfectPercentageFloat);
            m_notesHitText.text = $"{statistics.NotesHit} / {statistics.TotalNotes} ({hitPercentage}%)";
            m_perfectNotesHitText.text = $"{statistics.PerfectNotes} ({perfectPercentage}%)";
            m_notesMissedText.text = $"{statistics.NotesMissed}";
        }
    }
}