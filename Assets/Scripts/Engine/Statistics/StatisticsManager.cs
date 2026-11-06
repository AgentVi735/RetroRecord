using UnityEngine;

namespace RetroRecord.Engine
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager s_Instance { get; private set; }

        private uint m_totalNotes;
        private uint m_notesHit;
        private uint m_perfectNotes;
        private uint m_notesMissed;

        public void Initialize()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetTotalNotes(uint totalNotes)
        {
            m_totalNotes = totalNotes;
        }

        public static void Dispose()
        {
            Destroy(s_Instance);
        }

        private void OnDestroy()
        {
            if (s_Instance != this)
                return;

            s_Instance = null;
        }

        public void HitNote(bool isPerfect)
        {
            m_notesHit++;
            if (isPerfect)
                m_perfectNotes++;
        }

        public void MissNote()
        {
            m_notesMissed++;
        }
    }
}
