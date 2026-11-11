using System;
using RetroRecord.Engine.Player;
using RetroRecord.Engine.Settings;
using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    public class LightManager : MonoBehaviour
    {
        [SerializeField]
        private LightController[] m_lightControllers;

        [SerializeField]
        private LightController[] m_laneLights;

        [SerializeField]
        private LightController[] m_sideLights;

        private LightColor[] m_lightData;

        private LightColor m_currentColor;

        private event Action<LightColor> m_quarterBeatActions;

        private int m_lastCount;

        private double m_currentProgress { get; set; }
        private int m_currentCount => Mathf.FloorToInt((float)m_currentProgress);

        public void Initialize()
        {
            m_lightData = EngineSettings.Song.Lights;
            
            foreach (LightController light in m_sideLights)
            {
                light.Setup();
                m_quarterBeatActions += light.ChangeColor;
            }
        }

        public void ManualUpdate()
        {
            uint quarterTick = SyncTrack.TimeToTick(SongPlayer.ConvertedCurrentInputTime);
            double progress = SyncTrack.GetQuarterNotePosition(quarterTick);
            
            m_currentProgress = progress;
            if (m_currentCount == m_lastCount) return;
            m_quarterBeatActions?.Invoke(m_currentColor);
            m_lastCount = m_currentCount;
        }

        public void ChangeCurrentColor(uint index)
        {
            if (m_lightData.Length <= index)
            {
                Debug.LogError("Color index is higher than the amount of Light Data");
                return;
            }

            m_currentColor = m_lightData[index];
        }
    }
}