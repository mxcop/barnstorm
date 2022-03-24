using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput input)
    {
        Player player = input.GetComponent<Player>();
        if (input.devices.Count > 0)
        {
            player.buttonPromptType = ButtonPromptParse(input.devices[0].displayName);
        }

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
