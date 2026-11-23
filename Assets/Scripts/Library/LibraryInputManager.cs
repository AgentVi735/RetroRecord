using RetroRecord.Library.Crates;
using RetroRecord.Library.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Library
{
    public class LibraryInputManager : MonoBehaviour
    {
        [SerializeField]
        private CameraManager m_cameraManager;

        [SerializeField]
        private UILpManager m_uiLpManager;
        
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

        [SerializeField]
        private string m_deselectInputPath;
        private InputAction m_deselectInput;
        
        public bool HasAlbumSelected { get; private set; }

        private bool m_isEnabled;

        public void Setup()
        {
            m_forwardInput = m_inputActionAsset.FindAction(m_forwardInputPath);
            m_backwardInput = m_inputActionAsset.FindAction(m_backwardInputPath);
            m_selectInput = m_inputActionAsset.FindAction(m_selectInputPath);
            m_deselectInput = m_inputActionAsset.FindAction(m_deselectInputPath);
            
            m_forwardInput.started += OnForward;
            m_backwardInput.started += OnBackward;
            m_selectInput.started += OnSelect;
            m_deselectInput.started += OnDeselect;
        }

        public void Toggle(bool toggle)
        {
            m_isEnabled = toggle;
        }

        private void OnDestroy()
        {
            m_forwardInput.started -= OnForward;
            m_backwardInput.started -= OnBackward;
            m_selectInput.started -= OnSelect;
            m_deselectInput.started -= OnDeselect;
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
            HasAlbumSelected = true;
            AlbumObject obj = m_crateManager.GetCurrentAlbum();
            bool isAllowed = m_uiLpManager.SelectAlbum(obj.Album);
            
            if (isAllowed) return;
            
            HasAlbumSelected = false;
        }

        private void OnDeselect(InputAction.CallbackContext ctx)
        {
            if (!HasAlbumSelected) return;
            HasAlbumSelected = false;
            m_uiLpManager.DeselectAlbum();
        }

        private bool CanMove() => m_isEnabled && !m_cameraManager.IsMoving && !HasAlbumSelected;
    }
}