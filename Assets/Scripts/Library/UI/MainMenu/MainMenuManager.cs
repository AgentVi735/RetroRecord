using RetroRecord.General.UI;
using UnityEditor;
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

        public void OnStart()
        {
            gameObject.SetActive(false);
            m_recordStoreManager.OnStart();
        }

        public void OnQuit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }
}