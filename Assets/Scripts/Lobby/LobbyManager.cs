using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public static List<Player> players;
    public static bool hasStarted;

    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private Material[] playerMats;

    public delegate void OnGameStartEvent();
    public static event OnGameStartEvent OnGameStart;

    private void Awake()
    {
        OnGameStart += () =>
        {
            waitingPanel.SetActive(false);
        };
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        Player player = input.GetComponent<Player>();
        if (input.devices.Count > 0)
        {
            player.profile = GetProfile(input.devices[0].displayName);
        }

        if (players == null) players = new List<Player>();
        players.Add(player);
        player.GetComponent<SpriteRenderer>().material = playerMats[players.Count - 1];
        Debug.Log("Player Joined");

        // TODO : Handle the joining with the UI.
    }

    public void OnPlayerLeft(PlayerInput input)
    {
        // TODO : Handle the leaving with the UI.
    }

    /// <summary>
    /// Check if all players are ready.
    /// </summary>
    public static void CheckForReady()
    {
        bool startGame = true;
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isReady)
                startGame = false;
        }

        if (startGame == true)
        {
            //players.Clear();
            hasStarted = true;
            OnGameStart.Invoke();
        }
    }

    /// <summary>
    /// Get the correct profile for this device.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
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
