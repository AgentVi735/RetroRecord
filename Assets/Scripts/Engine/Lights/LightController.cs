using RetroRecord.General;
using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    public class LightController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_renderer;

        private float m_defaultOpacity;
        private int m_count;
        
        public void Setup()
        {
            m_defaultOpacity = m_renderer.color.a;
            m_renderer.color = ColorEx.clear;
        }
        
        public void Setup(float xPos)
        {
            transform.position = new Vector3(xPos, transform.position.y, 0);
            Setup();
        }

        public void ChangeColor()
        {
            m_renderer.color = m_count switch
            {
                0 => Color.blue,
                1 => Color.red,
                _ => m_renderer.color
            };

            m_count++;

            if (m_count > 1)
                m_count = 0;
        }
    }
}