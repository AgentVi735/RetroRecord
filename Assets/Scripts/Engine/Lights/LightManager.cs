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

        private event Action m_quarterBeatActions;

        private int m_lastCount;
        
        public double CurrentProgress { get; private set; }
        public double CurrentPercentage => CurrentProgress % 1;
        public int CurrentCount => Mathf.FloorToInt((float)CurrentProgress);

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
            
            CurrentProgress = progress;
            if (CurrentCount == m_lastCount) return;
            m_quarterBeatActions?.Invoke();
            m_lastCount = CurrentCount;
        }
    }
}