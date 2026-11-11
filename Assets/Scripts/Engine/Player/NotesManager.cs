using System;
using System.Collections.Generic;
using System.Linq;
using RetroRecord.Engine.Chart;
using RetroRecord.Engine.Chart.Events;
using RetroRecord.Engine.Effects;
using RetroRecord.Engine.Player.Renderer;
using RetroRecord.Engine.Settings;
using RetroRecord.Engine.Statistics;
using RetroRecord.Engine.UI;
using UnityEngine;

namespace RetroRecord.Engine.Player
{
    public class NotesManager : MonoBehaviour
    {
        [SerializeField]
        private HighwayRenderer m_highwayRenderer;

        [SerializeField]
        private InputManager m_inputManager;

        [SerializeField]
        private SongPlayer m_songPlayer;

        [SerializeField]
        private SongInfoDisplay m_songInfoDisplay;

        [SerializeField]
        private EffectsManager m_effectManager;

        private NoteGroup[] m_noteGroups;
        private TempoChange[] m_tempoChanges;
        private TimeSignatureChange[] m_timeSignatureChanges;
        private LocalEventsTrack m_localEvents;
        private GlobalEventsTrack m_globalEvents;

        private readonly List<Beatline> m_beatlines = new();
        private List<Beatline> m_strongBeatlines = new();

        private bool m_isPlaying;

        private int m_currentGroupIndex;
        private int m_currentTempoIndex;
        private int m_currentTimeSignatureIndex;
        private int m_currentLocalEventIndex;
        private int m_currentGlobalEventIndex;
        private int m_currentBeatlineIndex;

        public static float s_CurrentTempo { get; private set; } = 120;

        public delegate uint GetBeatlineRatePower(TimeSignatureChange timeSignature);

        public delegate BeatlineType GetBeatlineType(TimeSignatureChange timeSignature, uint beatlineCount);


        public void Initialize()
        {
            Song song = EngineSettings.Song;
            Chart.Chart noteChart =
                song.Charts.FirstOrDefault(difficultyChart =>
                    difficultyChart.Difficulty == EngineSettings.Difficulty);

            if (noteChart == null)
                return;

            m_noteGroups = noteChart.Notes;
            m_tempoChanges = song.BeatTrack.TempoChanges;
            m_timeSignatureChanges = song.BeatTrack.TimeSignatureChanges;
            m_localEvents = noteChart.LocalEvents;
            m_globalEvents = song.GlobalEvents;

            double spawnTime = 0;
            if (EngineSettings.StartTime > 0)
                spawnTime = EngineSettings.StartTime + EngineSettings.NoteSpawnDistance;

            for (int i = 0; i < m_tempoChanges.Length; i++)
            {
                TempoChange item = m_tempoChanges[i];
                if (item.Time <= spawnTime)
                {
                    m_currentTempoIndex = i;
                    s_CurrentTempo = item.Tempo;
                    SyncTrack.s_CurrentTempoChange = item;
                }
                else
                    break;
            }

            TimeSignatureChange previousChange = null;
            for (int i = 0; i < m_timeSignatureChanges.Length; i++)
            {
                TimeSignatureChange item = m_timeSignatureChanges[i];
                double quarterNoteCount = 0;
                if (item.Tick > 0 && previousChange != null)
                    quarterNoteCount = previousChange.QuarterNoteCount +
                                       previousChange.GetQuarterNoteProgress(item.Tick);

                item.QuarterNoteCount = quarterNoteCount;
                previousChange = item;

                if (!(item.Time <= spawnTime)) continue;
                m_currentTempoIndex = i;
                SyncTrack.s_CurrentTimeSignature = item;
            }

            for (int i = 0; i < m_noteGroups.Length; i++)
            {
                NoteGroup group = m_noteGroups[i];
                if (group.Time <= spawnTime)
                    m_currentGroupIndex = i;

                foreach (Note note in group.Notes)
                    note.WasHit = false;
            }

            m_localEvents.SetItems();
            m_currentLocalEventIndex = m_localEvents.GetCurrentTimeIndex(spawnTime);
            
            m_globalEvents.SetItems();
            m_currentGlobalEventIndex = m_globalEvents.GetCurrentTimeIndex(spawnTime);

            GenerateBeatlines(noteChart.LastTick);
            RefreshStrongBeats();

            for (int i = 0; i < m_beatlines.Count; i++)
            {
                Beatline item = m_beatlines[i];
                if (item.Time <= spawnTime - EngineSettings.NoteSpawnDistance)
                    m_currentBeatlineIndex = i;
                else
                    break;
            }

            int noteCount = m_noteGroups.Sum(group => group.Notes.Length);
            StatisticsManager.s_Instance.SetTotalNotes((uint)(noteCount - m_currentGroupIndex));

            m_inputManager.Initialize(m_noteGroups, m_currentGroupIndex);
        }

