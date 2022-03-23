using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float friction;
    [Space]
    public int inventoryMaxSize;

    Vector2 input_move;
    float lastInputMag;
    Vector2 move;

    int currentSlot;
    ButtonPromptType buttonPromptType;    

    private void Update()
    {
        float currentMaxSpeed = maxSpeed * lastInputMag;
        if (input_move != Vector2.zero)
        {
            move = Vector2.ClampMagnitude(move + input_move * movementSpeed * Time.deltaTime, currentMaxSpeed);
        }
        else
        {
            move = Vector2.Lerp(move, Vector2.zero, friction * Time.deltaTime);
        }

        lastInputMag = input_move.magnitude;
    }
    public void OnJoin()
    {

    }

    public void Move(CallbackContext input) { input_move = input.ReadValue<Vector2>(); }

    public void ProcessInteract(CallbackContext input)
    {
        switch (input.phase)
        {
            case InputActionPhase.Performed:
                if (input.action.WasPerformedThisFrame()) InteractStart();
                break;

            case InputActionPhase.Canceled:
                InteractEnd();
                break;
        }
    }

    /// <summary>
    /// Press and hold to use tools and open inventories
    /// </summary>
    void InteractStart()
    {

    }

    /// <summary>
    /// Called when interact button stops being held
    /// </summary>
    void InteractEnd()
    {

    }    

    #region Inventory Controls
    /// <summary>
    /// Switches current hotbar slot to the given int
    /// </summary>
    /// <param name="slot"></param>
    public void HotbarSwitch(int slot)
    {
        currentSlot = slot;
    }
    public void HotbarSwitchR(CallbackContext input) => HotbarSwitch(Mathf.Clamp(inventoryMaxSize + 1, 0, inventoryMaxSize));
    public void HotbarSwitchL(CallbackContext input) => HotbarSwitch(Mathf.Clamp(inventoryMaxSize - 1, 0, inventoryMaxSize));

    /// <summary>
    /// Swaps the currently held item with the item in single-inventory 
    /// In case of an inventory with the same item-type, first stack every item in the players inventory
    /// </summary>
    public void InventorySwap(CallbackContext input)
    {
        // :)
    }

    /// <summary>
    /// Gives half of currently held item to opened inventories
    /// </summary>
    public void InventorySplit(CallbackContext input)
    {
        // :(
    }

    /// <summary>
    /// Called every frame when held down
    /// </summary>
    /// <param name="input"></param>
    public void DropItem(CallbackContext input)
    {
        // :|
    }
    #endregion

    private void FixedUpdate()
    {
        transform.Translate(move);
    }
}

public enum ButtonPromptType
{
    Playstation, Xbox, PC
}
