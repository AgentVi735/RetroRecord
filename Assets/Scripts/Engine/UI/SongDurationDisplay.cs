using RetroRecord.Engine.Player;
using UnityEngine;
using UnityEngine.UI;

namespace RetroRecord.Engine.UI
{
    public class SongDurationDisplay : MonoBehaviour
    {
        [SerializeField]
        private Image m_durationImage;

        public bool IsEnabled;

        private void Update()
        {
            if (!IsEnabled)
                return;

            if (SongPlayer.s_Instance.HasFinished)
            {
                m_durationImage.fillAmount = 1;
                IsEnabled = false;
                return;
            }
            
            m_durationImage.fillAmount = (float)(SongPlayer.CurrentAudioTime / SongPlayer.SongTime);
        }
    }
}