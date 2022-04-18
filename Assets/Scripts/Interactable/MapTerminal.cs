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
    public ButtonProfile interactButton { get; set; }

    public void BreakInteraction()
    {
        inUse = false;
    }

    public bool Interact(Player player)
    {
        if (currentController == null)
        {
            if (PersistentPlayerManager.main.TryGetPlayer(player.playerID, out PersistentPlayer _c))
            {
                currentController = _c;
                currentController.SetControlLayer(this, controlLayer);
                inUse = true;
                return true;
            }
        }

        return false;        
    }

    void RelinquishControl()
    {
        if (currentController != null)
        {
            currentController.BreakControlLayer(controlLayer);
            currentController = null;
            Debug.Log("Relinquished control of mapterminal");
        }
    }

    #region IPlayerInputActions
    public void Initialize()
    {
        Debug.Log("Player " + currentController.playerID + " Opened the map terminal");
    }

    public void DeInitialize()
    {

    }

    public void Input_BEast(InputAction.CallbackContext c)
    {
        if (c.performed) RelinquishControl();
    }

    public void Input_BNorth(InputAction.CallbackContext c)
    {
        if (c.performed) RelinquishControl();
    }

    public void Input_BSouth(InputAction.CallbackContext c)
    {
        if (c.performed) RelinquishControl();
    }

    public void Input_BWest(InputAction.CallbackContext c)
    {
        if (c.performed) RelinquishControl();
    }

    public void Input_DEast(InputAction.CallbackContext c) { }

    public void Input_DNorth(InputAction.CallbackContext c) { }

    public void Input_DSouth(InputAction.CallbackContext c) { }

    public void Input_DWest(InputAction.CallbackContext c) { }

    public void Input_LStick(InputAction.CallbackContext c)
    {
        if (c.phase != InputActionPhase.Performed) return;

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
