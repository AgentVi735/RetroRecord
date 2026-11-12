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

        private Coroutine m_flickeringCoroutine;

        public void Setup()
        {
            m_renderer.color = ColorEx.clear2;
        }

        public void Setup(float xPos)
        {
            transform.position = new Vector3(xPos, transform.position.y, 0);
            Setup();
        }

        public void OnQuarterBeat(LightColor data)
        {
            if (data == null || data.Colors == null || data.Colors.Length == 0)
            {
                if (m_flickeringCoroutine != null)
                    StopCoroutine(m_flickeringCoroutine);
                m_renderer.color = Color.clear;
                return;
            }

            switch (data.Type)
            {
                case LightType.QuarterBeat:
                {
                    if (m_flickeringCoroutine != null)
                        StopCoroutine(m_flickeringCoroutine);
                    
                    int idx = m_count + m_colorOffset;
                    while (idx >= data.Colors.Length)
                        idx -= data.Colors.Length;

                    m_renderer.color = data.Colors[idx];

                    m_count++;

                    if (m_count > 1)
                        m_count = 0;
                    break;
                }
                case LightType.Flickering:
                default:
                    break;
            }
        }

        public void EnableFlicker(double time, LightColor color, bool shouldBeOn)
        {
            if (m_flickeringCoroutine != null)
                StopCoroutine(m_flickeringCoroutine);
            
            if (shouldBeOn)
                m_flickeringCoroutine = StartCoroutine(Flicker((float)time, color));
            else
                m_renderer.color = color.Colors[1];
        }

        private IEnumerator Flicker(float time, LightColor color)
        {
            for (float i = 0; i < time; i += Time.deltaTime)
            {
                m_renderer.color = Color.Lerp(color.Colors[0], color.Colors[1], i / time);
                yield return null;
            }
        }
    }
}