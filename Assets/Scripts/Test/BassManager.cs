using ManagedBass;
using UnityEngine;

namespace DefaultNamespace
{
    public class BassManager : MonoBehaviour
    {
        public static BassManager s_Instance { get; private set; }

        public bool m_IsInitialised { get; private set; }

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            m_IsInitialised = Bass.Init();

            if (!m_IsInitialised)
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

            if (m_IsInitialised)
            {
                Bass.Free();
                m_IsInitialised = false;
            }

            s_Instance = null;
        }
    }
}