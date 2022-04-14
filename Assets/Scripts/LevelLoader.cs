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

        SceneManager.UnloadSceneAsync(levelSelectSceneName);
        SceneManager.LoadSceneAsync(currentLevel.l_sceneName);
    }

    public void ExitLevel()
    {
        SceneManager.UnloadSceneAsync(currentLevel.l_sceneName);

        currentLevel = null;
        LoadLevelSelect();
    }

    void LoadLevelSelect()
    {
        SceneManager.LoadSceneAsync(levelSelectSceneName, LoadSceneMode.Additive);
    }

    public void RestartLevel()
    {
        SceneManager.UnloadSceneAsync(currentLevel.l_sceneName).completed += (a) => SceneManager.LoadSceneAsync(currentLevel.l_sceneName);
    }
}
