using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PersistentPlayerManager : MonoBehaviour
{
    public static PersistentPlayerManager main;
    Dictionary<int, PersistentPlayer> playerList = new Dictionary<int, PersistentPlayer>();
    public int PlayerCount => playerList.Count;

    Scene persistentScene;

    [SerializeField] DeviceProfileLibElement[] _controlProfiles;
    Dictionary<DeviceProfile, DeviceProfileSprites> controlProfiles = new Dictionary<DeviceProfile, DeviceProfileSprites>();

    private void Awake()
    {
        main = this;
        SceneManager.sceneUnloaded += (s) => ClearAllControlLayers();

        persistentScene = SceneManager.GetSceneByName("Persistent Scene");

        foreach(DeviceProfileLibElement d in _controlProfiles)
        {
            controlProfiles.Add(d.profile, d.sprites);
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PersistentPlayer p = input.GetComponent<PersistentPlayer>();
        p.playerID = GetLowestAvailablePlayerID();
        p.controlsProfile = GetProfile(input.devices[0].layout);
        playerList.Add(p.playerID, p);

        SceneManager.MoveGameObjectToScene(p.gameObject, persistentScene);
    }

    public void OnPlayerLeft(PlayerInput input)
    {
        PersistentPlayer p = input.GetComponent<PersistentPlayer>();
        playerList.Remove(p.playerID);
    }

    public void ClearAllControlLayers()
    {
        foreach(PersistentPlayer p in playerList.Values)
        {
            p.ClearControlLayers();
        }
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

    #region Button Profiles

    private DeviceProfileSprites GetProfile(string deviceName)
    {
        DeviceProfileSprites output;

        if (deviceName.StartsWith("Xbox")) output = controlProfiles[DeviceProfile.Xbox];
        else if (deviceName.StartsWith("DualShock")) output = controlProfiles[DeviceProfile.Playstation];
        else if (deviceName.StartsWith("Keyboard")) output = controlProfiles[DeviceProfile.Keyboard];
        else output = controlProfiles[DeviceProfile.Default];

        Debug.Log($"Player connected: {deviceName}, uses {output.ToString()} controls profile");

        return output;
    }

    enum DeviceProfile
    {
        Default,
        Xbox,
        Playstation,
        Keyboard
    }

    [System.Serializable]
    struct DeviceProfileLibElement
    {
        public DeviceProfile profile;
        public DeviceProfileSprites sprites;
    }
    #endregion
}