        public void ManualUpdate()
        {
            if (!SongPlayer.s_Instance.HasStarted)
                return;

            m_highwayRenderer.UpdatePos(SyncTrack.TimeToYPos(SongPlayer.ConvertedCurrentInputTime));

            // Check for next note
            if (m_currentGroupIndex < m_noteGroups.Length)
            {
                NoteGroup currentGroup = m_noteGroups[m_currentGroupIndex];
                while (m_currentGroupIndex < m_noteGroups.Length && currentGroup.Time <=
                       SongPlayer.ConvertedCurrentInputTime + EngineSettings.NoteSpawnDistance)
                {
                    foreach (Note note in currentGroup.Notes)
                        ProcessItem(note);

                    m_currentGroupIndex++;
                    if (m_currentGroupIndex < m_noteGroups.Length)
                        currentGroup = m_noteGroups[m_currentGroupIndex];
                }
            }

            // Check for current tempo change
            if (m_currentTempoIndex < m_tempoChanges.Length)
            {
                TempoChange currentTempoChange = m_tempoChanges[m_currentTempoIndex];
                while (m_currentTempoIndex < m_tempoChanges.Length &&
                       currentTempoChange.Time <= SongPlayer.ConvertedCurrentInputTime)
                {
                    ProcessItem(currentTempoChange);

                    m_currentTempoIndex++;
                    if (m_currentTempoIndex < m_tempoChanges.Length)
                        currentTempoChange = m_tempoChanges[m_currentTempoIndex];
                }
            }
            
            // Check for current time signature change
            if (m_currentTimeSignatureIndex < m_timeSignatureChanges.Length)
            {
                TimeSignatureChange currentTimeSignature = m_timeSignatureChanges[m_currentTimeSignatureIndex];
                while (m_currentTimeSignatureIndex < m_timeSignatureChanges.Length &&
                       currentTimeSignature.Time <= SongPlayer.ConvertedCurrentInputTime)
                {
                    ProcessItem(currentTimeSignature);

                    m_currentTimeSignatureIndex++;
                    if (m_currentTimeSignatureIndex < m_timeSignatureChanges.Length)
                        currentTimeSignature = m_timeSignatureChanges[m_currentTimeSignatureIndex];
                }
            }

            // Check for current local event
            if (m_currentLocalEventIndex < m_localEvents.Items.Length)
            {
                EventItem currentEventItem = m_localEvents.Items[m_currentLocalEventIndex];
                while (m_currentLocalEventIndex < m_localEvents.Items.Length &&
                       currentEventItem.Time <= SongPlayer.ConvertedCurrentInputTime)
                {
                    ProcessEvent(currentEventItem);

                    m_currentLocalEventIndex++;
                    if (m_currentLocalEventIndex < m_localEvents.Items.Length)
                        currentEventItem = m_localEvents.Items[m_currentLocalEventIndex];
                }
            }

            // Check for current global event
            if (m_currentGlobalEventIndex < m_globalEvents.Items.Length)
            {
                EventItem currentEventItem = m_globalEvents.Items[m_currentGlobalEventIndex];
                while (m_currentGlobalEventIndex < m_globalEvents.Items.Length &&
                       currentEventItem.Time <= SongPlayer.ConvertedCurrentInputTime)
                {
                    ProcessEvent(currentEventItem);

                    m_currentGlobalEventIndex++;
                    if (m_currentGlobalEventIndex < m_globalEvents.Items.Length)
                        currentEventItem = m_globalEvents.Items[m_currentGlobalEventIndex];
                }
            }

            // Check for current beatline
            if (m_currentBeatlineIndex >= m_beatlines.Count) return;
            Beatline currentBeatline = m_beatlines[m_currentBeatlineIndex];
            while (m_currentBeatlineIndex < m_beatlines.Count &&
                   currentBeatline.Time <= SongPlayer.ConvertedCurrentInputTime +
                   EngineSettings.NoteSpawnDistance)
            {
                m_highwayRenderer.SpawnBeatline(currentBeatline);

                m_currentBeatlineIndex++;
                if (m_currentBeatlineIndex < m_beatlines.Count)
                    currentBeatline = m_beatlines[m_currentBeatlineIndex];
            }
        }

