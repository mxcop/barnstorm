using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerInputActions
{
    public void Initialize();

    // Stick
    public void Input_LStick(InputAction.CallbackContext c);
    public void Input_RStick(InputAction.CallbackContext c);

    // Buttons
    public void Input_BNorth(InputAction.CallbackContext c);
    public void Input_BEast(InputAction.CallbackContext c);
    public void Input_BSouth(InputAction.CallbackContext c);
    public void Input_BWest(InputAction.CallbackContext c);

    // Dpad
    public void Input_DNorth(InputAction.CallbackContext c);
    public void Input_DEast(InputAction.CallbackContext c);
    public void Input_DSouth(InputAction.CallbackContext c);
    public void Input_DWest(InputAction.CallbackContext c);

    // Shoulder
    public void Input_ShoulderR(InputAction.CallbackContext c);
    public void Input_ShoulderL(InputAction.CallbackContext c);

    // Misc
    public void Input_NumberSelect(int num);
}
