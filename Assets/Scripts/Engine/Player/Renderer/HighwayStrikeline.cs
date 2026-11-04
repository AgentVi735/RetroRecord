using UnityEngine;

namespace RetroRecord.Engine.Player
{
    public class HighwayStrikeline : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        private LaneSettings m_settings;

        private Color m_prevColor;
        private Color m_prevColorHit;
        private Color m_newColor;
        private Color m_newColorHit;

        private int m_prevColorIdx = -1;
        private int m_colorIdx = -1;
        private bool m_isHolding;

        public void Initialize(LaneSettings settings)
        {
            m_settings = settings;
        }
        
        public void ChangeColor(bool isSecondColor)
        {
            m_prevColorIdx = m_colorIdx;

            m_colorIdx = isSecondColor ? 1 : 0;

            m_prevColor = m_newColor;
            m_prevColorHit = m_newColorHit;

            UpdateColorIdx();
        }

        private void UpdateColorIdx()
        {
            if (m_colorIdx == m_prevColorIdx)
                return;

            Color color;
            Color colorHit;
            switch (m_colorIdx)
            {
                default:
                    color = m_settings.StrikelineSprite.DefaultColor;
                    colorHit = m_settings.StrikelineHitSprite.DefaultColor;
                    break;
                case 1:
                    color = m_settings.StrikelineSprite.SecondaryColor;
                    colorHit = m_settings.StrikelineHitSprite.SecondaryColor;
                    break;
            }

            m_newColor = color;
            m_newColorHit = colorHit;
        }

        public void UpdateColorAmt(float amt)
        {
            if (m_colorIdx == m_prevColorIdx)
                return;
            Color color = m_isHolding
                ? Color.Lerp(m_prevColorHit, m_newColorHit, amt)
                : Color.Lerp(m_prevColor, m_newColor, amt);
            m_spriteRenderer.color = color;
        }

        public void Hold(bool toggle)
        {
            m_isHolding = toggle;
            m_spriteRenderer.color = m_isHolding ? m_newColorHit : m_newColor;
        }
    }
}