using System;
using UnityEditor;
using UnityEngine;


namespace RetroRecord.Engine.Chart.Editor
{
    public class SongConverterWindow : EditorWindow
    {
        private AlbumLibrary AlbumLibrary
        {
            get => m_albumLibrary;
            set
            {
                m_albumLibrary = value;
                EditorPrefs.SetString("AlbumPath", AssetDatabase.GetAssetPath(value));
            }
        }

        [SerializeField]
        private AlbumLibrary m_albumLibrary;

        [SerializeField]
        private Song[] m_values = Array.Empty<Song>();

        private bool m_hasInitialized;

        private SerializedObject m_serializedObject;
        private SerializedProperty m_valuesProperty;

        private SerializedProperty m_singleChartToUpdateProp;

        [SerializeField]
        private Song m_singleChart;

        [MenuItem("Tools/Song Converter")]
        public static void ShowWindow()
        {
            GetWindow<SongConverterWindow>("Song Converter");
        }

        private void OnEnable()
        {
            if (AlbumLibrary == null || m_serializedObject == null)
                m_hasInitialized = false;
        }

        private void Initialize()
        {
            m_hasInitialized = true;
            string path = EditorPrefs.GetString("AlbumPath");
            if (string.IsNullOrEmpty(path))
                path = "Assets/Data/Album Library.asset";
            if (AssetDatabase.AssetPathExists(path))
            {
                AlbumLibrary = AssetDatabase.LoadAssetAtPath<AlbumLibrary>(path);
                Debug.Log($"Loaded album library from {path}");
            }
            else
            {
                Debug.Log($"Couldn't load album library at {path}");
            }

            m_serializedObject = new SerializedObject(this);
            m_valuesProperty = m_serializedObject.FindProperty("m_values");
            m_singleChartToUpdateProp = m_serializedObject.FindProperty("m_singleChart");
        }

        private void OnGUI()
        {
            if (!m_hasInitialized)
                Initialize();
            GUILayout.Space(10);

            AlbumLibrary = (AlbumLibrary)EditorGUILayout.ObjectField(AlbumLibrary, typeof(Album), false);

            GUILayout.Space(10);

            GUILayout.Label("Songs to ignore", EditorStyles.boldLabel);

            // int newSize = EditorGUILayout.IntField("Size", values.Length);
            // if (newSize != values.Length)
            //     Array.Resize(ref values, newSize);
            //
            // for (int i = 0; i < values.Length; i++)
            //     values[i] = (SongObject)EditorGUILayout.ObjectField(new GUIContent($"Song {i}"), values[i], typeof(SongObject), false);

            m_serializedObject.Update();

            // Show the array with children (elements) visible
            if (m_valuesProperty != null)
                EditorGUILayout.PropertyField(m_valuesProperty, true);
            
            if (m_singleChartToUpdateProp != null)
                EditorGUILayout.PropertyField(m_singleChartToUpdateProp);

            m_serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);

            if (GUILayout.Button("Convert all songs from library"))
            {
                AssetDatabase.Refresh();
                SongConverter.ConvertAllSongsFromLibrary(AlbumLibrary, m_values);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Scan all songs and convert"))
            {
                AssetDatabase.Refresh();
                SongConverter.ConvertAllSongs(AlbumLibrary);
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (m_singleChart == null) return;
            if (!GUILayout.Button("Update selected song")) return;
            AssetDatabase.Refresh();
            SongConverter.ConvertOneSongsFromLibrary(AlbumLibrary, m_singleChart);
            AssetDatabase.Refresh();
        }
    }
}