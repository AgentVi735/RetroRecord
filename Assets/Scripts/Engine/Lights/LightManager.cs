using System;
using System.Linq;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Chart.Events;
using RetroRecord.Engine.Player;
using RetroRecord.Engine.Settings;
using UnityEngine;

namespace RetroRecord.Engine.Lights
{
    // TODO:
    // LightColor moet een Time var hebben die bepaalt hoeveel in de maat de kleur moet veranderen
    // Dus bijvoorbeeld 1/4ste, 1/8ste, 1/16de, 1/2de, 1/1ste, etc
    // Er komt een class/method die kijkt of er eentje af moet gaan, zo ja kijkt het welke en hoe ver die is, etc (moet in update)
    // Type QuarterBeat moet verandert worden naar switch
    
    public class LightManager : MonoBehaviour
    {
        [SerializeField]
        private InputManager m_inputManager;
        
        [SerializeField]
        private LightController[] m_lightControllers;

        [SerializeField]
        private LightController[] m_laneLights;

        [SerializeField]
        private LightController[] m_sideLights;

        [SerializeField]
        private LightColor m_flickeringColor;

        private LightColor[] m_lightData;

        private LightColor m_currentSideColor;

        private LightColor m_currentLaneColor;

        private event Action<LightColor> m_onQuarterBeatSide;
        private event Action<LightColor> m_onQuarterBeatLane;

        private int m_lastCount;
        private bool m_hasFlickers;

        private double m_currentProgress { get; set; }
        private int m_currentCount => Mathf.FloorToInt((float)m_currentProgress);

        public void Initialize()
        {
            m_lightData = EngineSettings.Song.Lights;
            
            foreach (LightController light in m_sideLights)
            {
                light.Setup();
                m_onQuarterBeatSide += light.OnQuarterBeat;
            }
            
            foreach (LightController light in m_laneLights)
            {
                light.Setup();
                m_onQuarterBeatLane += light.OnQuarterBeat;
            }
        }

        private void OnDestroy()
        {
            m_onQuarterBeatSide = null;
        }

        public void ManualUpdate()
        {
            uint quarterTick = SyncTrack.TimeToTick(SongPlayer.ConvertedCurrentInputTime);
            double progress = SyncTrack.GetQuarterNotePosition(quarterTick);
            
            m_currentProgress = progress;
            if (m_currentCount == m_lastCount) return;
            m_onQuarterBeatSide?.Invoke(m_currentSideColor);
            m_onQuarterBeatLane?.Invoke(m_currentLaneColor);
            m_lastCount = m_currentCount;
        }

        public void OnLightEvent(LightsEvent lightsEvent)
        {
            uint index = lightsEvent.Index;
            if (m_lightData.Length <= index)
            {
                Debug.LogError("Color index is higher than the amount of Light Data");
                return;
            }

            switch (lightsEvent.Type)
            {
                case LightType.Switch:
                {
                    LightColor data = m_lightData[index];
                    switch (data.Side)
                    {
                        default:
                        case LightSide.All:
                            m_currentSideColor = data;
                            m_currentLaneColor = data;
                            break;
                        case LightSide.Side:
                            m_currentSideColor = data;
                            break;
                        case LightSide.Lanes:
                            m_currentLaneColor = data;
                            break;
                    }

                    if (m_currentLaneColor?.Type == LightType.Switch)
                    {
                        foreach (LightController light in m_laneLights)
                            light.ForceChangeColor(m_currentLaneColor);
                    }
                    if (m_currentSideColor?.Type == LightType.Switch)
                    {
                        foreach (LightController light in m_sideLights)
                            light.ForceChangeColor(m_currentSideColor);
                    }
                    break;
                }
                case LightType.Flickering:
                    OnNoteFlicker(lightsEvent);
                    break;
            }
        }

        private void OnNoteFlicker(LightsEvent lightsEvent)
        {
            if (lightsEvent.NoteGroup != null)
            {
                byte[] lanes = new byte[lightsEvent.NoteGroup.Notes.Length];
                for (int i = 0; i < lightsEvent.NoteGroup.Notes.Length; i++)
                {
                    Note note = lightsEvent.NoteGroup.Notes[i];
                    lanes[i] = note.Lane;
                }

                foreach (LightController light in m_laneLights)
                {
                    bool shouldBeOn = lanes.Contains((byte)light.LaneIndex);
                    light.EnableFlicker(lightsEvent.TimeUntilNextFlicker, m_flickeringColor, shouldBeOn);
                }
            }

            foreach (LightController light in m_sideLights)
                light.EnableFlicker(lightsEvent.TimeUntilNextFlicker, m_flickeringColor, true);
        }
    }
}