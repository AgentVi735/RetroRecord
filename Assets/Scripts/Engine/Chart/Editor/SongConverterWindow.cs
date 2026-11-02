using System;
using UnityEditor;
using UnityEngine;


namespace RetroRecord.Engine.Chart.Editor
{
    public class SongConverterWindow : EditorWindow
    {
        private AlbumLibrary albumLibrary
        {
            get => _albumLibrary;
            set
            {
                _albumLibrary = value;
                EditorPrefs.SetString("AlbumPath", AssetDatabase.GetAssetPath(value));
            }
        }

        [SerializeField] private AlbumLibrary _albumLibrary;
        [SerializeField] private Song[] values = Array.Empty<Song>();
        private bool hasInitialized;

        private SerializedObject serializedObject;
        private SerializedProperty valuesProperty;

        [MenuItem("Tools/Song Converter")]
        public static void ShowWindow()
        {
            GetWindow<SongConverterWindow>("Song Converter");
        }

        private void OnEnable()
        {
            if (albumLibrary == null || serializedObject == null)
                hasInitialized = false;
        }

        private void Initialize()
        {
            hasInitialized = true;
            string path = EditorPrefs.GetString("AlbumPath");
            if (string.IsNullOrEmpty(path))
                path = "Assets/Data/Album Library.asset";
            if (AssetDatabase.AssetPathExists(path))
            {
                albumLibrary = AssetDatabase.LoadAssetAtPath<AlbumLibrary>(path);
                Debug.Log($"Loaded album library from {path}");
            }
            else
            {
                Debug.Log($"Couldn't load album library at {path}");
            }

            serializedObject = new SerializedObject(this);
            valuesProperty = serializedObject.FindProperty("values");
        }

        private void OnGUI()
        {
            if (!hasInitialized)
                Initialize();
            GUILayout.Space(10);

            albumLibrary = (AlbumLibrary)EditorGUILayout.ObjectField(albumLibrary, typeof(Album), false);

            GUILayout.Space(10);

            GUILayout.Label("Songs to ignore", EditorStyles.boldLabel);

            // int newSize = EditorGUILayout.IntField("Size", values.Length);
            // if (newSize != values.Length)
            //     Array.Resize(ref values, newSize);
            //
            // for (int i = 0; i < values.Length; i++)
            //     values[i] = (SongObject)EditorGUILayout.ObjectField(new GUIContent($"Song {i}"), values[i], typeof(SongObject), false);

            serializedObject.Update();

            // Show the array with children (elements) visible
            EditorGUILayout.PropertyField(valuesProperty, true);

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            if (GUILayout.Button("Convert all songs from library"))
            {
                AssetDatabase.Refresh();
                SongConverter.ConvertAllSongsFromLibrary(albumLibrary, values);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (!GUILayout.Button("Scan all songs and convert")) return;
            AssetDatabase.Refresh();
            SongConverter.ConvertAllSongs(albumLibrary);
            AssetDatabase.Refresh();
        }
    }
}