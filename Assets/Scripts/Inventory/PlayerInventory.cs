using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject GUI;

    [Header("Inventory Config")]
    [SerializeField] private int size = 3;

    [Space]

    [HideInInspector] public Container<Item> container;
    [HideInInspector] public int selected;

    private PlayerInventoryGUI gui;

    [SerializeField] private Item TEST_ITEM;

    private void Awake()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Load the resources.
        GUI = Resources.Load<GameObject>("PlayerPanel");

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("MainCanvas").transform.Find("Lobby Panel")).GetComponent<PlayerInventoryGUI>();
        gui.Initialize(size, ref container);

        container.PushItem(TEST_ITEM, 14);
        container.PushItem(TEST_ITEM, 5);
    }

    /// <summary>
    /// Set the selected slot of the player inventory.
    /// </summary>
    /// <param name="slot">The slot to select.</param>
    public void SelectSlot(int slot)
    {
        selected = slot;

        // TODO : Select a slot visually
    }
}
