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
        private float m_defaultXScale = 4.5f;

        [SerializeField]
        private float m_eightLanesXPos = -3.5f;

        [SerializeField]
        private float m_sixLanesXPos = -2.75f;

        [SerializeField]
        private float m_threeLanesXPos;

        public void Initialize()
        {
            float scaleX;
            float posXL;
            float posXR;

            switch (EngineSettings.s_CurrentDifficultySettings.LaneAmount)
            {
                default:
                    scaleX = m_eightLaneXScale;
                    posXL = m_eightLanesXPos;
                    posXR = -m_eightLanesXPos;
                    break;
                case 6:
                    scaleX = m_defaultXScale;
                    posXL = m_sixLanesXPos;
                    posXR = -m_sixLanesXPos;
                    break;
                case 3:
                    scaleX = m_defaultXScale;
                    posXL = m_threeLanesXPos;
                    posXR = -m_threeLanesXPos;
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