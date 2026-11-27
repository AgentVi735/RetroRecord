using RetroRecord.General.UI;
using RetroRecord.Library.UI.Settings;
using UnityEngine;

namespace RetroRecord.Library.UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private RecordStoreManager m_recordStoreManager;

        [SerializeField]
        private SettingsMenuManager m_settingsMenuManager;
        
        [SerializeField]
        private UIButton m_startButton;
        
        [SerializeField]
        private UIButton m_quitButton;

        public void Toggle(bool toggle)
        {
            gameObject.SetActive(toggle);
        }
        
        public void OnStart()
        {
            gameObject.SetActive(false);
            m_recordStoreManager.OnStart();
        }

        public void OnSettings()
        {
            Toggle(false);
            m_settingsMenuManager.Toggle(true);
        }

        public void OnQuit()
        {
            m_recordStoreManager.OnQuit();
        }
    }
}