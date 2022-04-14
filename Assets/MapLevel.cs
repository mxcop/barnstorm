using System;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    public LevelSettings levelSettings;
    public bool isPassthrough = false;
    public Surroundings surroundings;
    // TODO : Add level settings object as public

    public void StartLevel()
    {
        LevelLoader.main.EnterLevel(levelSettings);
    }

    [Serializable]
    public struct Surroundings
    {
        public MapLevel top, right, bottom, left;
    }
}