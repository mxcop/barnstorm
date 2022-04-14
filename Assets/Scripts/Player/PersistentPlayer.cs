using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersistentPlayer : MonoBehaviour, IPlayerInputActions
{
    public int playerID;
    public IPlayerInputActions currentlyControlling { get; private set; }

    public void SetCurrentlyControlling(IPlayerInputActions p)
    {
        currentlyControlling = p;
        p.Initialize();
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
