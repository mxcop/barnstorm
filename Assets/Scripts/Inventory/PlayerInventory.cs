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
        GUI = Resources.Load<GameObject>("PlayerPanel");

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("MainCanvas").transform.Find("Lobby Panel")).GetComponent<PlayerInventoryGUI>();
        gui.Initialize(size, ref container);

        for (int i = 0; i < starterItems.Length; i++)
            container.PushItem(starterItems[i].item, starterItems[i].amount);

        SelectSlot(0);
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
    /// <param name="slot">The slot to select.</param>
    public void SelectSlot(int slot)
    {
        this.slot = slot;
        gui.UpdateSelection(slot);
    }
}
