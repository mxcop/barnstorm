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
    [SerializeField] float interactCheckRadius;
    [SerializeField] LayerMask cf;
    [Space]
    [SerializeField] TransformArray[] tillPoints;

    [HideInInspector] public ButtonPromptType buttonPromptType;

    Animator anim;

    Vector2 input_move;
    float lastInputMag;
    Vector2 move;

    int animDir = 2;
    int tillDir = 2;

    bool usingTool;
    Interactable currentInteraction;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        PlayerInput i;
    }

    private void Update()
    {
        float currentMaxSpeed = maxSpeed * lastInputMag;
        if (input_move != Vector2.zero && !usingTool)
        {
            move = Vector2.ClampMagnitude(move + input_move * movementSpeed * Time.deltaTime, currentMaxSpeed);
        }
        else
        {
            move = Vector2.Lerp(move, Vector2.zero, friction * Time.deltaTime);
        }

        lastInputMag = input_move.magnitude;
        anim.SetFloat("Velocity", lastInputMag);

        //chooses what direction to face
        //anti-clockwise

        Vector2 norm = input_move.normalized;
        if (norm.y <= -0.5f) animDir = 2;
        else if (norm.y >= 0.5f) animDir = 0;
        else if (norm.x <= -0.5f) animDir = 1;
        else if (norm.x >= 0.5f) animDir = 3;
        anim.SetInteger("Dir", animDir);

        //break interaction with interactable when too far away
        if (currentInteraction != null && Vector2.Distance((currentInteraction as MonoBehaviour).transform.position, transform.position) > interactCheckRadius) BreakInteraction();
    }

    private void FixedUpdate()
    {
        transform.Translate(move);
    }

    public void Move(CallbackContext input) { input_move = input.ReadValue<Vector2>(); }

    public void ProcessInteract(CallbackContext input)
    {
        switch (input.phase)
        {
            case InputActionPhase.Started:
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
        BreakInteraction();
        anim.SetBool("Tilling", true);
        tillDir = animDir;
    }

    /// <summary>
    /// Called when interact button stops being held
    /// </summary>
    void InteractEnd()
    {
        anim.SetBool("Tilling", false);
    }

    bool CheckForInteractable()
    {
        Collider2D[] colls = new Collider2D[2];
        Physics2D.OverlapCircleNonAlloc(transform.position, interactCheckRadius, results: colls, cf);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != null)
            {
                Interactable c = colls[i].gameObject.GetComponent<Interactable>();
                if (c != null && !c.inUse)
                {
                    currentInteraction = c;
                    return true;
                }
            }
        }
        return false;
    }

    void BreakInteraction()
    {
        if (currentInteraction != null)
        {
            currentInteraction.BreakInteraction();
            currentInteraction = null;
        }
    }

    #region Inventory Controls
    /// <summary>
    /// Switches current hotbar slot to the given int
    /// </summary>
    /// <param name="slot"></param>
    public void HotbarSwitch(int slot)
    {
    }

    public void HotbarSwitchR(CallbackContext input)
    {
        //HotbarSwitch(Mathf.Clamp(inventoryMaxSize + 1, 0, inventoryMaxSize)); 
    }
    public void HotbarSwitchL(CallbackContext input)
    {
        //HotbarSwitch(Mathf.Clamp(inventoryMaxSize - 1, 0, inventoryMaxSize)); 
    }

    /// <summary>
    /// Swaps the currently held item with the item in single-inventory 
    /// In case of an inventory with the same item-type, first stack every item in the players inventory
    /// </summary>
    public void InventorySwap(CallbackContext input)
    {
        if (currentInteraction == null)
        {
            if (CheckForInteractable())
            {
                currentInteraction.Interact();
            }
        }
        else
        {
            currentInteraction.SwapAction();
        }
    }

    /// <summary>
    /// Gives half of currently held item to opened inventories
    /// </summary>
    public void InventorySplit(CallbackContext input)
    {
        if (currentInteraction == null)
        {
            if (CheckForInteractable())
            {
                currentInteraction.Interact();
            }
        }
        else
        {
            currentInteraction.SplitAction();
        }
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

    public void Anim_TillEvent()
    {
        usingTool = false;

        TransformArray tps = tillPoints[tillDir];

        for (int i = 0; i < tps.array.Length; i++)
        {
            Vector2 pos = tps[i].position;
            Till(pos);
        }

    }

    void Till(Vector2 pos) => Till(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));        
    void Till(int x, int y)
    {
        if (CropManager.current.TileIsTillable(x, y))
        {
            CropManager.current.PlaceTile(TileType.Tilled, x, y);
        }
    }


    public void Anim_TillStart() => usingTool = true;
}

public enum ButtonPromptType
{
    Unknown, Playstation, Xbox, PC
}

[System.Serializable]
public struct TransformArray
{
    public Transform[] array;
    public Transform this[int i]
    {
        get { return array[i]; }
        set { array[i] = value; }
    }
}
