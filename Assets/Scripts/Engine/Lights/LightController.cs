using System.Collections;
using RetroRecord.General;
using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    public class LightController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_renderer;

        [SerializeField]
        private int m_colorOffset;

        private ushort m_count;

        [field: SerializeField]
        public int LaneIndex { get; private set; }

        private Coroutine m_lerpCoroutine;

        private bool m_isLerping;
        
        public bool IsEnabled { get; private set; }

        public void Setup()
        {
            if (LaneIndex == -1)
                IsEnabled = true;
            m_renderer.color = ColorEx.clear2;
        }

        public void Setup(float xPos, bool shouldBeEnabled)
        {
            IsEnabled = shouldBeEnabled;
            gameObject.SetActive(IsEnabled);
            transform.position = new Vector3(xPos, transform.position.y, 0);
            Setup();
        }

        public void ForceChangeColor(LightColor data)
        {
            if (m_lerpCoroutine != null)
                StopCoroutine(m_lerpCoroutine);

            m_isLerping = false;
            OnQuarterBeat(data);
        }

        public void OnQuarterBeat(LightColor data)
        {
            if (m_isLerping)
                return;
            
            if (data == null || data.Colors == null || data.Colors.Length == 0)
            {
                m_renderer.color = Color.clear;
                return;
            }

            switch (data.Type)
            {
                case LightType.Static:
                    m_renderer.color = data.Colors[m_colorOffset];
                    m_count = 0;
                    break;
                case LightType.Switch:
                    int idx = m_count + m_colorOffset;
                    while (idx >= data.Colors.Length)
                        idx -= data.Colors.Length;

                    m_renderer.color = data.Colors[idx];

                    m_count++;

                    if (m_count > 1)
                        m_count = 0;
                    break;
                case LightType.Flash:
                case LightType.Lerp:
                default:
                    break;
            }
        }

        public void EnableLerp(double time, LightColor color, bool shouldBeOn)
        {
            if (m_lerpCoroutine != null)
                StopCoroutine(m_lerpCoroutine);

            m_isLerping = true;
            m_lerpCoroutine = StartCoroutine(Lerp((float)time, color, shouldBeOn));
        }

        private IEnumerator Lerp(float time, LightColor color, bool shouldBeOn)
        {
            if (shouldBeOn)
            {
                for (float i = 0; i < time; i += Time.deltaTime)
                {
                    m_renderer.color = Color.Lerp(color.Colors[0], color.Colors[1], i / time);
                    yield return null;
                }
            }
            else
            {
                m_renderer.color = color.Colors[1];
                yield return new WaitForSeconds(time);
            }
        }
    }
}