using System;
using System.Collections;
using UnityEngine;

namespace RetroRecord.Library
{
    // TODO:
    // make the camera move through every crate instead of only the next one with an album
    // make it auto continue if there's no album
    
    public class CameraManager : MonoBehaviour
    {
        [field: SerializeField]
        public Camera Camera { get; private set; }

        [SerializeField]
        private Transform m_camTarget;

        [field: SerializeField]
        public Vector3 CamPosOffset { get; private set; }

        [field: SerializeField]
        public Vector3 CamRotOffset { get; private set; }

        [SerializeField]
        private Turntable m_turntable;

        [SerializeField]
        private float m_moveTime;

        [field: SerializeField]
        public bool IsMoving { get; private set; }

        public void SetPosition(CameraPosition data)
        {
            Camera.transform.position = data.position;
            Camera.transform.rotation = data.rotation;
        }
        
        public void MoveCamera(CameraPosition data)
        {
            StartCoroutine(MoveCamToTarget(data, true));
        }
        
        public IEnumerator MoveCamera_Coroutine(CameraPosition data)
        {
            yield return StartCoroutine(MoveCamToTarget(data, true));
        }

        private IEnumerator MoveCamToTarget(CameraPosition data, bool useOffsets)
        {
            IsMoving = true;
            
            m_camTarget.transform.position = data.position;
            m_camTarget.transform.rotation = data.rotation;
            if (useOffsets)
            {
                m_camTarget.transform.Translate(CamPosOffset);
                m_camTarget.transform.Rotate(CamRotOffset);
            }

            Vector3 startPos = Camera.transform.position;
            Vector3 endPos = m_camTarget.position;
            Quaternion startRot = Camera.transform.rotation;
            Quaternion endRot = m_camTarget.rotation;
            for (float i = 0; i < m_moveTime; i += Time.deltaTime)
            {
                float ratio = i / m_moveTime;
                Camera.transform.position = Vector3.Lerp(startPos, endPos, ratio);
                Camera.transform.rotation = Quaternion.Lerp(startRot, endRot, ratio);
                yield return null;
            }

            Camera.transform.position = endPos;
            Camera.transform.rotation = endRot;
            IsMoving = false;
        }

        public IEnumerator MoveToTurntable()
        {
            yield return StartCoroutine(MoveCamToTarget(m_turntable.CamPos, false));
        }
    }
}