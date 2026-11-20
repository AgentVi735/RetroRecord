using System.Collections;
using UnityEngine;

namespace RetroRecord.Library.Crates
{
    public class CrateObject : MonoBehaviour
    {
        public virtual CrateObjectType Type => CrateObjectType.None;
        
        [SerializeField]
        protected AlbumObjectSettings m_settings;

        private float m_flipIndex;
        private Coroutine m_flipCoroutine;
        private bool m_isFlipped;

        public void Flip(bool isFlipped)
        {
            if (isFlipped == m_isFlipped)
                return;
            m_isFlipped = isFlipped;
            
            if (m_flipCoroutine != null)
                StopCoroutine(m_flipCoroutine);
            
            m_flipCoroutine = StartCoroutine(FlipCoroutine(isFlipped));
        }

        private IEnumerator FlipCoroutine(bool isFlipped)
        {
            Quaternion startRot = transform.localRotation;
            Quaternion endRot = Quaternion.Euler(isFlipped ? m_settings.FlippedRotation : m_settings.DefaultRotation);
            
            if (m_flipIndex > 0)
                m_flipIndex = m_settings.TimeToFlip - m_flipIndex;

            for (; m_flipIndex < m_settings.TimeToFlip; m_flipIndex += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(startRot, endRot, m_flipIndex / m_settings.TimeToFlip);
                yield return null;
            }

            transform.localRotation = endRot;

            m_flipIndex = 0;
        }
    }
}