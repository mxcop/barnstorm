using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class SceneSwitchEditor : Editor
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(3, 3, 3, 3),
            };
        }
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent(Resources.Load("Editor/play-button") as Texture, "Play Persistent"), ToolbarStyles.commandButtonStyle)) {
            PlayPersistentScene();
        }
    }

    static SceneSwitchEditor() {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        EditorApplication.playModeStateChanged += ModeChanged;
    }

    static void ModeChanged(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredEditMode &&
             EditorPrefs.HasKey("PreviousScene"))
        {
            EditorSceneManager.OpenScene(EditorPrefs.GetString("PreviousScene"), OpenSceneMode.Single);
            EditorPrefs.DeleteKey("PreviousScene");
        }
    }

    static void PlayPersistentScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            EditorPrefs.SetString("PreviousScene", EditorSceneManager.GetActiveScene().path);
            EditorSceneManager.OpenScene("Assets/Scenes/Persistent Scene.unity", OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
    }
}
