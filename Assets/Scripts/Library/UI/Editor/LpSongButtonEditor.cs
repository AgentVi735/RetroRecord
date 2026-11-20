using RetroRecord.General.UI;
using UnityEditor;

namespace RetroRecord.Library.UI.Editor
{
    [CustomEditor(typeof(LpSongButton))]
    public class LpSongButtonEditor : UIButtonEditor
    {
        private SerializedProperty[] m_properties;
        
        private readonly string[] m_propertyNames = {
            "m_text"
        };
        
        protected override void OnEnable()
        {
            base.OnEnable();
        
            m_properties = new SerializedProperty[m_propertyNames.Length];
            for (int i = 0; i < m_propertyNames.Length; i++)
                m_properties[i] = serializedObject.FindProperty(m_propertyNames[i]);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            foreach (SerializedProperty property in m_properties)
            {
                if (property == null)
                    continue;
        
                EditorGUILayout.PropertyField(property);
            }
        
            EditorGUILayout.Space();
        
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}