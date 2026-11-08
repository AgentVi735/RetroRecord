using UnityEngine;

namespace RetroRecord.Engine.Statistics
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager s_Instance { get; private set; }

        private Statistics m_statistics;
        public Statistics Statistics => m_statistics;

        public void Initialize()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            m_statistics = new Statistics();
        }

        public void SetTotalNotes(uint totalNotes)
        {
            m_statistics.TotalNotes = totalNotes;
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
            m_statistics.NotesHit++;
            if (isPerfect)
                m_statistics.PerfectNotes++;
        }

        public void MissNote()
        {
            m_statistics.NotesMissed++;
        }
    }
}
