using System;
using System.Collections;
using System.Collections.Generic;
using RetroRecord.General.UI;
using RetroRecord.Library.Crates;
using RetroRecord.Library.UI.MainMenu;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private Image m_backgroundImage;
        
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
            yield return null;
            
            CaptureBackground(m_cameraManager.Camera);
        }

        private void CaptureBackground(Camera cam)
        {
            List<DisplayInfo> displayLayout = new();
            Screen.GetDisplayLayout(displayLayout);

            int width = displayLayout[cam.targetDisplay].width;
            int height = displayLayout[cam.targetDisplay].height;


            RenderTexture renderTexture = new(
                width,
                height,
                24,
                RenderTextureFormat.ARGB32
            )
            {
                antiAliasing = 1
            };

            RenderTexture previousTarget = cam.targetTexture;
            CameraClearFlags previousClearFlags = cam.clearFlags;
            Color previousBackgroundColor = cam.backgroundColor;

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.targetTexture = renderTexture;

            cam.Render();

            RenderTexture previousActive = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D screenshot = new(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );

            screenshot.ReadPixels(
                new Rect(0, 0, width, height),
                0,
                0
            );

            screenshot.Apply();

            byte[] pngData = screenshot.EncodeToPNG();

            Texture2D tex = new(1, 1);
            tex.LoadImage(pngData);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(tex.width / 2, tex.height / 2));
            m_backgroundImage.sprite = sprite;

            cam.targetTexture = previousTarget;
            cam.clearFlags = previousClearFlags;
            cam.backgroundColor = previousBackgroundColor;

            RenderTexture.active = previousActive;

            DestroyImmediate(screenshot);
            renderTexture.Release();
            DestroyImmediate(renderTexture);

            m_backgroundImage.gameObject.SetActive(true);
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            m_backgroundImage.gameObject.SetActive(true);
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
            m_backgroundImage.gameObject.SetActive(false);
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

        private void OnDestroy()
        {
            m_libraryInputManager.Dispose();
        }
    }
}
