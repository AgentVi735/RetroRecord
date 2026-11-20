using System.Collections;
using UnityEngine;

namespace RetroRecord.Library
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private Camera m_camera;

        [SerializeField]
        private Transform m_camTarget;

        [field: SerializeField]
        public Vector3 CamPosOffset { get; private set; }

        [field: SerializeField]
        public Vector3 CamRotOffset { get; private set; }

        [SerializeField]
        private float m_moveTime;

        [field: SerializeField]
        public bool IsMoving { get; private set; }

        public void MoveCamera(CameraPosition newPosition)
        {
            StartCoroutine(MoveCam(newPosition));
        }

        private IEnumerator MoveCam(CameraPosition data)
        {
            IsMoving = true;
            m_camTarget.transform.position = data.position; // TODO: smooth
            m_camTarget.transform.rotation = data.rotation;
            m_camTarget.transform.Translate(CamPosOffset);
            m_camTarget.transform.Rotate(CamRotOffset);

            Vector3 startPos = m_camera.transform.position;
            Vector3 endPos = m_camTarget.position;
            Quaternion startRot = m_camera.transform.rotation;
            Quaternion endRot = m_camTarget.rotation;
            for (float i = 0; i < m_moveTime; i += Time.deltaTime)
            {
                float ratio = i / m_moveTime;
                m_camera.transform.position = Vector3.Lerp(startPos, endPos, ratio);
                m_camera.transform.rotation = Quaternion.Lerp(startRot, endRot, ratio);
                yield return null;
            }

            m_camera.transform.position = endPos;
            m_camera.transform.rotation = endRot;
            IsMoving = false;
        }
    }
}