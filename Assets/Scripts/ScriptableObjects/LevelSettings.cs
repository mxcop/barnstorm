  using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    [Header("Level select settings")]
    public string l_sceneName;
    public string l_displayName;
    public string l_description;

    [Header("Level goals")]
    public Objective[] l_objectives;

    public enum GoalType
    {
        Item,
        FarmGoal,
        Other
    }

    [System.Serializable]
    public class Objective
    {
        [Header("!! ALWAYS PUT THE SECONDARY OBJECTIVES AT THE BOTTOM OF THE LIST !!")]
        [Tooltip("An icon representing this goal (A crop icon or wave depending on the goal)")] public Sprite icon;
        [Tooltip("The goal type this objective is")] public GoalType goal;
        [Tooltip("The item that this goal needs (is only needed with a Item Goal)")] public Item item;
        [Tooltip("The quantity of the objective, e.g. number of crops to farm")] public int quantity = 999;
        [Tooltip("Whether this objective is optional for completing the level")] public bool isSecondary = false;
    }

    [Header("Level animals")]
    public Animal[] l_animals;

    [System.Serializable]
    public class Animal
    {
        [Tooltip("An icon representing this animal")] public Sprite icon;
    }

    //[Header("World map unlock requirements")]

    //[Header("Delivery truck settings")]

    [Header("Enemy waves")]
    public List<Wave> waves = new List<Wave>();

    [System.Serializable]
    public class Enemy
    {
        [Tooltip("The Prefab of this enemy instance")] public GameObject prefab;
        [Tooltip("The Chance this enemy will be spawned, higher number is higher chance")] public float weight;
        [HideInInspector] public Vector2 dropRange;
    }

    [System.Serializable]
    public class Wave
    {
        [Header("Enemy Types")]
        [Tooltip("All the enemies that can spawn in this wave")] public List<Enemy> enemies;

        [Header("Group Settings")]
        [Tooltip("The amount of times a group will spawn inside a wave")] public int groups;
        [Tooltip("The radius all the enemies in that group will spawn inside of")] public Vector2 groupSize;
        [Tooltip("The minimum (x) and maximum (y) amount of time between every group spawns")] public Vector2 groupDelay;
        [Tooltip("The radius all the enemies in that group will spawn inside of")] public float groupRadius;

        [Header("Extra's")]
        [Tooltip("The amount of time before the next wave starts")] public float waveDelay;
    }
}
