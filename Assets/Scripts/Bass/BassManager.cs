using ManagedBass;
using UnityEngine;

namespace RetroRecord.Audio
{
    public class BassManager : MonoBehaviour
    {
        public static BassManager s_Instance { get; private set; }

        public bool IsInitialised { get; private set; }

        public void Initialize()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            IsInitialised = Bass.Init();

            if (!IsInitialised)
            {
                Debug.LogError(
                    "BASS initialization failed: " + Bass.LastError
                );
            }
            else
            {
                Debug.Log("BASS initialized successfully");
            }
        }

        private void OnDestroy()
        {
            if (s_Instance != this)
                return;

            if (IsInitialised)
            {
                Bass.Free();
                IsInitialised = false;
            }

            s_Instance = null;
        }
    }
}