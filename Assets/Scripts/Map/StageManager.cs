using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public static StageManager current;
    public LevelSettings currentSettings { get; private set; }

    public UnityEvent<LevelSettings> OnStartGameplay;

    public bool finishedObjecive;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        StartGameplay();
    }

    /// <summary>
    /// Called by the LevelLoader after it has finished loading the scene
    /// </summary>
    /// <param name="settings"></param>
    public void Setup(LevelSettings settings)
    {
        Debug.Log("Loaded level: " + settings.l_displayName);
        currentSettings = settings;
    }

    public void StartGameplay()
    {
        Debug.Log("Started gameplay of level " + currentSettings.l_displayName);
        OnStartGameplay?.Invoke(currentSettings);
    }

    public void UpdateObjective(ContainedItem<Item> item)
    {
        for (int i = 0; i < currentSettings.l_objectives.Length; i++)
        {
            if(currentSettings.l_objectives[i].goal == LevelSettings.GoalType.Item &&
               currentSettings.l_objectives[i].item == item.item &&
               currentSettings.l_objectives[i].quantity <= item.num)
            {
                finishedObjecive = true;
                // Idk what to do next
            }
        }
        
    }
}
