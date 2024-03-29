using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader main;

    [SerializeField] string levelSelectSceneName;
    public LevelSettings currentLevel { get; private set; }

    private void Start()
    {
        main = this;
        LoadLevelSelect();
    }

    AsyncOperation LoadNewActiveScene(string n)
    {

        AsyncOperation a = SceneManager.LoadSceneAsync(n, LoadSceneMode.Additive);
        a.completed += (s) =>
        {
            Scene scene = SceneManager.GetSceneByName(n);
            SceneManager.SetActiveScene(scene);
        };

        return a;
    }

    public void EnterLevel(LevelSettings settings)
    {
        currentLevel = settings;
        Debug.Log("Loading level: " + currentLevel.l_sceneName);

        if(SceneManager.GetSceneByName(levelSelectSceneName) != null) SceneManager.UnloadSceneAsync(levelSelectSceneName);

        LoadNewActiveScene(currentLevel.l_sceneName).completed += (s) => StageManager.current.Setup(settings);

        SFXManager.PlayMusic(settings.musicName);

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
        LoadNewActiveScene(levelSelectSceneName);
        SFXManager.PlayMusic("mus_lobby");
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level: " + currentLevel.l_sceneName);
        SceneManager.UnloadSceneAsync(currentLevel.l_sceneName).completed += (a) => EnterLevel(currentLevel);
    }
}
