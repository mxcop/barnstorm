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
        Debug.Log("Mode Change:" + playModeState);
        Debug.Log(PlayerPrefs.GetString("PreviousScene"));
        if (playModeState == PlayModeStateChange.EnteredEditMode &&
             PlayerPrefs.HasKey("PreviousScene"))
        {
            Debug.Log("Exiting");
            EditorSceneManager.OpenScene(PlayerPrefs.GetString("PreviousScene"), OpenSceneMode.Single);
            PlayerPrefs.DeleteKey("PreviousScene");
        }
    }

    static void PlayPersistentScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            PlayerPrefs.SetString("PreviousScene", EditorSceneManager.GetActiveScene().path);
            EditorSceneManager.OpenScene("Assets/Scenes/Persistent Scene.unity", OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
    }
}
