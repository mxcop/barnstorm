using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    [Header("Load settings")]
    public string l_sceneName;

    //[Header("Level goals")]

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
