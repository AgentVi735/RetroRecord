using UnityEditor;
using UnityEditor.SceneManagement;

namespace RetroRecord.SceneManagement.Editor
{
    internal static class EditorSceneBrowser
    {
        [MenuItem("Scenes/Editor/Viany")]
        private static void OpenEditViany() => OpenScene("Assets/Scenes/Editor/Viany.unity");
        
        [MenuItem("Scenes/Editor/Engine Test")]
        private static void OpenEditorEngineTest() => OpenScene("Assets/Scenes/Editor/Engine Test.unity");

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
    }
}