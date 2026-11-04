using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Engine.Player.Renderer
{
    public class HighwayLane : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        [SerializeField]
        private HighwayStrikeline m_strikeline;

        [SerializeField]
        private Vector3 m_defaultLanePos;

        [SerializeField]
        private Vector3 m_middleLanePos;

        private int m_newColorIdx = -1;
        private Color m_prevColor;
        private Color m_newColor;
        private Coroutine m_changeColorCoroutine;

        public int Idx;
        private bool m_isEnabled;

        private InputAction m_holdInputAction;
        private InputAction m_holdInputActionAlt;
        private Action<InputAction.CallbackContext> m_holdAction;
        private Action<InputAction.CallbackContext> m_releaseAction;

        private LaneSettings m_settings;

        public void Initialize(bool shouldTurnOn, LaneSettings settings)
        {
            m_isEnabled = shouldTurnOn;
            if (!shouldTurnOn)
                gameObject.SetActive(false);
            
            m_settings = settings;
            m_strikeline.Initialize(m_settings);
            
            m_holdAction = _ => { m_strikeline.Hold(true); };
            m_releaseAction = _ => { m_strikeline.Hold(false); };

            if (Idx % 2 == 0)
            {
                m_spriteRenderer.color = m_settings.LaneSprite.DefaultColor;
                m_strikeline.ChangeColor(false);
                m_newColorIdx = Idx < 4 ? 0 : 2;
            }
            else
            {
                m_spriteRenderer.color = m_settings.LaneSprite.SecondaryColor;
                m_strikeline.ChangeColor(true);
                m_newColorIdx = Idx < 4 ? 1 : 3;
            }

            m_newColor = m_spriteRenderer.color;

            m_strikeline.UpdateColorAmt(1);

            transform.localPosition = EngineSettings.s_CurrentDifficultySettings.AreLanesInMiddle
                ? m_middleLanePos
                : m_defaultLanePos;
        }

        public void ChangeColor(LaneSettings newSettings)
        {
            if (!m_isEnabled)
                return;
            if (newSettings.name == m_settings.name)
                return;
            m_settings = newSettings;
            Color tempNewColor;
            int tempNewColorIdx;
            bool isAlt;
            if (Idx % 2 == 0)
            {
                tempNewColor = m_settings.LaneSprite.DefaultColor;
                isAlt = false;
                tempNewColorIdx = 0;
            }
            else
            {
                tempNewColor = m_settings.LaneSprite.SecondaryColor;
                isAlt = true;
                tempNewColorIdx = 1;
            }

            if (m_changeColorCoroutine != null)
                StopCoroutine(m_changeColorCoroutine);

            m_prevColor = m_newColor;
            m_strikeline.ChangeColor(isAlt);

            m_newColorIdx = tempNewColorIdx;
            m_newColor = tempNewColor;

            m_changeColorCoroutine = StartCoroutine(ChangeColorLoop());
        }

        private IEnumerator ChangeColorLoop()
        {
            float startTime = (float)SongPlayer.s_Instance.CurrentAudioTime;
            float time = startTime + 60 * EngineSettings.s_LaneColorUpdateBeatCount / NotesManager.s_CurrentTempo;
            float amt = 0;
            while (amt < 1)
            {
                amt = Mathf.InverseLerp(startTime, time, (float)SongPlayer.s_Instance.CurrentAudioTime);
                m_spriteRenderer.color = Color.Lerp(m_prevColor, m_newColor, amt);
                m_strikeline.UpdateColorAmt(amt);
                yield return null;
            }
        }

        public void SetActionEvents(InputAction action)
        {
            if (!m_isEnabled)
                return;
            if (m_holdInputAction == null)
                m_holdInputAction = action;
            else if (m_holdInputActionAlt == null)
                m_holdInputActionAlt = action;
            else
                return;
            action.started += m_holdAction;
            action.canceled += m_releaseAction;
        }

        private void OnDestroy()
        {
            if (m_holdInputAction != null)
            {
                m_holdInputAction.started -= m_holdAction;
                m_holdInputAction.canceled -= m_releaseAction;
            }

            if (m_holdInputActionAlt == null) return;
            m_holdInputActionAlt.started -= m_holdAction;
            m_holdInputActionAlt.canceled -= m_releaseAction;
        }
    }
}