using UnityEngine;

namespace RetroRecord.Library
{
    public class Turntable : MonoBehaviour
    {
        public CameraPosition CamPos { get; private set; }

        [SerializeField]
        private Transform m_camPos;

        private void Awake()
        {
            CamPos = new CameraPosition
            {
                position = m_camPos.position,
                rotation = m_camPos.rotation
            };
        }
    }
}