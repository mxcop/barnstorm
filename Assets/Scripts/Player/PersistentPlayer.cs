using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PersistentPlayer : MonoBehaviour, IPlayerInputActions
{
    public int playerID;
    public IPlayerInputActions currentlyControlling { get; private set; }
    SortedDictionary<byte, IPlayerInputActions> controlLayers = new SortedDictionary<byte, IPlayerInputActions>();

    public ControlsProfile controlsProfile;

    /// <summary>
    /// Sets the specific control layer to the given IPlayerInputActions interface
    /// </summary>
    /// <param name="p"></param>
    /// <param name="layer"></param>
    public void SetControlLayer(IPlayerInputActions p, byte layer)
    {
        controlLayers.Add(layer, p);

        currentlyControlling = GetHighestLayer();
        currentlyControlling.Initialize();
    }

    /// <summary>
    /// Removes control from the IPlayerInputActions on this layer
    /// </summary>
    /// <param name="layer"></param>
    public void BreakControlLayer(byte layer)
    {
        controlLayers.Remove(layer);
    }

    IPlayerInputActions GetHighestLayer()
    {
        return controlLayers[controlLayers.Keys.Max()];
    }

    #region Input rerouting
    public void Input_BEast(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_BEast(c);
    }

    public void Input_BNorth(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_BNorth(c);
    }

    public void Input_BSouth(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_BSouth(c);
    }

    public void Input_BWest(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_BWest(c);
    }

    public void Input_DEast(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_DEast(c);
    }

    public void Input_DNorth(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_DNorth(c);
    }

    public void Input_DSouth(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_DSouth(c);
    }

    public void Input_DWest(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_DWest(c);
    }

    public void Input_LStick(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_LStick(c);
    }

    public void Input_NumberSelect(int num)
    {
        currentlyControlling?.Input_NumberSelect(num);
    }

    public void Input_RStick(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_RStick(c);
    }

    public void Input_ShoulderL(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_ShoulderL(c);
    }

    public void Input_ShoulderR(InputAction.CallbackContext c)
    {
        currentlyControlling?.Input_ShoulderR(c);
    }

    public void Initialize()
    {
        // should not be initialized
    }
    #endregion
}