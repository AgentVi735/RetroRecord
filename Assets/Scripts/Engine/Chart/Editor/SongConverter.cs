using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RetroRecord.Engine.Chart.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RetroRecord.Engine.Chart.Editor
{
    public static class SongConverter
    {
#if UNITY_EDITOR
        private const int k_millisecondsInSecond = 1000;

        private static RawSongInfo s_currentRawSongInfo;
        private static AlbumLibrary s_albumLibrary;
        
        private const string k_chartFileSongDataHeader = "[Song]";
        private const string k_chartFileSyncTrackHeader = "[SyncTrack]";
        private const string k_chartFileEventsHeader = "[Events]";
        private const string k_chartFileGhlGuitarHardHeader = "[HardGHLGuitar]";
        private const string k_chartFileGhlGuitarNormalHeader = "[NormalGHLGuitar]";
        private const string k_chartFileGhlGuitarEasyHeader = "[EasyGHLGuitar]";

        private const string k_songInfoNameKey = "Name";
        private const string k_songInfoArtistKey = "Artist";
        private const string k_songInfoAlbumKey = "Album";
        private const string k_songInfoYearKey = "Year";
        private const string k_songInfoGenreKey = "Genre";
        private const string k_songInfoAudioKey = "MusicStream";
        private const string k_songInfoDifficultyKey = "Difficulty";
        private const string k_songInfoCharterKey = "Charter";
        
        private const string k_noteKey = "N";
        private const string k_tempoChangeKey = "B";
        private const string k_timeSignatureChangeKey = "TS";
        
        private const string k_laneSwitchEventKey = "LS";
        private const string k_showSongInfoEventKey = "SI";
        private const string k_effectEventKey = "Ef";

        private const string k_flameEffectKey = "F";
        private const float k_flameEffectBeatCount = 1;
        private const string k_sparkEffectKey = "S";
        private const float k_sparkEffectBeatCount = 2;

        public static void ConvertAllSongs(AlbumLibrary albumLibrary)
        {
            s_albumLibrary = albumLibrary;
            
            string[] guidsFound = AssetDatabase.FindAssets("chart", new[] { Path.Combine("Assets", "Data") });

            List<string> paths = new();

            foreach (string guid in guidsFound)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".chart"))
                    continue;
                if (path.StartsWith("Assets"))
                    path = path.Remove(0, 6);
                if (path.Contains('.'))
                {
                    const char delimiter = '/';
                    int index = path.LastIndexOf(delimiter);
                    if (index != -1)
                        path = path[..(index + 1)];
                }

                if (path.EndsWith("Editor") || path.EndsWith("Editor/"))
                {
                    int amtToRemove = 6;
                    if (path.EndsWith('/'))
                        amtToRemove++;

                    path = path.Remove(path.Length - amtToRemove - 1, amtToRemove);
                }

                paths.Add(path);
            }

            foreach (string path in paths)
            {
                Song song = ConvertSong(path);
                if (song == null || albumLibrary.Albums.Contains(song.SongInfo.Album)) continue;
                List<Album> tempAlbums = albumLibrary.Albums.ToList();
                tempAlbums.Add(song.SongInfo.Album);
                albumLibrary.Albums = tempAlbums.OrderBy(album => album.ArtistName)
                    .ThenBy(album => album.AlbumName)
                    .ToArray();
            }

            albumLibrary.Albums = albumLibrary.Albums.OrderBy(album => album.ArtistName)
                .ThenBy(album => album.AlbumName)
                .ToArray();
            
            AssetDatabase.Refresh();
        }

        public static void ConvertAllSongsFromLibrary(AlbumLibrary library, Song[] songsToIgnore)
        {
            s_albumLibrary = library;
            List<string> paths = (from album in library.Albums
                from song in album.Songs
                where !songsToIgnore.Contains(song)
                where song != null
                select AssetDatabase.GetAssetPath(song)).ToList();

            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                if (path.StartsWith("Assets"))
                    path = path.Remove(0, 6);
                if (path.Contains('.'))
                {
                    const char delimiter = '/';
                    int index = path.LastIndexOf(delimiter);
                    if (index != -1)
                        path = path[..(index + 1)];
                }

                if (path.EndsWith("Editor") || path.EndsWith("Editor/"))
                {
                    int amtToRemove = 6;
                    if (path.EndsWith('/'))
                        amtToRemove++;

                    path = path.Remove(path.Length - amtToRemove - 1, amtToRemove);
                }

                paths[i] = path;
            }

            foreach (string path in paths)
                ConvertSong(path);
            
            AssetDatabase.Refresh();
        }

        private static Song ConvertSong(string folderPath)
        {
            string fullPath = Application.dataPath + folderPath;
            string assetsPath = "Assets" + folderPath;

            Song song = GetChart(fullPath);
            if (song == null)
                return null;
            RawSongInfo rawSongInfo = s_currentRawSongInfo;
            string objPath = assetsPath + rawSongInfo.songName + ".asset";
            if (!AssetDatabase.AssetPathExists(objPath))
            {
                string tempPath = Path.Combine("Assets", "Data");
                if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.artistName)))
                    AssetDatabase.CreateFolder(tempPath, rawSongInfo.artistName);
                tempPath = Path.Combine(tempPath, rawSongInfo.artistName);
                if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.albumName)))
                    AssetDatabase.CreateFolder(tempPath, rawSongInfo.albumName);
                tempPath = Path.Combine(tempPath, rawSongInfo.albumName);
                if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.songName)))
                    AssetDatabase.CreateFolder(tempPath, rawSongInfo.songName);
                AssetDatabase.CreateAsset(song, objPath);
                Debug.Log("Song created at: " + fullPath + rawSongInfo.songName + ".asset");
            }
            else
            {
                Song oldSong = AssetDatabase.LoadAssetAtPath<Song>(objPath);
                oldSong.Charts = song.Charts;
                oldSong.BeatTrack = song.BeatTrack;
                oldSong.GlobalEvents = song.GlobalEvents;
                song = oldSong;
            }

            AssetDatabase.Refresh();

            song.SongInfo ??= new SongInfo();
            song.SongInfo.SongName = rawSongInfo.songName;
            bool isValidDifficulty = ushort.TryParse(rawSongInfo.difficulty, out ushort difficulty);
            if (isValidDifficulty)
                song.SongInfo.Difficulty = difficulty;
            song.SongInfo.Charter = rawSongInfo.charter;
            if (rawSongInfo.audioFileName != "")
            {
                string audioPath = rawSongInfo.audioFileName;
                if (!string.IsNullOrEmpty(audioPath))
                {
                    bool isInAssets = audioPath.Contains("Assets");
                    if (isInAssets)
                    {
                        int idx = audioPath.IndexOf("Assets", StringComparison.Ordinal);
                        audioPath = audioPath.Remove(0, idx);
                    
                        song.SongInfo.Audio = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);
                    }
                }
            }

            string albumPath = Path.Combine("\\Data", rawSongInfo.artistName, rawSongInfo.albumName + "\\");
            string assetsAlbumPath = "Assets" + albumPath;
            string albumObjPath = assetsAlbumPath + rawSongInfo.albumName + ".asset";
            Album album;
            if (!AssetDatabase.AssetPathExists(albumObjPath))
            {
                album = ScriptableObject.CreateInstance<Album>();
                AssetDatabase.CreateAsset(album, albumObjPath);
                Debug.Log("Album created at: " + albumObjPath);
            }
            else
                album = AssetDatabase.LoadAssetAtPath<Album>(albumObjPath);

            album.AlbumName = rawSongInfo.albumName;
            if (string.IsNullOrEmpty(album.ArtistName))
                album.ArtistName = rawSongInfo.artistName;
            else if (album.ArtistName != rawSongInfo.artistName)
                song.SongInfo.OverrideArtist = rawSongInfo.artistName;
            else if (album.ArtistName == song.SongInfo.OverrideArtist)
                song.SongInfo.OverrideArtist = "";
            album.Year = Convert.ToInt32(rawSongInfo.year);
            album.Genre = rawSongInfo.genre;
            if (album.Songs == null || !album.Songs.Contains(song))
            {
                List<Song> tempSongs = album.Songs != null ? album.Songs.ToList() : new List<Song>();
                tempSongs.Add(song);
                album.Songs = tempSongs.ToArray();
            }

            if (AssetDatabase.AssetPathExists(assetsAlbumPath + s_albumLibrary.CoverFileName))
                album.Cover = AssetDatabase.LoadAssetAtPath<Sprite>(assetsAlbumPath + s_albumLibrary.CoverFileName);

            EditorUtility.SetDirty(album);
            Debug.Log("Saved Album at: " + albumObjPath);

            song.SongInfo.Album = album;

            EditorUtility.SetDirty(song);
            Debug.Log("Saved Song at: " + fullPath + rawSongInfo.songName + ".asset");

            return song;
        }

        private static Song GetChart(string fullPath)
        {
            string filePath = fullPath + "Editor/" + "Chart.chart";
            if (!File.Exists(filePath))
            {
                Debug.Log(filePath + " doesn't exist");
                return null;
            }

            string[] txt = File.ReadAllLines(filePath);

            // chart
            Vector2Int defaultState = new(-1, -1);
            Vector2Int songDataIdx = defaultState;
            Vector2Int syncTrackIdx = defaultState;
            List<TempoChange> tempoChanges = new();
            List<TimeSignatureChange> timeSignatures = new();
            RawSongInfo rawSongInfo = new();
            Vector2Int eventsIdx = defaultState;
            List<SongItem> events = new();
            Vector2Int ghlGuitarHardIdx = defaultState;
            Vector2Int ghlGuitarNormalIdx = defaultState;
            Vector2Int ghlGuitarEasyIdx = defaultState;
            List<Chart> charts = new();

            for (int i = 0; i < txt.Length; i++)
            {
                string text = txt[i];
                switch (text)
                {
                    case k_chartFileSongDataHeader:
                        songDataIdx = FindBrackets(i, txt);
                        break;
                    case k_chartFileSyncTrackHeader:
                        syncTrackIdx = FindBrackets(i, txt);
                        break;
                    case k_chartFileEventsHeader:
                        eventsIdx = FindBrackets(i, txt);
                        break;
                    case k_chartFileGhlGuitarHardHeader:
                        ghlGuitarHardIdx = FindBrackets(i, txt);
                        break;
                    case k_chartFileGhlGuitarNormalHeader:
                        ghlGuitarNormalIdx = FindBrackets(i, txt);
                        break;
                    case k_chartFileGhlGuitarEasyHeader:
                        ghlGuitarEasyIdx = FindBrackets(i, txt);
                        break;
                }
            }

            if (syncTrackIdx.x == -1)
            {
                Debug.LogError("No bpms set in song " + fullPath);
                return null;
            }

            TempoChange[] tempoChangesArray = null;
            SongItem[] eventsArray = null;
            if (songDataIdx.x != -1)
            {
                for (int i = songDataIdx.x; i < songDataIdx.y; i++)
                {
                    string text = txt[i];
                    if (text == "{" || text.Contains(k_timeSignatureChangeKey))
                        continue;
                    if (text == "}")
                        break;

                    if (text.Contains(k_songInfoNameKey))
                    {
                        string a = text[10..];
                        rawSongInfo.songName = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoArtistKey))
                    {
                        string a = text[12..];
                        rawSongInfo.artistName = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoAlbumKey))
                    {
                        string a = text[11..];
                        rawSongInfo.albumName = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoYearKey))
                    {
                        string a = text[12..];
                        rawSongInfo.year = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoGenreKey))
                    {
                        string a = text[11..];
                        rawSongInfo.genre = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoAudioKey))
                    {
                        string a = text[17..];
                        rawSongInfo.audioFileName = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoDifficultyKey))
                    {
                        string a = text[15..];
                        rawSongInfo.difficulty = a.Replace("\"", "");
                    }
                    else if (text.Contains(k_songInfoCharterKey))
                    {
                        string a = text[13..];
                        rawSongInfo.charter = a.Replace("\"", "");
                    }
                }
            }

            if (syncTrackIdx.x != -1)
            {
                for (int i = syncTrackIdx.x; i < syncTrackIdx.y; i++)
                {
                    string text = txt[i];
                    if (text == "{")
                        continue;
                    if (text == "}")
                        break;

                    SongItem item = GetSyncTrackItem(text);

                    switch (item.GetItemType())
                    {
                        case SongItemType.TempoChange:
                            tempoChanges.Add((TempoChange)item);
                            break;
                        case SongItemType.TimeSignature:
                            timeSignatures.Add((TimeSignatureChange)item);
                            break;
                    }
                }

                tempoChangesArray = tempoChanges.ToArray();
                CalculateTempoChanges(tempoChangesArray);
            }

            if (eventsIdx.x != -1)
            {
                for (int i = eventsIdx.x; i < eventsIdx.y; i++)
                {
                    string text = txt[i];
                    if (text == "{")
                        continue;
                    if (text == "}")
                        break;

                    SongItem item = GetEvent(text);

                    if (item == null) continue;
                    events.Add(item);
                }

                eventsArray = events.ToArray();
                CalculateSongItemTimes(eventsArray);
            }

            if (ghlGuitarHardIdx.x != -1)
            {
                Chart diffChart = GetGroups(txt, ghlGuitarHardIdx, Difficulty.Hard);
                CalculateSongItemTimes(diffChart.Notes, diffChart.LocalEvents);
                charts.Add(diffChart);
            }

            if (ghlGuitarNormalIdx.x != -1)
            {
                Chart diffChart = GetGroups(txt, ghlGuitarNormalIdx, Difficulty.Normal);
                CalculateSongItemTimes(diffChart.Notes, diffChart.LocalEvents);
                charts.Add(diffChart);
            }

            if (ghlGuitarEasyIdx.x != -1)
            {
                Chart diffChart = GetGroups(txt, ghlGuitarEasyIdx, Difficulty.Easy);
                CalculateSongItemTimes(diffChart.Notes, diffChart.LocalEvents);
                charts.Add(diffChart);
            }

            Song song = ScriptableObject.CreateInstance<Song>();
            song.BeatTrack = new BeatTrack();
            if (syncTrackIdx.x != -1)
            {
                song.BeatTrack.TempoChanges = tempoChangesArray;
                TimeSignatureChange[] timeSignaturesArray = timeSignatures.ToArray();
                CalculateTimeSignatureTimes(timeSignaturesArray);
                song.BeatTrack.TimeSignatureChanges = timeSignaturesArray;
            }

            if (charts.Count > 0)
                song.Charts = charts.ToArray();
            
            GlobalEventsTrack globalEventsTrack = new();
            List<ShowSongInfoEvent> showSongInfoEvents = new();
            List<EffectItem> effectEvents = new();

            if (eventsArray.Length > 0)
            {
                showSongInfoEvents.AddRange(eventsArray
                    .Where(songItem => songItem.GetItemType() == SongItemType.ShowSongInfo).Cast<ShowSongInfoEvent>());
                
                effectEvents.AddRange(eventsArray
                    .Where(songItem => songItem.GetItemType() == SongItemType.Effect).Cast<EffectItem>());
            }

            globalEventsTrack.ShowSongInfoEvents = showSongInfoEvents.ToArray();
            globalEventsTrack.Effects = effectEvents.ToArray();
            
            song.GlobalEvents = globalEventsTrack;
            s_currentRawSongInfo = rawSongInfo;

            return song;
        }

        private static Vector2Int FindBrackets(int currentIdx, string[] txt)
        {
            Vector2Int brackets = new(-1, -1);
            for (int i = currentIdx; i < txt.Length; i++)
            {
                string text = txt[i];

                switch (text)
                {
                    case "{":
                        brackets = new Vector2Int(i, brackets.y);
                        break;
                    case "}":
                        return new Vector2Int(brackets.x, i);
                }
            }

            return new Vector2Int(-1, -1);
        }

        private static SongItem GetNote(string text)
        {
            if (!text.Contains(k_noteKey))
                return GetLocalEvent(text);
            Note item = new();
            string tick = "";
            string lane = "";
            string length = "";
            int spaceAmt = 0;
            foreach (char character in text)
            {
                if (character == ' ')
                    spaceAmt++;
                else
                    switch (spaceAmt)
                    {
                        case 2:
                            tick += character;
                            break;
                        case 5:
                            lane += character;
                            break;
                        case 6:
                            length += character;
                            break;
                    }
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            byte.TryParse(lane, out byte laneByte);
            if (laneByte == 8)
                laneByte = 5;
            item.Lane = laneByte;
            if (item.Lane > 7)
                return null;

            uint.TryParse(length, out uint lengthUInt);
            if (lengthUInt > 0)
                item.TickSustainLength = lengthUInt;
            else
                item.TickSustainLength = 0;

            return item;
        }

        private static SongItem GetLocalEvent(string text)
        {
            SongItem item = new();
            string tick = "";
            string type = "";
            int spaceAmt = 0;
            foreach (char character in text)
            {
                if (character == ' ')
                    spaceAmt++;
                else
                    switch (spaceAmt)
                    {
                        case 2:
                            tick += character;
                            break;
                        case 5:
                            type += character;
                            break;
                    }
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            if (!type.StartsWith(k_laneSwitchEventKey)) return item;
            LaneSwitchEvent laneSwitch = LaneSwitchEvent.Clone(item);
            string idx = type.Remove(0, k_laneSwitchEventKey.Length);
            laneSwitch.Lanes = idx;

            return laneSwitch;
        }

        private static SongItem GetSyncTrackItem(string text)
        {
            if (text.Contains(k_timeSignatureChangeKey))
                return GetTimeSignature(text);
            return text.Contains(k_tempoChangeKey) ? GetTempoChange(text) : null;
        }

        private static TimeSignatureChange GetTimeSignature(string text)
        {
            TimeSignatureChange item = new();
            string tick = "";
            string numerator = "";
            string denominator = "";
            int spaceAmt = 0;
            foreach (char character in text)
            {
                if (character == ' ')
                    spaceAmt++;
                else
                    switch (spaceAmt)
                    {
                        case 2:
                            tick += character;
                            break;
                        case 5:
                            numerator += character;
                            break;
                        case 6:
                            denominator += character;
                            break;
                    }
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            uint.TryParse(numerator, out uint itemNumerator);
            item.Numerator = itemNumerator;

            item.Denominator = denominator switch
            {
                "0" => 1,
                "1" => 2,
                "3" => 8,
                "4" => 16,
                "5" => 32,
                "6" => 64,
                _ => 4
            };

            return item;
        }

        private static SongItem GetEvent(string text)
        {
            if (text.Contains(k_effectEventKey))
                return GetEffectEvent(text);
            return text.Contains(k_showSongInfoEventKey) ? GetShowSongInfoEvent(text) : null;
        }
        
        private static ShowSongInfoEvent GetShowSongInfoEvent(string text)
        {
            ShowSongInfoEvent item = new();
            string tick = "";
            int spaceAmt = 0;
            foreach (char character in text)
            {
                if (character == ' ')
                    spaceAmt++;
                else if (spaceAmt == 2)
                    tick += character;
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            return item;
        }
        
        private static EffectItem GetEffectEvent(string text)
        {
            EffectItem item = new();
            string tick = "";
            string effectType = "";
            int spaceAmt = 0;
            bool isLookingForType = false;
            foreach (char character in text)
            {
                switch (character)
                {
                    case ' ':
                        spaceAmt++;
                        break;
                    case '-':
                        isLookingForType = true;
                        break;
                    case '"':
                        isLookingForType = false;
                        break;
                    default:
                    {
                        if (isLookingForType)
                            effectType += character;
                        else if (spaceAmt == 2)
                            tick += character;
                        break;
                    }
                }
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            item.EffectType = effectType switch
            {
                k_flameEffectKey => EffectType.Flame,
                k_sparkEffectKey => EffectType.Spark,
                _ => EffectType.None
            };

            return item;
        }

        private static TempoChange GetTempoChange(string text)
        {
            TempoChange item = new();
            string tick = "";
            string itemBpm = "";
            int spaceAmt = 0;
            foreach (char character in text)
            {
                if (character == ' ')
                    spaceAmt++;
                else
                    switch (spaceAmt)
                    {
                        case 2:
                            tick += character;
                            break;
                        case 5:
                            itemBpm += character;
                            break;
                    }
            }

            uint.TryParse(tick, out uint tickUInt);
            item.Tick = tickUInt;

            float.TryParse(itemBpm, out float itemTempoFloat);
            itemTempoFloat /= k_millisecondsInSecond;
            item.Tempo = itemTempoFloat;

            return item;
        }

        private static Chart GetGroups(string[] file, Vector2Int idx, Difficulty difficulty)
        {
            List<NoteGroup> noteGroups = new();
            NoteGroup currentGroup = new();
            List<Note> notes = new();
            uint currentTick = 0;

            LocalEventsTrack localEventsTrack = new();
            List<LaneSwitchEvent> laneSwitchEvents = new();

            for (int i = idx.x; i < idx.y; i++)
            {
                string text = file[i];
                if (text == "{")
                    continue;
                if (text == "}")
                    break;

                SongItem item = GetNote(text);
                if (item == null)
                    continue;
                switch (item.GetItemType())
                {
                    case SongItemType.Note when currentTick == 0:
                        currentTick = item.Tick;
                        currentGroup = new NoteGroup
                        {
                            Tick = currentTick
                        };
                        notes.Add((Note)item);
                        break;
                    case SongItemType.Note when currentTick != item.Tick:
                        noteGroups.Add(currentGroup);
                        currentGroup.Notes = notes.ToArray();
                        notes.Clear();
                        notes.Add((Note)item);
                        currentTick = item.Tick;
                        currentGroup = new NoteGroup
                        {
                            Tick = currentTick
                        };
                        break;
                    case SongItemType.Note:
                    {
                        if (currentTick == item.Tick)
                            notes.Add((Note)item);
                        break;
                    }
                    case SongItemType.LaneSwitch:
                        LaneSwitchEvent laneSwitchEvent = (LaneSwitchEvent)item;
                        laneSwitchEvents.Add(laneSwitchEvent);
                        break;
                }
            }

            currentGroup.Notes = notes.ToArray();
            noteGroups.Add(currentGroup);

            uint lastTick = 0;

            foreach (Note item in noteGroups[^1].Notes)
            {
                if (item.Tick + item.TickSustainLength > lastTick)
                    lastTick = item.Tick + item.TickSustainLength;
            }

            localEventsTrack.LaneSwitchEvents = laneSwitchEvents.ToArray();
            Chart chart = new()
            {
                Difficulty = difficulty,
                Notes = noteGroups.ToArray(),
                LocalEvents = localEventsTrack,
                LastTick = lastTick
            };

            return chart;
        }

        private static TempoChange[] s_tempoChanges;

        private static void CalculateTempoChanges(TempoChange[] tempoChanges)
        {
            if (tempoChanges.Length == 0)
                return;

            float currentBPM = tempoChanges[0].Tempo;
            uint previousTick = 0;
            double currentTime = 0.0;

            foreach (TempoChange item in tempoChanges)
            {
                uint deltaTicks = item.Tick - previousTick;

                double beats = deltaTicks / (double)s_albumLibrary.Resolution;
                currentTime += beats * (60.0 / currentBPM);

                item.Time = currentTime;
                currentBPM = item.Tempo;

                previousTick = item.Tick;
            }

            s_tempoChanges = tempoChanges;
        }

        private static void CalculateTimeSignatureTimes(TimeSignatureChange[] items)
        {
            if (items.Length == 0)
                return;

            items = items.OrderBy(item => item.Tick).ToArray();

            foreach (TimeSignatureChange item in items)
                item.Time = TickToSecondsAlt(item.Tick);
        }

        private static void CalculateSongItemTimes(SongItem[] items)
        {
            if (items.Length == 0)
                return;

            items = items.OrderBy(item => item.Tick).ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                SongItem item = items[i];
                item.Time = TickToSecondsAlt(item.Tick);

                if (item.GetItemType() != SongItemType.Effect) continue;

                EffectItem effectItem = (EffectItem)item;

                switch (effectItem.EffectType)
                {
                    default:
                        continue;
                    case EffectType.Flame:
                        effectItem.TickDuration = (uint)Mathf.RoundToInt(k_flameEffectBeatCount * EngineSettings.RESOLUTION);
                        break;
                    case EffectType.Spark:
                        effectItem.TickDuration = (uint)Mathf.RoundToInt(k_sparkEffectBeatCount * EngineSettings.RESOLUTION);
                        break;
                }

                double endTime = TickToSecondsAlt(effectItem.Tick + effectItem.TickDuration);
                endTime -= effectItem.Time;
                effectItem.TimeDuration = endTime;
                items[i] = effectItem;
            }
        }

        private static void CalculateSongItemTimes(NoteGroup[] noteGroups, LocalEventsTrack localEvents)
        {
            if (noteGroups.Length == 0)
                return;

            noteGroups = noteGroups.OrderBy(item => item.Tick).ToArray();

            foreach (NoteGroup group in noteGroups)
            {
                group.Time = TickToSecondsAlt(group.Tick);

                foreach (Note note in group.Notes)
                {
                    note.Time = group.Time;
                    if (note.TickSustainLength > 0)
                        note.TimeSustainLength = TickToSecondsAlt(note.Tick + note.TickSustainLength) - note.Time;
                }
            }

            foreach (LaneSwitchEvent item in localEvents.LaneSwitchEvents)
                item.Time = TickToSecondsAlt(item.Tick);
        }

        private static double TickToSecondsAlt(uint tick)
        {
            TempoChange current = s_tempoChanges[0];

            foreach (TempoChange change in s_tempoChanges)
            {
                if (change.Tick > tick)
                    break;

                current = change;
            }

            uint deltaTicks = tick - current.Tick;

            double beats = (double)deltaTicks / s_albumLibrary.Resolution;

            return current.Time + beats * (60.0 / current.Tempo);
        }
#endif
    }
}