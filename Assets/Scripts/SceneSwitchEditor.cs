using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SceneSwitchEditor : Editor
{
    static SceneSwitchEditor() {
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

    [MenuItem("Scenes Tools/Open Persistent Scene")]
    static void PersistentScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            EditorSceneManager.OpenScene("Assets/Scenes/Persistent Scene.unity", OpenSceneMode.Single);
        }
    }

    [MenuItem("Scenes Tools/Play Persistent Scene")]
    static void PlayPersistentScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
            PlayerPrefs.SetString("PreviousScene", EditorSceneManager.GetActiveScene().path);
            EditorSceneManager.OpenScene("Assets/Scenes/Persistent Scene.unity", OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
    }
}
