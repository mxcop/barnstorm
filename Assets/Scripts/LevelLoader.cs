using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader main;

    [SerializeField] string levelSelectSceneName;
    public LevelSettings currentLevel { get; private set; }

    private void Awake()
    {
        main = this;
        LoadLevelSelect();
    }

    public void EnterLevel(LevelSettings settings)
    {
        currentLevel = settings;
        Debug.Log("Loading level: " + currentLevel.l_sceneName);

        SceneManager.UnloadSceneAsync(levelSelectSceneName);
        SceneManager.LoadSceneAsync(currentLevel.l_sceneName);
    }

    public void ExitLevel()
    {
        Debug.Log("Unloading level: " + currentLevel.l_sceneName);
        SceneManager.UnloadSceneAsync(currentLevel.l_sceneName);

        currentLevel = null;
        LoadLevelSelect();
    }

    void LoadLevelSelect()
    {
        Debug.Log("Loading level select scene");
        SceneManager.LoadSceneAsync(levelSelectSceneName, LoadSceneMode.Additive);
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level: " + currentLevel.l_sceneName);
        SceneManager.UnloadSceneAsync(currentLevel.l_sceneName).completed += (a) => SceneManager.LoadSceneAsync(currentLevel.l_sceneName);
    }
}
