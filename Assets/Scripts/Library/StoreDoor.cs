using System.Collections;
using UnityEngine;

namespace RetroRecord.Library
{
    public class StoreDoor : MonoBehaviour
    {
        [SerializeField]
        private float m_openTime;

        [SerializeField]
        private Vector2 m_animationRotation;

        public IEnumerator OpenDoor()
        {
            Quaternion startRot = Quaternion.Euler(new Vector3(0, m_animationRotation.x, 0));
            Quaternion endRot = Quaternion.Euler(new Vector3(0, m_animationRotation.y, 0));
            for (float i = 0; i < m_openTime; i += Time.deltaTime)
            {
                float ratio = i / m_openTime;
                transform.rotation = Quaternion.Lerp(startRot, endRot, ratio);
                yield return null;
            }

            transform.rotation = endRot;
        }

        public void CloseDoor()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, m_animationRotation.x, 0));
        }
    }
}