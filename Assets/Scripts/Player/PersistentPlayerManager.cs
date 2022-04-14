using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersistentPlayerManager : MonoBehaviour
{
    public static PersistentPlayerManager main;
    Dictionary<int, PersistentPlayer> playerList = new Dictionary<int, PersistentPlayer>();
    public int PlayerCount => playerList.Count;

    private void Awake()
    {
        main = this;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PersistentPlayer p = input.GetComponent<PersistentPlayer>();
        p.playerID = GetLowestAvailablePlayerID();
        playerList.Add(p.playerID, p);
    }

    public void OnPlayerLeft(PlayerInput input)
    {
        PersistentPlayer p = input.GetComponent<PersistentPlayer>();
        playerList.Remove(p.playerID);
    }

    int GetLowestAvailablePlayerID()
    {
        for(int i = 0; i< 4; i++)
        {
            if (!playerList.ContainsKey(i)) return i;
            else continue;
        }

        Debug.Log("Something has gone horribly wrong");
        return -1;
    }

    public bool TryGetPlayer(int id, out PersistentPlayer player)
    {
        if (playerList.ContainsKey(id))
        {
            player = playerList[id];
            return true;
        }
        else
        {
            player = null;
            return false;
        }
    }
}
