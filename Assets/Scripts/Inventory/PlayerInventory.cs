using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject GUI;

    [Header("Inventory Config")]
    [SerializeField] private int size = 3;

    [Space]

    [HideInInspector] public Container<Item> container;
    [HideInInspector] public int slot;

    protected PlayerInventoryGUI gui;

    [SerializeField] private StarterItem[] starterItems;

    protected virtual void Awake()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Load the resources.
        GUI = Resources.Load<GameObject>("Player Inventory");

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("WorldCanvas").transform).GetComponent<PlayerInventoryGUI>();
        gui.Setup(ref container);

        for (int i = 0; i < starterItems.Length; i++)
            container.PushItem(starterItems[i].item, starterItems[i].amount);

        gui.Initialize(size, ref container);
    }

    private void LateUpdate()
    {
        if (!(gui is null))
            gui.transform.position = transform.position + Vector3.up;
    }

    [System.Serializable]
    private struct StarterItem
    {
        public Item item;
        public int amount;
    }

    /// <summary>
    /// Set the selected slot of the player inventory.
    /// </summary>
    public void RotateRight()
    {
        //this.slot = slot;
        gui.RotateRight();
    }

    public void RotateLeft()
    {
        gui.RotateLeft();
    }
}