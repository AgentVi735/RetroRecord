using RetroRecord.Engine.Chart;
using UnityEngine;

namespace RetroRecord.Engine.Player.Renderer
{
    public class BeatlineObject : MonoBehaviour
    {
        private Beatline m_beatline;

        public double Time;

        [SerializeField]
        private SpriteRenderer m_leftRenderer;

        [SerializeField]
        private SpriteRenderer m_rightRenderer;

        [SerializeField]
        private Transform m_leftTrans;

        [SerializeField]
        private Transform m_rightTrans;

        [SerializeField]
        private float m_measureYScale = 0.2f;

        [SerializeField]
        private float m_strongYScale = 0.1f;

        [SerializeField]
        private float m_weakYScale = 0.05f;

        [SerializeField]
        private float m_eightLaneXScale = 6;

        [SerializeField]
        private float m_normalXScale = 4.5f;
        
        [SerializeField]
        private float m_easyXScale = 6;

        [SerializeField]
        private float m_hardLanesXPos = -3.5f;

        [SerializeField]
        private float m_normalLanesXPos = -2.75f;

        [SerializeField]
        private float m_easyLanesXPos;

        public void Initialize()
        {
            float scaleX;
            float posXL;
            float posXR;

            switch (EngineSettings.s_Difficulty)
            {
                default:
                case Difficulty.Hard:
                    scaleX = m_eightLaneXScale;
                    posXL = m_hardLanesXPos;
                    posXR = -m_hardLanesXPos;
                    break;
                case Difficulty.Normal:
                    scaleX = m_normalXScale;
                    posXL = m_normalLanesXPos;
                    posXR = -m_normalLanesXPos;
                    break;
                case Difficulty.Easy:
                    scaleX = m_easyXScale;
                    posXL = m_easyLanesXPos;
                    posXR = -m_easyLanesXPos;
                    m_rightRenderer.gameObject.SetActive(false);
                    break;
            }

            Vector3 scale = new(scaleX, 1, 1);
            Vector3 posL = new(posXL, 0, 0);
            Vector3 posR = new(posXR, 0, 0);

            m_leftTrans.localScale = scale;
            m_leftTrans.localPosition = posL;
            m_rightTrans.localScale = scale;
            m_rightTrans.localPosition = posR;

            gameObject.SetActive(false);
        }

        public void Load(Beatline givenBeatline)
        {
            m_beatline = givenBeatline;
            Time = m_beatline.Time;
            transform.position = new Vector3(0, SyncTrack.TimeToYPos(m_beatline.Time));
            transform.localScale = m_beatline.Type switch
            {
                BeatlineType.Measure => new Vector3(1, m_measureYScale, 1),
                BeatlineType.Strong => new Vector3(1, m_strongYScale, 1),
                BeatlineType.Weak => new Vector3(1, m_weakYScale, 1),
                _ => transform.localScale
            };

            gameObject.SetActive(true);
        }

        public void Unload()
        {
            gameObject.SetActive(false);
        }
    }
}