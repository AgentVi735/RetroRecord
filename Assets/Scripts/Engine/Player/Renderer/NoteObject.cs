using System;
using System.Collections;
using RetroRecord.Engine.Chart;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Engine.Player.Renderer
{
    public class NoteObject : MonoBehaviour
    {
        public Note Note { get; private set; }

        [SerializeField]
        private LineRenderer m_lineRenderer;

        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        [SerializeField]
        private Color m_noteColor = Color.white;

        [SerializeField]
        private Color m_missedNoteColor = Color.white;

        [SerializeField]
        private Color m_lineColor = Color.white;

        [SerializeField]
        private Color m_missedLineColor = Color.white;

        private InputAction m_sustainAction;
        private bool m_isHoldingSustain;
        private float m_lanePos;
        private double m_sustainEndTime;

        public double DespawnTime { get; private set; }

        public void Load(Note givenNote, Transform lane)
        {
            m_isHoldingSustain = false;
            Note = givenNote;
            Vector2 pos = new(lane.position.x, SyncTrack.TimeToYPos(Note.Time));
            m_lanePos = pos.x;
            transform.position = pos;
            m_sustainEndTime = Note.Time + Note.TimeSustainLength;
            DespawnTime = m_sustainEndTime + EngineSettings.s_MinSecUntilNoteDespawn;
            m_spriteRenderer.color = m_noteColor;
            m_spriteRenderer.enabled = true;

            if (Note.TickSustainLength > 0)
            {
                float sustainHeight = SyncTrack.TimeToYPos(m_sustainEndTime);
                Vector3[] positions = new Vector3[2];
                positions[0] = new Vector3(pos.x, pos.y, 0);
                positions[1] = new Vector3(pos.x, sustainHeight, 0);
                m_lineRenderer.SetPositions(positions);
                m_lineRenderer.startColor = m_lineColor;
                m_lineRenderer.endColor = m_lineColor;

                m_lineRenderer.enabled = true;
            }
            else
                m_lineRenderer.enabled = false;

            gameObject.SetActive(true);
        }

        public void Unload()
        {
            m_isHoldingSustain = false;
            gameObject.SetActive(false);
            m_lineRenderer.enabled = false;
            if (m_sustainAction == null) return;
            m_sustainAction.canceled -= OnCancelledInput;
            m_sustainAction = null;
        }

        public void HitSustain(InputAction action)
        {
            m_spriteRenderer.enabled = false;
            m_isHoldingSustain = true;
            StartCoroutine(SustainLoop());
            m_sustainAction = action;
            m_sustainAction.canceled += OnCancelledInput;
        }

        private IEnumerator SustainLoop()
        {
            while (m_isHoldingSustain)
            {
                if (m_sustainEndTime > SongPlayer.CurrentAudioTime)
                {
                    Vector3 pos = new(m_lanePos, SyncTrack.TimeToYPos(SongPlayer.CurrentAudioTime));
                    m_lineRenderer.SetPosition(0, pos);
                    transform.position = pos;
                }
                else
                {
                    m_isHoldingSustain = false;
                    Unload();
                    yield break;
                }

                yield return null;
            }
        }

        private void OnCancelledInput(InputAction.CallbackContext ctx)
        {
            m_isHoldingSustain = false;
            if (Math.Abs(Note.Time + Note.TimeSustainLength - SongPlayer.CurrentAudioTime) >
                EngineSettings.s_MinSecShowStopSustain)
            {
                m_spriteRenderer.color = m_missedNoteColor;
                m_spriteRenderer.enabled = true;
                m_lineRenderer.startColor = m_missedLineColor;
                m_lineRenderer.endColor = m_missedLineColor;
            }
            else
                Unload();

            if (m_sustainAction == null) return;
            m_sustainAction.canceled -= OnCancelledInput;
            m_sustainAction = null;
        }

        public void Miss()
        {
            m_spriteRenderer.color = m_missedNoteColor;
            m_spriteRenderer.enabled = true;
            m_lineRenderer.startColor = m_missedLineColor;
            m_lineRenderer.endColor = m_missedLineColor;
        }

        private void OnDestroy()
        {
            if (m_sustainAction != null)
                m_sustainAction.canceled -= OnCancelledInput;
        }
    }
}