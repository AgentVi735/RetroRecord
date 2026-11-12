using System.Collections.Generic;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Settings;
using RetroRecord.Engine.Statistics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRecord.Engine.Player.Renderer
{
    public class HighwayRenderer : MonoBehaviour
    {
        [SerializeField]
        private NotesManager m_notesManager;
        
        [SerializeField]
        private LaneSettings[] m_laneSettings;
        
        [SerializeField]
        private Transform m_cam;

        [SerializeField]
        private float m_camOffset;

        [SerializeField]
        private SongPlayer m_songPlayer;

        [SerializeField]
        private Transform m_notePoolParent;

        [SerializeField]
        private NoteObject m_noteObjectPrefab;

        [SerializeField]
        private int m_amountOfNotesToPool;

        private List<NoteObject> m_activeNotes;
        private List<NoteObject> m_pooledNotes;

        [SerializeField]
        private Transform m_beatlinePoolParent;

        [SerializeField]
        private BeatlineObject m_beatlineObjectPrefab;

        [SerializeField]
        private int m_amountOfBeatlinesToPool;

        private List<BeatlineObject> m_activeBeatlines;
        private List<BeatlineObject> m_pooledBeatlines;

        [SerializeField]
        private float m_ySpawnPos;

        public HighwayLane[] Lanes;

        [SerializeField]
        private float m_yEndPos;

        [SerializeField]
        private Transform m_hitboxTransform;

        [SerializeField]
        private Transform m_perfectHitboxTransform;

        public void Initialize()
        {
            m_pooledNotes = new List<NoteObject>();
            for (int i = 0; i < m_amountOfNotesToPool; i++)
            {
                NoteObject tmp = Instantiate(m_noteObjectPrefab, m_notePoolParent);
                tmp.gameObject.SetActive(false);
                m_pooledNotes.Add(tmp);
            }

            m_activeNotes = new List<NoteObject>();

            m_pooledBeatlines = new List<BeatlineObject>();
            for (int i = 0; i < m_amountOfBeatlinesToPool; i++)
            {
                BeatlineObject tmp = Instantiate(m_beatlineObjectPrefab, m_beatlinePoolParent);
                tmp.Initialize();
                m_pooledBeatlines.Add(tmp);
            }

            m_activeBeatlines = new List<BeatlineObject>();

            for (int i = 0; i < Lanes.Length; i++)
            {
                HighwayLane lane = Lanes[i];
                LaneSettings settings;
                if (EngineSettings.CurrentDifficultySettings.StartingLane >= 2)
                    settings = m_laneSettings[0];
                else
                    settings = lane.Idx < 4 ? m_laneSettings[0] : m_laneSettings[1];
                if (i >= EngineSettings.CurrentDifficultySettings.StartingLane &&
                    i < EngineSettings.CurrentDifficultySettings.LaneAmount +
                    EngineSettings.CurrentDifficultySettings.StartingLane)
                    lane.Initialize(true, settings);
                else
                    lane.Initialize(false, settings);
            }

            float xScale = EngineSettings.CurrentDifficultySettings.LaneAmount * 1.5f +
                           (EngineSettings.CurrentDifficultySettings.AreLanesInMiddle ? 0.25f : 1.25f);
            m_hitboxTransform.localScale = new Vector3(xScale, SyncTrack.TimeToYPos(EngineSettings.NoteHitboxSec), 1);
            m_perfectHitboxTransform.localScale =
                new Vector3(xScale, SyncTrack.TimeToYPos(EngineSettings.PerfectHitboxSec), 1);

            UpdatePos(0);

            m_notesManager.OnNoteSpawn += SpawnNote;
        }

        public void SetLaneAction(int idx, InputAction action) => Lanes[idx].SetActionEvents(action);

        private NoteObject GetPooledNote()
        {
            for (int i = 0; i < m_amountOfNotesToPool; i++)
            {
                if (!m_pooledNotes[i].gameObject.activeInHierarchy)
                    return m_pooledNotes[i];
            }

            return null;
        }

        private BeatlineObject GetPooledBeatline()
        {
            for (int i = 0; i < m_amountOfBeatlinesToPool; i++)
            {
                if (!m_pooledBeatlines[i].gameObject.activeInHierarchy)
                    return m_pooledBeatlines[i];
            }

            return null;
        }

        public void SpawnNote(Note note)
        {
            if (m_activeNotes.Count == m_amountOfNotesToPool)
                return;
            NoteObject obj = GetPooledNote();
            obj.Load(note, Lanes[note.Lane].transform);
            m_activeNotes.Add(obj);
        }

        public void SpawnBeatline(Beatline beatline)
        {
            if (m_activeBeatlines.Count == m_amountOfBeatlinesToPool)
                return;
            BeatlineObject obj = GetPooledBeatline();
            obj.Load(beatline);
            m_activeBeatlines.Add(obj);
        }

        public void ManualUpdate()
        {
            if (!SongPlayer.s_Instance.HasStarted) return;

            for (int i = 0; i < m_activeNotes.Count; i++)
            {
                NoteObject note = m_activeNotes[i];
                if (!(note.DespawnTime < SongPlayer.ConvertedCurrentInputTime)) continue;
                note.Unload();
                m_activeNotes.Remove(note);
            }

            for (int i = 0; i < m_activeBeatlines.Count; i++)
            {
                BeatlineObject beatline = m_activeBeatlines[i];
                if (!(beatline.Time < SongPlayer.ConvertedCurrentInputTime)) continue;
                beatline.Unload();
                m_activeBeatlines.Remove(beatline);
            }
        }

        public void UpdatePos(float newPos) => m_cam.position = new Vector3(0, newPos + m_camOffset, -10);

        public void HitNote(Note item, InputAction action, bool isPerfect)
        {
            StatisticsManager.s_Instance.HitNote(isPerfect);
            foreach (NoteObject note in m_activeNotes)
            {
                if (note.Note.Tick != item.Tick || note.Note.Lane != item.Lane)
                    continue;
                if (item.TickSustainLength > 0)
                    note.HitSustain(action);
                else
                {
                    note.Unload();
                    m_activeNotes.Remove(note);
                }

                return;
            }
        }

        public void MissNote(Note item)
        {
            StatisticsManager.s_Instance.MissNote();
            foreach (NoteObject note in m_activeNotes)
            {
                if (note.Note.Tick != item.Tick || note.Note.Lane != item.Lane) continue;
                note.Miss();

                return;
            }
        }

        public void ChangeLaneLayout(string lanesLayout)
        {
            foreach (HighwayLane lane in Lanes)
            {
                bool couldParse = int.TryParse($"{lanesLayout[lane.Idx]}", out int idx);
                if (!couldParse)
                {
                    Debug.LogError($"Couldn't parse laneLayout to int. Lane idx: {lane.Idx} | Layout: {lanesLayout}");
                    continue;
                }
                LaneSettings settings = m_laneSettings[idx];
                lane.ChangeColor(settings);
            }
        }
    }
}