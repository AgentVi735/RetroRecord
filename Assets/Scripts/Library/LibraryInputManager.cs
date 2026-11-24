using RetroRecord.Library.Crates;
using RetroRecord.Library.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Library
{
    public class LibraryInputManager : MonoBehaviour
    {
        [SerializeField]
        private RecordStoreManager m_recordStoreManager;
        
        [SerializeField]
        private CameraManager m_cameraManager;

        [SerializeField]
        private UILpManager m_uiLpManager;
        
        [SerializeField]
        private CrateManager m_crateManager;

        [SerializeField]
        private InputActionAsset m_inputActionAsset;

        [SerializeField]
        private InputActionReference m_forwardInput;

        [SerializeField]
        private InputActionReference m_backwardInput;

        [SerializeField]
        private InputActionReference m_selectInput;

        [SerializeField]
        private InputActionReference m_deselectInput;
        
        public bool HasAlbumSelected { get; private set; }

        private bool m_isEnabled;

        public void Setup()
        {
            m_forwardInput.action.started += OnForward;
            m_backwardInput.action.started += OnBackward;
            m_selectInput.action.started += OnSelect;
            m_deselectInput.action.started += OnDeselect;
        }

        public void Toggle(bool toggle)
        {
            m_isEnabled = toggle;
            if (toggle)
                m_forwardInput.action.actionMap.Enable();
            else
                m_forwardInput.action.actionMap.Disable();
        }

        private void OnDestroy()
        {
            m_forwardInput.action.started -= OnForward;
            m_backwardInput.action.started -= OnBackward;
            m_selectInput.action.started -= OnSelect;
            m_deselectInput.action.started -= OnDeselect;
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
            if (!HasAlbumSelected)
                m_recordStoreManager.DisableLibrary();
            else
            {
                HasAlbumSelected = false;
                m_uiLpManager.DeselectAlbum();
            }
        }

        public void UnselectAlbum()
        {
            if (!HasAlbumSelected) return;
            HasAlbumSelected = false;
            m_uiLpManager.DeselectAlbum();
        }

        private bool CanMove() => m_isEnabled && !m_cameraManager.IsMoving && !HasAlbumSelected;
    }
}