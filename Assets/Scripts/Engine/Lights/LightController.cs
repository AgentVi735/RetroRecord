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
        
        public void Setup()
        {
            m_renderer.color = ColorEx.clear2;
        }
        
        public void Setup(float xPos)
        {
            transform.position = new Vector3(xPos, transform.position.y, 0);
            Setup();
        }

        public void ChangeColor(LightColor data)
        {
            if (data == null)
            {
                m_renderer.color = Color.clear;
                return;
            }

            int idx = m_count + m_colorOffset;
            while (idx >= data.Colors.Length)
                idx -= data.Colors.Length;

            m_renderer.color = data.Colors[idx];

            m_count++;

            if (m_count > 1)
                m_count = 0;
        }
    }
}