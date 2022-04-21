using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Cinemachine;

public class Player : PlayerInventory, IPlayerInputActions
{
    bool isInitialized;

    bool isBeingControlled;
    public int playerID;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float friction;
    [SerializeField] float outOfBoundsRadius;
    [Space]
    [Header("Interactions")]
    [SerializeField] float interactCheckRadius;
    [SerializeField] ContactFilter2D cf;

    [HideInInspector] public DeviceProfileSprites profile;
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

    // do not add base.Awake to this, this is to stop the base awake code from being ran immediately
    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        transform.localScale = Vector2.zero;
    }

    public void Initialize()
    {
        isBeingControlled = true;

        if (!isInitialized)
        {
            isInitialized = true;
            gameObject.LeanScale(Vector3.one, 0.6f).setEaseOutBounce();

            base.Awake();
            tools = Instantiate(playerToolsPrefab).GetComponent<PlayerTools>();
            tools.plr = this;
            tools.playerAnim = anim;
            isInteracting = false;

            FindObjectOfType<CinemachineTargetGroup>()?.AddMember(gameObject.transform, 1f, 1.25f);
        }
    }

    public void DeInitialize()
    {
        inputMove = Vector2.zero;
    }

    private void Update()
    {
        //go no further if not being controlled, but try to get the player
        if (!isBeingControlled)
        {
            if (PersistentPlayerManager.main.TryGetPlayer(playerID, out PersistentPlayer p))
            {
                p.SetControlLayer(this, 0);
                profile = p.controlsProfile;
            }
            else return;
        }

        float currentMaxSpeed = maxSpeed * lastInputMag;
        if (inputMove != Vector2.zero && !tools.isUsingTool)
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
        if (queuedHotbarSelect >= 0)
        {
            HotbarSwitch(queuedHotbarSelect);
            queuedHotbarSelect = -1;
        }

        // Prevent player out of bounds
        transform.localPosition = Vector2.ClampMagnitude(transform.localPosition, outOfBoundsRadius);
    }

    private void FixedUpdate()
    {
        transform.Translate(move);
    }

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
                if (c != null)
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

    bool InteractWithSelected(InteractButton usedButton)
    {
        if (currentInteraction is null || currentInteraction.interactButton != usedButton) return false;

        currentInventory = currentInteraction as Inventory;
        isInteracting = true;

        if (!currentInteraction.Interact(this))
        {
            // put animation for denied access here
        }

        return true;
    }
    #endregion

    //are called by the animator attached to this player, do not call these via code
    #region Animation events
    public void Anim_ToolAction() => tools.ToolAction();

    public void Anim_ToolStart() => tools.Anim_ToolStart();


    #endregion

    //Input call territory
    #region Joysticks

    public void Input_LStick(CallbackContext c)
    {
        inputMove = c.ReadValue<Vector2>();
    }

    public void Input_RStick(CallbackContext c)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Buttons
    public void Input_BNorth(CallbackContext c)
    {
        if (c.phase != InputActionPhase.Performed) return;

        if (CheckForInteractable()) InteractWithSelected(InteractButton.North);

        if (currentInventory != null)
        {
            currentInventory.QuickSplit(this, slot);
        }
        
    }

    public void Input_BEast(CallbackContext c)
    {
        if (c.phase != InputActionPhase.Performed) return;

        bool shouldDropItem = true;

        if (CheckForInteractable())
        {
            if (InteractWithSelected(InteractButton.East))
            {                
                shouldDropItem = false;
            }
        }        

        if(shouldDropItem)
        {
            BreakInteraction();
            if (container.PullItem(slot, out var item))
            {
                DroppedItem.DropUp(item.item, item.num, transform.position);
            }
        }
    }

    // also known as the Interact button
    public void Input_BSouth(CallbackContext c)
    {
        switch (c.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                if (c.action.WasPerformedThisFrame())
                {
                    if (!isInBuilding) tools.PlayerUse();
                }
                break;

            case InputActionPhase.Canceled:
                tools.PlayerStopUse();
                break;
        }

        if (CheckForInteractable()) InteractWithSelected(InteractButton.South);

    }

    public void Input_BWest(CallbackContext c)
    {
        if (c.phase != InputActionPhase.Performed) return;

        if (CheckForInteractable()) InteractWithSelected(InteractButton.West);

        if (currentInventory != null)
        {
            currentInventory.QuickSwap(this, slot);
        }
    }
    #endregion

    #region DPad inputs
    public void Input_DNorth(CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void Input_DEast(CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void Input_DSouth(CallbackContext c)
    {
        throw new System.NotImplementedException();
    }

    public void Input_DWest(CallbackContext c)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Shoulder buttons
    public void Input_ShoulderR(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            RotateRight();
        }
    }

    public void Input_ShoulderL(CallbackContext c)
    {
        if (c.phase == InputActionPhase.Performed)
        {
            RotateLeft();
        }
    }

    public void Input_NumberSelect(int num)
    {
        Debug.Log(num);
        if (num == 0) RotateLeft();
        if (num == 2) RotateRight();
        //HotbarSwitch(num);
    }
    #endregion
}