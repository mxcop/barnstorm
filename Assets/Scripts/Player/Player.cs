using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : PlayerInventory
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float friction;
    [Space]
    [Header("Interactions")]
    [SerializeField] float interactCheckRadius;
    [SerializeField] ContactFilter2D cf;
    [Space]
    [SerializeField] TransformArray[] tillPoints;

    [HideInInspector] public ButtonPromptType buttonPromptType;

    private Animator anim;

    private Vector2 inputMove;
    private float lastInputMag;
    private Vector2 move;

    int animDir = 2;
    int tillDir = 2;

    private bool usingTool;
    private Interactable currentInteraction;
    private Inventory currentInventory;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float currentMaxSpeed = maxSpeed * lastInputMag;
        if (inputMove != Vector2.zero && !usingTool)
            move = Vector2.ClampMagnitude(move + inputMove * movementSpeed * Time.deltaTime, currentMaxSpeed);
        else
            move = Vector2.Lerp(move, Vector2.zero, friction * Time.deltaTime);

        lastInputMag = inputMove.magnitude;
        anim.SetFloat("Velocity", lastInputMag);

        // Chooses what direction to face
        // Anti-clockwise
        Vector2 norm = inputMove.normalized;
        if (norm.y <= -0.5f) animDir = 2;
        else if (norm.y >= 0.5f) animDir = 0;
        else if (norm.x <= -0.5f) animDir = 1;
        else if (norm.x >= 0.5f) animDir = 3;
        anim.SetInteger("Dir", animDir);

        // Break interaction with interactable when too far away
        if (currentInteraction != null && Vector2.Distance((currentInteraction as MonoBehaviour).transform.position, transform.position) -0.5f > interactCheckRadius) BreakInteraction();
    }

    private void FixedUpdate()
    {
        transform.Translate(move);        
    }

    public void Move(CallbackContext input) { inputMove = input.ReadValue<Vector2>(); }

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

    /// <summary>
    /// Checks if there is an interactable within the players range.
    /// </summary>
    /// <returns>Whether an interactable was found.</returns>
    private bool CheckForInteractable()
    {
        Collider2D[] colls = new Collider2D[3];
        Physics2D.OverlapCircle(transform.position, interactCheckRadius, cf, colls);

        (Interactable, float) closest = (null,0);
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != null)
            {
                Interactable c = colls[i].gameObject.GetComponent<Interactable>();
                if (c != null && !c.inUse)
                {
                    float d = Vector2.Distance((c as MonoBehaviour).transform.position, transform.position);
                    if (closest.Item2 < d) closest = (c, d);
                }
            }
        }

        if (closest.Item1 != null)
        {
            currentInteraction = closest.Item1;
            return true;
        }
        return false;
    }

    void BreakInteraction()
    {
        if (currentInteraction != null)
        {
            currentInteraction.BreakInteraction();
            currentInteraction = null;
            currentInventory = null;
        }
    }

    #region Inventory Controls
    /// <summary>
    /// Switches current hotbar slot to the given int
    /// </summary>
    /// <param name="slot"></param>
    public void HotbarSwitch(int slot)
    {
        SelectSlot(slot);
    }

    public void HotbarSwitchR(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            HotbarSwitch(Mathf.Clamp(selected + 1, 0, container.size - 1));
        }
    }
    public void HotbarSwitchL(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            HotbarSwitch(Mathf.Clamp(selected - 1, 0, container.size - 1));
        }
    }

    /// <summary>
    /// Swaps the currently held item with the item in single-inventory 
    /// In case of an inventory with the same item-type, first stack every item in the players inventory
    /// </summary>
    public void InventorySwap(CallbackContext input)
    {
        if (input.phase != InputActionPhase.Performed) return;

        if (CheckForInteractable())
        {
            currentInteraction.Interact();
            currentInventory = currentInteraction as Inventory;
        }
        else if (currentInventory != null) currentInventory.QuickSwap(this, selected);
    }

    /// <summary>
    /// Gives half of currently held item to opened inventories
    /// </summary>
    public void InventorySplit(CallbackContext input)
    {
        if (input.phase != InputActionPhase.Performed) return;

        if (currentInteraction == null)
        {
            if (CheckForInteractable())
            {
                currentInteraction.Interact();
                currentInventory = currentInteraction as Inventory;
            }
        }
        else if (currentInventory != null)
        {
            currentInventory.QuickSplit(this, selected);
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
            CropData? _crop = CropManager.current.Till(pos);
            if(_crop != null)
            {
                CropData crop = (CropData)_crop;
                container.PushItem(crop.item, crop.amount);
            }
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
