using System.Collections;
using RetroRecord.General.UI;
using RetroRecord.Library.Crates;
using RetroRecord.Library.UI.MainMenu;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroRecord.Library
{
    public class RecordStoreManager : MonoBehaviour
    {
        [SerializeField]
        private CameraManager m_mainMenuCameraManager;
        
        [SerializeField]
        private CameraManager m_cameraManager;

        [SerializeField]
        private StoreDoor m_storeDoor;
        
        [SerializeField]
        private FadeManager m_fadeManager;

        [SerializeField]
        private LibraryInputManager m_libraryInputManager;

        [SerializeField]
        private MainMenuManager m_mainMenuManager;
        
        [SerializeField]
        private CrateManager m_crateManager;

        [SerializeField]
        private GameObject m_gameTexture;

        [SerializeField]
        private GameObject[] m_objectToHide;

        private void Awake()
        {
            m_crateManager.Load();
            m_libraryInputManager.Toggle(false);
            m_libraryInputManager.Setup();
        }

        public void OnStart()
        {
            StartCoroutine(OnStart_Internal());
        }

        private IEnumerator OnStart_Internal()
        {
            yield return m_storeDoor.OpenDoor();
            
            m_fadeManager.StartFade(false, EnterStore);
        }

        private void EnterStore()
        {
            m_cameraManager.gameObject.SetActive(true);
            m_mainMenuCameraManager.gameObject.SetActive(false);
            
            m_storeDoor.CloseDoor();
            
            m_fadeManager.StartFade(true, EnableLibrary);
        }

        private void EnableLibrary()
        {
            m_cameraManager.MoveCamera(m_crateManager.GetCurrentPosition());
            m_libraryInputManager.Toggle(true);
        }

        public void OnSelectSong() => StartCoroutine(OnSelectSong_Internal());

        private IEnumerator OnSelectSong_Internal()
        {
            m_libraryInputManager.Toggle(false);
            yield return StartCoroutine(m_cameraManager.MoveToTurntable());
            foreach (GameObject obj in m_objectToHide)
                obj.SetActive(false);
            yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            m_gameTexture.SetActive(true);
        }

        public void OnEndSong() => StartCoroutine(OnEndSong_Internal());

        private IEnumerator OnEndSong_Internal()
        {
            m_gameTexture.SetActive(false);
            yield return SceneManager.UnloadSceneAsync(1);
            m_libraryInputManager.UnselectAlbum();
            foreach (GameObject obj in m_objectToHide)
                obj.SetActive(true);
            yield return StartCoroutine(m_cameraManager.MoveCamera_Coroutine(m_crateManager.GetCurrentPosition()));
            m_libraryInputManager.Toggle(true);
        }

        public void DisableLibrary()
        {
            m_libraryInputManager.Toggle(false);
            m_fadeManager.StartFade(false, LeaveStore);
        }

        private void LeaveStore()
        {
            m_cameraManager.gameObject.SetActive(false);
            m_mainMenuCameraManager.gameObject.SetActive(true);
            m_mainMenuManager.Toggle(true);
            
            m_fadeManager.StartFade(true);
        }

        public void OnQuit()
        {
            m_fadeManager.StartFade(false, ExitGame);
        }
        
        private static void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }
}
