using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapTerminal : MonoBehaviour, Interactable, IPlayerInputActions
{
    [SerializeField] byte controlLayer;
    [SerializeField] MapNavigator nav;
    PersistentPlayer currentController;

    public bool inUse { get; set; }

    public void BreakInteraction()
    {
        inUse = false;
    }

    public void Interact(int playerID)
    {
        inUse = true;
        if(PersistentPlayerManager.main.TryGetPlayer(playerID, out currentController))
        {
            currentController.SetControlLayer(this, controlLayer);
        }
        
    }

    #region IPlayerInputActions
    public void Initialize()
    {
        Debug.Log("Player " + currentController.playerID + " Opened the map terminal");
    }

    public void Input_BEast(InputAction.CallbackContext c) { }

    public void Input_BNorth(InputAction.CallbackContext c) { }

    public void Input_BSouth(InputAction.CallbackContext c) { }

    public void Input_BWest(InputAction.CallbackContext c)
    {
        currentController.BreakControlLayer(controlLayer);
        currentController = null;
    }

    public void Input_DEast(InputAction.CallbackContext c) { }

    public void Input_DNorth(InputAction.CallbackContext c) { }

    public void Input_DSouth(InputAction.CallbackContext c) { }

    public void Input_DWest(InputAction.CallbackContext c) { }

    public void Input_LStick(InputAction.CallbackContext c)
    {
        Vector2 norm = c.ReadValue<Vector2>().normalized;
        Direction? d = null;

        if (norm.y <= -0.5f) d = Direction.Down;
        else if (norm.y >= 0.5f) d = Direction.Up;
        else if (norm.x <= -0.5f) d = Direction.Left;
        else if (norm.x >= 0.5f) d = Direction.Right;

        if (d != null) nav.Navigate((Direction)d);
    }

    public void Input_NumberSelect(int num)
    {
    }

    public void Input_RStick(InputAction.CallbackContext c)
    {
    }

    public void Input_ShoulderL(InputAction.CallbackContext c)
    {
    }

    public void Input_ShoulderR(InputAction.CallbackContext c)
    {
    }
    #endregion
}