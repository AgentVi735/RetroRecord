using RetroRecord.Library.Crates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Library
{
    public class LibraryInputManager : MonoBehaviour
    {
        [SerializeField]
        private CameraManager m_cameraManager;
        
        [SerializeField]
        private CrateManager m_crateManager;

        [SerializeField]
        private InputActionAsset m_inputActionAsset;

        [SerializeField]
        private string m_forwardInputPath;
        private InputAction m_forwardInput;

        [SerializeField]
        private string m_backwardInputPath;
        private InputAction m_backwardInput;

        [SerializeField]
        private string m_selectInputPath;
        private InputAction m_selectInput;

        private void Awake()
        {
            m_forwardInput = m_inputActionAsset.FindAction(m_forwardInputPath);
            m_backwardInput = m_inputActionAsset.FindAction(m_backwardInputPath);
            m_selectInput = m_inputActionAsset.FindAction(m_selectInputPath);

            m_forwardInput.started += OnForward;
            m_backwardInput.started += OnBackward;
            m_selectInput.started += OnSelect;
        }

        private void OnDestroy()
        {
            m_forwardInput.started -= OnForward;
            m_backwardInput.started -= OnBackward;
            m_selectInput.started -= OnSelect;
        }

        private void Start()
        {
            m_cameraManager.MoveCamera(m_crateManager.GetCurrentPosition());
        }

        private void OnForward(InputAction.CallbackContext ctx)
        {
            if (!CanMove()) return;
            m_cameraManager.MoveCamera(m_crateManager.GetNextPosition());
        }

        private void OnBackward(InputAction.CallbackContext ctx)
        {
            if (!CanMove()) return;
            m_cameraManager.MoveCamera(m_crateManager.GetBackPosition());
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!CanMove()) return;
            Debug.Log(m_crateManager.GetCurrentAlbum().Album.AlbumName);
        }

        private bool CanMove()
        {
            return !m_cameraManager.IsMoving;
        }
    }
}