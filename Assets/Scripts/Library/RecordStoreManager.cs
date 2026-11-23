using System.Collections;
using RetroRecord.General.UI;
using RetroRecord.Library.Crates;
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

        public void OnSelectSong()
        {
            StartCoroutine(OnSelectSong_Internal());
        }
        
        private IEnumerator OnSelectSong_Internal()
        {
            yield return StartCoroutine(m_cameraManager.MoveToTurntable());
            foreach (GameObject obj in m_objectToHide)
                obj.SetActive(false);
            yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            m_gameTexture.SetActive(true);
        }
    }
}