using System;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    public bool isPassthrough = false;
    public Surroundings surroundings;
    // TODO : Add level settings object as public

    public void StartLevel()
    {
        throw new NotImplementedException("Start level function hasn't been implemented yet!");
    }

    [Serializable]
    public struct Surroundings
    {
        public MapLevel top, right, bottom, left;
    }
}