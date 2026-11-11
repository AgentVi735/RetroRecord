using System.Collections;
using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine.Effects
{
    public class SmokeEffectDisplay : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] m_smokeSprites;

        private Coroutine m_showCoroutine;
        private double m_currentFadeTime;

        private bool m_isOn;

        private readonly Color m_clearColor = new(1, 1, 1, 0);

        [SerializeField]
        private Color m_finalColor;

        public void Enable(EffectItem item)
        {
            gameObject.SetActive(true);

            if (m_showCoroutine != null)
                StopCoroutine(m_showCoroutine);
            m_showCoroutine = StartCoroutine(LerpFadeCoroutine(item.TimeDuration));
        }

        private IEnumerator LerpFadeCoroutine(double duration)
        {
            m_isOn = !m_isOn;

            Color startColor = m_smokeSprites[0].color;
            Color endColor = m_isOn ? m_finalColor : m_clearColor;

            if (m_currentFadeTime > 0)
                m_currentFadeTime = duration - m_currentFadeTime;

            for (; m_currentFadeTime < duration; m_currentFadeTime += Time.deltaTime)
            {
                Color newColor = Color.Lerp(startColor, endColor, (float)(m_currentFadeTime / duration));
                foreach (SpriteRenderer sprite in m_smokeSprites)
                    sprite.color = newColor;
                yield return null;
            }

            foreach (SpriteRenderer sprite in m_smokeSprites)
                sprite.color = endColor;

            m_currentFadeTime = 0;
        }
    }
}