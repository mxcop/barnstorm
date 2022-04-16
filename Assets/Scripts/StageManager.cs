using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public static StageManager current;
    public LevelSettings currentSettings { get; private set; }

    public UnityEvent OnStartGameplay;

    private void Awake()
    {
        current = this;
    }

    public void Setup(LevelSettings settings)
    {
        Debug.Log("Loaded level: " + settings.l_displayName);
        currentSettings = settings;
    }

    public void StartGameplay()
    {
        OnStartGameplay?.Invoke();
    }
}
