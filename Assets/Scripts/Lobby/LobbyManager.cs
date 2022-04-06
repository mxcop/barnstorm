using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public static List<Player> players;

    [SerializeField] private Material[] playerMats;

    public void OnPlayerJoined(PlayerInput input)
    {
        Player player = input.GetComponent<Player>();
        if (input.devices.Count > 0)
        {
            player.buttonPromptType = ButtonPromptParse(input.devices[0].displayName);
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
    /// Parses a device name to a button prompt type.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    private ButtonPromptType ButtonPromptParse(string deviceName)
    {
        switch (deviceName)
        {
            case "Keyboard":
                return ButtonPromptType.PC;
            case "Xbox Controller":
                return ButtonPromptType.Xbox;
            case "Playstation Controller":
                return ButtonPromptType.Playstation;
            default:
                Debug.LogError($"Unknown device detected ({ deviceName })");
                return ButtonPromptType.Unknown;
        }
    }
}
