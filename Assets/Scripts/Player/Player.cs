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

    [HideInInspector] public ButtonPromptType buttonPromptType;
    [HideInInspector] public bool isInteracting;
    [HideInInspector] public bool isInBuilding;

    private Animator anim;
    public PlayerAngle animDir;

    private Vector2 inputMove;
    private float lastInputMag;
    private Vector2 move;
    private int queuedHotbarSelect = -1;

    private Interactable currentInteraction;
    private Inventory currentInventory;

    [SerializeField] GameObject playerToolsPrefab;
    PlayerTools tools;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        tools = Instantiate(playerToolsPrefab).GetComponent<PlayerTools>();
        tools.plr = this;
        tools.playerAnim = anim;
        isInteracting = false;
    }

    private void Update()
    {
        float currentMaxSpeed = maxSpeed * lastInputMag;
        if (inputMove != Vector2.zero && !tools.isUsing)
            move = Vector2.ClampMagnitude(move + inputMove * movementSpeed * Time.deltaTime, currentMaxSpeed);
        else
            move = Vector2.Lerp(move, Vector2.zero, friction * Time.deltaTime);

        lastInputMag = inputMove.magnitude;
        anim.SetFloat("Velocity", lastInputMag);

        // Chooses what direction to face
        // Anti-clockwise
        Vector2 norm = inputMove.normalized;
        if (norm.y <= -0.5f) animDir = PlayerAngle.Down;
        else if (norm.y >= 0.5f) animDir = PlayerAngle.Up;
        else if (norm.x <= -0.5f) animDir = PlayerAngle.Left;
        else if (norm.x >= 0.5f) animDir = PlayerAngle.Right;
        anim.SetInteger("Dir", (int)animDir);

        // Break interaction with interactable when too far away
        if (currentInteraction != null && Vector2.Distance((currentInteraction as MonoBehaviour).transform.position, transform.position) -0.5f > interactCheckRadius) BreakInteraction();

        // Switched to queued hotbar slot
        if(queuedHotbarSelect >= 0)
        {
            HotbarSwitch(queuedHotbarSelect);
            queuedHotbarSelect = -1;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(move);        
    }

    public void Move(CallbackContext input) { inputMove = input.ReadValue<Vector2>(); }

    #region Interact
    public void ProcessInteract(CallbackContext input)
    {
        switch (input.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                if (input.action.WasPerformedThisFrame())
                {
                    BreakInteraction();
                    if(!isInBuilding) tools.PlayerUse();
                }
                break;

            case InputActionPhase.Canceled:
                tools.PlayerStopUse();
                break;
        }
    }
    #endregion

    #region Interactables / Inventories
    /// <summary>
    /// Checks if there is an interactable within the players range.
    /// </summary>
    /// <returns>Whether an interactable was found.</returns>
    private bool CheckForInteractable()
    {
        Collider2D[] colls = new Collider2D[3];
        Physics2D.OverlapCircle(transform.position, interactCheckRadius, cf, colls);

        (Interactable, float) closest = (null,Mathf.Infinity);
        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i] != null)
            {
                Interactable c = colls[i].gameObject.GetComponent<Interactable>();
                if (c != null && !c.inUse)
                {
                    float d = Vector2.Distance((c as MonoBehaviour).transform.position, transform.position);
                    if (closest.Item2 > d) closest = (c, d);
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
            isInteracting = false;
            currentInteraction = null;
            currentInventory = null;
        }
    }
    #endregion

    #region Hotbar Controls
    /// <summary>
    /// Switches current hotbar slot to the given int
    /// </summary>
    /// <param name="slot"></param>
    public void HotbarSwitch(int slot)
    {
        if (!tools.isUsing) SelectSlot(slot);
        else queuedHotbarSelect = slot;
    }

    public void HotbarSwitchR(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            HotbarSwitch((int)Mathf.Repeat(selected + 1, container.size));
        }
    }
    public void HotbarSwitchL(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            HotbarSwitch((int)Mathf.Repeat(selected-1, container.size));
        }
    }
    #endregion

    #region Hotbar item manipulation
    /// <summary>
    /// Swaps the currently held item with the item in single-inventory 
    /// In case of an inventory with the same item-type, first stack every item in the players inventory
    /// </summary>
    public void InventorySwap(CallbackContext input)
    {
        if (input.phase != InputActionPhase.Performed) return;

        if (isInteracting == false && CheckForInteractable())
        {
            currentInteraction.Interact();
            isInteracting = true;
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

        if (currentInteraction == null && isInteracting == false)
        {
            if (CheckForInteractable())
            {
                currentInteraction.Interact();
                isInteracting = true;
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
        if (input.phase != InputActionPhase.Performed) return;

        if (container.PullItem(selected, out var item))
        {
            DroppedItem.DropUp(item.item, item.num, transform.position);
        }
    }
    #endregion

    //are called by the animator attached to this player, do not call these via code
    #region Animation events
    public void Anim_ToolAction() => tools.ToolAction();

    public void Anim_ToolStart() => tools.Anim_ToolStart();
    #endregion
}

public enum ButtonPromptType
{
    Unknown, Playstation, Xbox, PC
}