        private void ProcessItem(SongItem item)
        {
            switch (item.GetItemType())
            {
                case SongItemType.Note:
                    m_highwayRenderer.SpawnNote((Note)item);
                    break;
                case SongItemType.TempoChange:
                    TempoChange change = (TempoChange)item;
                    s_CurrentTempo = change.Tempo;
                    SyncTrack.s_CurrentTempoChange = change;
                    break;
                case SongItemType.TimeSignature:
                    SyncTrack.s_CurrentTimeSignature = (TimeSignatureChange)item;
                    break;
            }
        }

        private void ProcessEvent(EventItem item)
        {
            switch (item.ItemType)
            {
                case SongItemType.LaneSwitch:
                    m_highwayRenderer.ChangeLaneLayout(m_localEvents.LaneSwitchEvents[item.Index].Lanes);
                    break;
                case SongItemType.ShowSongInfo:
                    m_songInfoDisplay.ShowSongNameEvent(item.Time);
                    break;
                case SongItemType.Effect:
                    m_effectManager.ShowEffect(m_globalEvents.Effects[item.Index]);
                    break;
            }
        }

        public void GenerateBeatlines(uint lastTick, bool keepExisting = false)
        {
            GenerateBeatlines(lastTick, GetBeatlinePower, GetBeatlineType, keepExisting);
            return;

            static uint GetBeatlinePower(TimeSignatureChange currentTimeSig)
            {
                return 0;
            }

            static BeatlineType GetBeatlineType(TimeSignatureChange currentTimeSig, uint beatlineCount)
            {
                // The denominator at which to generate strong beats
                const uint strongStep = 4;

                uint measureBeatCount = beatlineCount % currentTimeSig.Numerator;
                uint strongRate = currentTimeSig.Denominator <= 4
                    ? 1
                    : currentTimeSig.Denominator / strongStep;

                // 1/x time signatures
                if (currentTimeSig.Numerator == 1)
                {
                    // Only the first beatline should be a measure line
                    if (beatlineCount < 1)
                        return BeatlineType.Measure;

                    // The rest are emphasised periodically
                    return beatlineCount % strongRate == 0 ? BeatlineType.Strong : BeatlineType.Weak;
                }

                // Measure lines
                if (measureBeatCount == 0)
                    return BeatlineType.Measure;

                // Only use weak beats on x/8 or greater
                if (currentTimeSig.Denominator <= 4)
                    return BeatlineType.Strong;

                // Emphasize beatlines periodically
                if (measureBeatCount % strongRate == 0)
                {
                    // Always force the last beatline of a measure to be weak
                    return measureBeatCount == currentTimeSig.Numerator - 1 ? BeatlineType.Weak : BeatlineType.Strong;
                }

                return BeatlineType.Weak;
            }
        }

