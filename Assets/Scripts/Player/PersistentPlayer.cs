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

    public DeviceProfileSprites controlsProfile;
    bool dontSendInput;

    /// <summary>
    /// Sets the specific control layer to the given IPlayerInputActions interface
    /// </summary>
    /// <param name="p"></param>
    /// <param name="layer"></param>
    public void SetControlLayer(IPlayerInputActions p, byte layer)
    {
        if (!controlLayers.ContainsKey(layer)) controlLayers.Add(layer, p);

        dontSendInput = true;
        InitializeTopLevel();
    }

    /// <summary>
    /// Removes control from the IPlayerInputActions on this layer
    /// </summary>
    /// <param name="layer"></param>
    public void BreakControlLayer(byte layer)
    {
        if (controlLayers.ContainsKey(layer))
        {
            controlLayers[layer].DeInitialize();
            controlLayers.Remove(layer);

            InitializeTopLevel();
        }
    }

    public void ClearControlLayers()
    {
        controlLayers.Clear();
    }

    void InitializeTopLevel()
    {
        currentlyControlling = GetHighestLayer();
        currentlyControlling.Initialize();

        foreach (IPlayerInputActions i in controlLayers.Values)
        {
            if (i != currentlyControlling) i.DeInitialize();
        }
    }

    IPlayerInputActions GetHighestLayer()
    {
        return controlLayers[controlLayers.Keys.LastOrDefault()];
    }

    #region Input rerouting
    public void Input_BEast(InputAction.CallbackContext c)
    {        
        if(!dontSendInput) currentlyControlling?.Input_BEast(c);
        dontSendInput = false;
    }

    public void Input_BNorth(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_BNorth(c);
        dontSendInput = false;
    }

    public void Input_BSouth(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_BSouth(c);
        dontSendInput = false;
    }

    public void Input_BWest(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_BWest(c);
        dontSendInput = false;
    }

    public void Input_LStick(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_LStick(c);
        dontSendInput = false;
    }

    public void Input_NumberSelect(int num)
    {
        if (!dontSendInput) currentlyControlling?.Input_NumberSelect(num);
        dontSendInput = false;
    }

    public void Input_RStick(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_RStick(c);
        dontSendInput = false;
    }

    public void Input_ShoulderL(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_ShoulderL(c);
        dontSendInput = false;
    }

    public void Input_ShoulderR(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_ShoulderR(c);
        dontSendInput = false;
    }

    public void Input_TriggerL(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_TriggerL(c);
        dontSendInput = false;
    }

    public void Input_TriggerR(InputAction.CallbackContext c)
    {
        if (!dontSendInput) currentlyControlling?.Input_TriggerR(c);
        dontSendInput = false;
    }

    public void Initialize()
    {
        // should not be initialized
    }

    public void DeInitialize()
    {

    }
    #endregion
}