using RetroRecord.General.UI;
using UnityEngine;

namespace RetroRecord.Library.UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private RecordStoreManager m_recordStoreManager;
        
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

        public void OnQuit()
        {
            m_recordStoreManager.OnQuit();
        }
    }
}