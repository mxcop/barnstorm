using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerInputActions
{
    public void Initialize();
    public void DeInitialize();

    // Stick
    public void Input_LStick(InputAction.CallbackContext c);
    public void Input_RStick(InputAction.CallbackContext c);

    // Buttons
    public void Input_BNorth(InputAction.CallbackContext c);
    public void Input_BEast(InputAction.CallbackContext c);
    public void Input_BSouth(InputAction.CallbackContext c);
    public void Input_BWest(InputAction.CallbackContext c);

    // Shoulder
    public void Input_ShoulderR(InputAction.CallbackContext c);
    public void Input_ShoulderL(InputAction.CallbackContext c);

    // Trigger
    public void Input_TriggerR(InputAction.CallbackContext c);
    public void Input_TriggerL(InputAction.CallbackContext c);

    // Misc
    public void Input_NumberSelect(int num);
}