        public void GenerateBeatlines(uint lastTick, GetBeatlineRatePower getBeatlinePower,
            GetBeatlineType getBeatlineType,
            bool keepExisting = false)
        {
            lastTick++;

            uint lastBeatlineTick = 0;

            if (!keepExisting)
            {
                m_beatlines.Clear();
                m_beatlines.Capacity = (int)(lastTick / EngineSettings.RESOLUTION);
            }
            else
            {
                // Find the current last beatline
                if (m_beatlines.Count > 0)
                {
                    lastBeatlineTick = m_beatlines[^1].Tick;
                }

                // In case lastTick is before the current final beatline, do nothing
                if (lastTick <= lastBeatlineTick)
                {
                    return;
                }
            }


            // List indexes
            int tempoIndex = 0;
            int timeSigIndex = 0;

            // Iterate through all the time signatures
            TimeSignatureChange currentTimeSig = m_timeSignatureChanges[timeSigIndex++];
            for (; timeSigIndex < m_timeSignatureChanges.Length; timeSigIndex++)
            {
                TimeSignatureChange nextTimeSig = m_timeSignatureChanges[timeSigIndex];

                // Determine bounds
                uint startTick = currentTimeSig.Tick;
                uint endTick = nextTimeSig.Tick - 1;

                // If startTick and endTick are both prior to the current last beatline, we don't need to do anything
                if (startTick <= lastBeatlineTick && endTick <= lastBeatlineTick)
                {
                    continue;
                }

                // If startTick is prior to the current last beatline, we need to move it to the last beatline
                startTick = Math.Max(startTick, lastBeatlineTick);

                // Generate beatlines for this time signature
                GenerateBeatsForTimeSignature(currentTimeSig, startTick, endTick);
                currentTimeSig = nextTimeSig;
            }


            uint finalStartTick = Math.Max(currentTimeSig.Tick, lastBeatlineTick);
            // Final time signature
            GenerateBeatsForTimeSignature(currentTimeSig, finalStartTick, lastTick);

            m_beatlines.TrimExcess();
            RefreshStrongBeats();
            return;

            void GenerateBeatsForTimeSignature(TimeSignatureChange timeSignature, uint startTick, uint endTick)
            {
                uint beatlineTickFactor =
                    timeSignature.Denominator * (uint)Math.Pow(2, getBeatlinePower(timeSignature));
                uint beatlineTickRate = (EngineSettings.RESOLUTION * 4) / beatlineTickFactor;

                uint beatlineCount = 0;
                uint currentTick = startTick;
                var currentTempo = m_tempoChanges[tempoIndex];
                while (currentTick <= endTick)
                {
                    // Progress to current tempo
                    while (tempoIndex < m_tempoChanges.Length - 1)
                    {
                        var nextTempo = m_tempoChanges[tempoIndex + 1];
                        if (nextTempo.Tick > currentTick)
                            break;

                        currentTempo = nextTempo;
                        tempoIndex++;
                    }

                    var beatlineType = getBeatlineType(timeSignature, beatlineCount);

                    // Create beatline
                    double time = currentTempo.TickToTime(currentTick);
                    m_beatlines.Add(new Beatline(beatlineType, time, currentTick));
                    beatlineCount++;
                    currentTick += beatlineTickRate;
                }
            }
        }

        private void RefreshStrongBeats()
        {
            m_strongBeatlines.Clear();
            m_strongBeatlines.Capacity = m_beatlines.Count;
            foreach (var beatline in m_beatlines)
            {
                if (beatline.Type != BeatlineType.Weak)
                {
                    m_strongBeatlines.Add(beatline);
                }
            }

            m_strongBeatlines.TrimExcess();
        }
    }
}