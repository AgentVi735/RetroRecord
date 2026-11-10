using System.Collections;
using UnityEngine;

namespace RetroRecord.Engine.Effects
{
    public class SparkEffectDisplay : MonoBehaviour
    {
        private Coroutine m_showCoroutine;
        
        public void Enable(double duration)
        {
            gameObject.SetActive(true);
            if (m_showCoroutine != null)
                StopCoroutine(m_showCoroutine);
            m_showCoroutine = StartCoroutine(Enable_Internal(duration));
        }

        private IEnumerator Enable_Internal(double duration)
        {
            yield return new WaitForSeconds((float)duration);

            gameObject.SetActive(false);
        }
    }
}