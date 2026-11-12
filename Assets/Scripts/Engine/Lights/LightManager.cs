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

        private BeatHandler m_sideHandler;
        private BeatHandler m_lanesHandler;

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

        // TODO:
        // moet op een manier meerdere versies hebben
        // ig via een nieuwe class zoals yarg doet?
        // ik moet dan een class hebben die elke update runt dus die moet in een list opgeslagen worden. misschien is een array beter zodat ik er altijd mr 2 heb
        // 1 class voor side en 1 voor lane
        // kan ook anders gwn 2 vars worden
        
        public void ManualUpdate()
        {
            m_sideHandler?.Update();
            m_lanesHandler?.Update();
            
            // uint quarterTick = SyncTrack.TimeToTick(SongPlayer.ConvertedCurrentInputTime);
            // double progress = SyncTrack.GetQuarterNotePosition(quarterTick);
            //
            // m_currentProgress = progress;
            // if (m_currentCount == m_lastCount) return;
            // m_onQuarterBeatSide?.Invoke(m_currentSideColor);
            // m_onQuarterBeatLane?.Invoke(m_currentLaneColor);
            // m_lastCount = m_currentCount;
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
                            CreateNewSideHandler(data);
                            CreateNewLaneHandler(data);
                            break;
                        case LightSide.Side:
                            CreateNewSideHandler(data);
                            break;
                        case LightSide.Lanes:
                            CreateNewLaneHandler(data);
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

        private void CreateNewSideHandler(LightColor data)
        {
            m_currentSideColor = data;
            m_sideHandler = new BeatHandler(m_currentSideColor, m_onQuarterBeatSide);
        }

        private void CreateNewLaneHandler(LightColor data)
        {
            m_currentLaneColor = data;
            m_lanesHandler = new BeatHandler(m_currentLaneColor, m_onQuarterBeatLane);
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