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
        p.controlsProfile = GetProfile(input.devices[0].displayName);
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

    private ControlsProfile GetProfile(string deviceName)
    {
        string profileName;

        switch (deviceName)
        {
            case "Keyboard": profileName = "Keyboard Profile"; break;
            case "Xbox Controller": profileName = "Xbox Profile"; break;
            case "Playstation Controller": profileName = "Playstation Profile"; break;
            default:
                Debug.LogError($"Unknown device detected ({ deviceName })");
                profileName = "Keyboard Profile";
                break;
        }

        // Fetch the controller profile from the resources folder.
        return Resources.Load<ControlsProfile>("Controller Profiles/" + profileName);
    }
}
