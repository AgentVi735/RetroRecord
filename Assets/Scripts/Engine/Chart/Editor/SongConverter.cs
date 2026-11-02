using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using RetroRecord.Engine.Chart.Events;
using UnityEditor;
#endif

namespace RetroRecord.Engine.Chart.Editor
{

    public static class SongConverter
    {
        private const int RESOLUTION = 192;
        private const string COVER_FILE_NAME = "Cover.png";
        
        private const int millisecondsInSecond = 1000;

        private const string chartFileSongDataHeader = "[Song]";
        private const string chartFileSyncTrackHeader = "[SyncTrack]";
        private const string chartFileEventsHeader = "[Events]";
        private const string chartFileGHLGuitarHardHeader = "[HardGHLGuitar]";
        private const string chartFileGHLGuitarNormalHeader = "[NormalGHLGuitar]";
        private const string chartFileGHLGuitarEasyHeader = "[EasyGHLGuitar]";

        private static RawSongInfo s_currentRawSongInfo;

#if UNITY_EDITOR
        public static void ConvertAllSongs(AlbumLibrary albumLibrary)
        {
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
                oldSong.SyncTrack = song.SyncTrack;
                oldSong.Events = song.Events;
                song = oldSong;
            }

            // string chartPath = Path.Combine(assetsPath, "Chart.asset");
            // AssetDatabase.CreateAsset(song, chartPath);
            // Debug.Log("Chart saved to: " + fullPath + "Chart.asset");

            AssetDatabase.Refresh();

            song.SongInfo = new SongInfo
            {
                SongName = rawSongInfo.songName
            };

            // if (rawSongInfo.audioFileName != "")
            // {
                // song.SongInfo.Audio = AssetDatabase.LoadAssetAtPath<AudioClip>(assetsPath + "Editor/" + rawSongInfo.audioFileName);

                // tempPath = Path.Combine("Assets", "StreamingAssets", "Songs");
                // if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.artistName)))
                //     AssetDatabase.CreateFolder(tempPath, rawSongInfo.artistName);
                // tempPath = Path.Combine(tempPath, rawSongInfo.artistName);
                // if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.albumName)))
                //     AssetDatabase.CreateFolder(tempPath, rawSongInfo.albumName);
                // tempPath = Path.Combine(tempPath, rawSongInfo.albumName);
                // if (!AssetDatabase.AssetPathExists(Path.Combine(tempPath, rawSongInfo.songName)))
                //     AssetDatabase.CreateFolder(tempPath, rawSongInfo.songName);
                // tempPath = Path.Combine(tempPath, rawSongInfo.songName);
                // AssetDatabase.CopyAsset(assetsPath + "Editor/" + rawSongInfo.audioFileName,
                //     Path.Combine(tempPath, rawSongInfo.audioFileName));

                // songObj.songPath = Path.Combine("Songs", rawSongInfo.artistName, info.albumName, info.songName,
                //     info.audioFileName);
                // songObj.songPath = songObj.songPath.Replace('\\', '/');
            // }

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

            if (AssetDatabase.AssetPathExists(assetsAlbumPath + COVER_FILE_NAME))
                album.Cover = AssetDatabase.LoadAssetAtPath<Sprite>(assetsAlbumPath + COVER_FILE_NAME);

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
                    case chartFileSongDataHeader:
                        songDataIdx = FindBrackets(i, txt);
                        break;
                    case chartFileSyncTrackHeader:
                        syncTrackIdx = FindBrackets(i, txt);
                        break;
                    case chartFileEventsHeader:
                        eventsIdx = FindBrackets(i, txt);
                        break;
                    case chartFileGHLGuitarHardHeader:
                        ghlGuitarHardIdx = FindBrackets(i, txt);
                        break;
                    case chartFileGHLGuitarNormalHeader:
                        ghlGuitarNormalIdx = FindBrackets(i, txt);
                        break;
                    case chartFileGHLGuitarEasyHeader:
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
                    if (text == "{" || text.Contains("TS"))
                        continue;
                    if (text == "}")
                        break;

                    if (text.Contains("Name"))
                    {
                        string a = text[10..];
                        rawSongInfo.songName = a.Replace("\"", "");
                    }
                    else if (text.Contains("Artist"))
                    {
                        string a = text[12..];
                        rawSongInfo.artistName = a.Replace("\"", "");
                    }
                    else if (text.Contains("Album"))
                    {
                        string a = text[11..];
                        rawSongInfo.albumName = a.Replace("\"", "");
                    }
                    else if (text.Contains("Year"))
                    {
                        string a = text[12..];
                        rawSongInfo.year = a.Replace("\"", "");
                    }
                    else if (text.Contains("Genre"))
                    {
                        string a = text[11..];
                        rawSongInfo.genre = a.Replace("\"", "");
                    }
                    else if (text.Contains("MusicStream"))
                    {
                        string a = text[17..];
                        const char delimiter = '\\';
                        int index = a.LastIndexOf(delimiter);
                        if (index != -1)
                            a = a[(index + 1)..];
                        rawSongInfo.audioFileName = a.Replace("\"", "");
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
            song.SyncTrack = new SyncTrack();
            if (syncTrackIdx.x != -1)
            {
                song.SyncTrack.TempoChanges = tempoChangesArray;
                TimeSignatureChange[] timeSignaturesArray = timeSignatures.ToArray();
                CalculateTimeSignatureTimes(timeSignaturesArray);
                song.SyncTrack.TimeSignatureChanges = timeSignaturesArray;
            }

            if (charts.Count > 0)
                song.Charts = charts.ToArray();

            song.Events = eventsArray;
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
            if (text.Contains("E"))
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

            if (!type.StartsWith("L")) return item; // LaneSwitch
            LaneSwitchEvent laneSwitch = (LaneSwitchEvent)item;
            string idx = type.Remove(0, 1);
            int.TryParse(idx, out int idxInt);
            laneSwitch.Lanes = idxInt;

            return laneSwitch;
        }

        private static SongItem GetSyncTrackItem(string text)
        {
            if (text.Contains("TS"))
                return GetTimeSignature(text);
            return text.Contains("B") ? GetTempoChange(text) : null;
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
            SongItem item = new();
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

            if (!text.Contains("ShowInfo")) return item;
            ShowSongInfoEvent showSongInfoEvent = (ShowSongInfoEvent)item;
            return showSongInfoEvent;

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
            itemTempoFloat /= millisecondsInSecond;
            item.Tempo = itemTempoFloat;

            return item;
        }

        private static Chart GetGroups(string[] file, Vector2Int idx, Difficulty difficulty)
        {
            List<NoteGroup> noteGroups = new();
            NoteGroup currentGroup = new();
            List<Note> notes = new();
            uint currentTick = 0;

            List<SongItem> localEvents = new();

            for (int i = idx.x; i < idx.y; i++)
            {
                string text = file[i];
                if (text == "{")
                    continue;
                if (text == "}")
                    break;

                SongItem item = GetNote(text);
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
                        localEvents.Add(item);
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

            Chart chart = new()
            {
                Difficulty = difficulty,
                Notes = noteGroups.ToArray(),
                LocalEvents = localEvents.ToArray(),
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

                double beats = deltaTicks / (double)RESOLUTION;
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

            foreach (SongItem item in items)
                item.Time = TickToSecondsAlt(item.Tick);
        }

        private static void CalculateSongItemTimes(NoteGroup[] noteGroups, SongItem[] laneSwitches)
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

            foreach (SongItem item in laneSwitches)
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

            double beats = (double)deltaTicks / RESOLUTION;

            return current.Time + beats * (60.0 / current.Tempo);
        }
#endif
    }
}