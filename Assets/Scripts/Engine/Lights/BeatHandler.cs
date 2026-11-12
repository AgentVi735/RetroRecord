using System;
using RetroRecord.Engine.Player;
using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    public class BeatHandler
    {
        private int m_lastCount;
        private double m_currentProgress { get; set; }
        private int m_currentCount => Mathf.FloorToInt((float)m_currentProgress);

        private readonly double m_divisor;

        public event Action<LightColor> Action;

        private readonly LightColor m_color;

        public BeatHandler(LightColor color, Action<LightColor> action)
        {
            Action += action;
            LightTime timeType = color.Time;
            m_color = color;
            m_divisor = timeType.GetDivisor();
        }
        
        public void Update()
        {
            uint quarterTick = SyncTrack.TimeToTick(SongPlayer.ConvertedCurrentInputTime);
            double progress = SyncTrack.GetQuarterNotePosition(quarterTick);
            
            m_currentProgress = progress / m_divisor;
            if (m_currentCount == m_lastCount) return;
            Action?.Invoke(m_color);
            m_lastCount = m_currentCount;
        }
    }
}