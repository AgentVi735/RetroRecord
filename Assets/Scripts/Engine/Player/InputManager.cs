using System;
using System.Linq;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Player.Renderer;
using RetroRecord.Engine.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Engine.Player
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private HighwayRenderer m_highwayRenderer;

        private NoteGroup[] m_groups;

        [SerializeField]
        private InputActionAsset m_inputActionAsset;

        [SerializeField]
        private string m_actionMapName;

        [SerializeField]
        private string[] m_laneNames;

        private Action<InputAction.CallbackContext>[] m_actionCallbacks;

        public NoteGroup NextGroup { get; private set; }
        public NoteGroup NextGroup2 { get; private set; }
        private int m_groupIdx;

        public void Initialize(NoteGroup[] givenGroups, int currentIdx)
        {
            m_groups = givenGroups;
            m_groupIdx = currentIdx;
            NextGroup = m_groups[m_groupIdx];
            NextGroup2 = m_groupIdx < m_groups.Length ? m_groups[m_groupIdx + 1] : null;
            InputActionMap actionMap = m_inputActionAsset.FindActionMap(m_actionMapName);
            m_actionCallbacks = new Action<InputAction.CallbackContext>[m_laneNames.Length];

            for (int i = 0; i < m_laneNames.Length; i++)
            {
                if (i < EngineSettings.CurrentDifficultySettings.StartingLane ||
                    i >= EngineSettings.CurrentDifficultySettings.LaneAmount +
                    EngineSettings.CurrentDifficultySettings.StartingLane)
                    continue;
                InputAction action = actionMap.FindAction(m_laneNames[i]);
                int value = i;
                m_actionCallbacks[i] = context => OnLaneInput(context, value);
                action.performed += m_actionCallbacks[i];
                m_highwayRenderer.SetLaneAction(i, action);
            }
        }

        private void OnDestroy()
        {
            InputActionMap actionMap = m_inputActionAsset.FindActionMap(m_actionMapName);
            if (m_actionCallbacks == null)
                return;
            for (int i = 0; i < m_laneNames.Length; i++)
            {
                if (i < EngineSettings.CurrentDifficultySettings.StartingLane ||
                    i >= EngineSettings.CurrentDifficultySettings.LaneAmount +
                    EngineSettings.CurrentDifficultySettings.StartingLane)
                    continue;
                InputAction action = actionMap.FindAction(m_laneNames[i]);
                action.performed -= m_actionCallbacks[i];
            }
        }

        private void OnLaneInput(InputAction.CallbackContext ctx, int idx)
        {
            if (NextGroup == null)
                return;
            double time = ctx.time;

            double diff = Math.Abs(NextGroup.Time - (time - SongPlayer.InputTimeOffset));
            if (diff > EngineSettings.NoteHitboxSec)
                return;

            Note item = null;
            foreach (Note note in NextGroup.Notes)
            {
                if (note.WasHit || note.Lane != idx)
                    continue;
                item = note;
                note.WasHit = true;
                break;
            }

            if (item == null)
            {
                if (NextGroup2 == null)
                    return;

                diff = Math.Abs(NextGroup2.Time - (time - SongPlayer.InputTimeOffset));
                if (diff > EngineSettings.NoteHitboxSec)
                    return;

                Note item2 = NextGroup2.Notes.FirstOrDefault(a => a.Lane == idx && !a.WasHit);
                if (item2 == null)
                    return;

                item2.WasHit = true;
                m_highwayRenderer.HitNote(item2, ctx.action, diff < EngineSettings.PerfectHitboxSec);
                return;
            }


            m_highwayRenderer.HitNote(item, ctx.action, diff < EngineSettings.PerfectHitboxSec);

            int amt = NextGroup.Notes.Count(groupItem => groupItem.WasHit);
            if (amt == NextGroup.Notes.Length)
                MoveToNextGroup(false);
        }

        public void ManualUpdate()
        {
            if (!SongPlayer.s_Instance.HasStarted)
                return;

            if (NextGroup == null)
                return;

            while (NextGroup?.Time < SongPlayer.ConvertedCurrentInputTime - EngineSettings.NoteHitboxSec)
                MoveToNextGroup(true);
        }

        private void MoveToNextGroup(bool hasMissed)
        {
            if (hasMissed)
            {
                foreach (Note note in NextGroup.Notes)
                {
                    if (!note.WasHit)
                        m_highwayRenderer.MissNote(note);
                }
            }

            m_groupIdx++;
            if (m_groupIdx < m_groups.Length)
            {
                NextGroup = m_groups[m_groupIdx];
                NextGroup2 = m_groupIdx + 1 < m_groups.Length ? m_groups[m_groupIdx + 1] : null;
            }
            else
            {
                NextGroup = null;
                NextGroup2 = null;
                gameObject.SetActive(false);
            }
        }
    }
}