using UnityEditor;
using UnityEngine;

public class LevelLoader : EditorWindow
{
    private Maps selectedLevel;

    [MenuItem("Window/LevelLoader")]
    public static void OpenWindow()
    {
        LevelLoader window = GetWindow<LevelLoader>();
        window.titleContent = new GUIContent("Level Loader");

    }


    private void OnGUI()
    {
        selectedLevel = (Maps)EditorGUILayout.ObjectField(selectedLevel, typeof(Maps));
        if (GUILayout.Button("LoadLevel"))
        {
            FindObjectOfType<GameManager>().LoadLevel(selectedLevel);
        }
    }
}
