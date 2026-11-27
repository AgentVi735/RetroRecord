using System.Globalization;
using RetroRecord.Engine.Settings;
using RetroRecord.General.UI;
using RetroRecord.Library.UI.MainMenu;
using TMPro;
using UnityEngine;

namespace RetroRecord.Library.UI.Settings
{
    public class SettingsMenuManager : MonoBehaviour
    {
        [SerializeField]
        private EngineSettingsObj m_engineSettings;
        
        [SerializeField]
        private MainMenuManager m_mainMenuManager;

        [SerializeField]
        private UIButton m_returnButton;

        [SerializeField]
        private UIButton m_controlsButton;

        [SerializeField]
        private UISlider m_musicSlider;

        [SerializeField]
        private UISlider m_sfxSlider;

        [SerializeField]
        private TMP_InputField m_audioDelayField;

        private void Awake()
        {
            m_musicSlider.value = m_engineSettings.Volume;
            m_audioDelayField.text = m_engineSettings.AudioOffset.ToString(CultureInfo.InvariantCulture);
        }

        public void Toggle(bool toggle)
        {
            gameObject.SetActive(toggle);
        }
        
        public void OnReturnButton()
        {
            Toggle(false);
            m_mainMenuManager.Toggle(true);
        }

        public void SetMusicVolume(float value)
        {
            m_engineSettings.Volume = value;
        }

        public void SetAudioDelay(string value)
        {
            bool couldParse = float.TryParse(value, out float newValue);
            if (!couldParse)
                return;
            m_engineSettings.AudioOffset = newValue;
        }
    }
}