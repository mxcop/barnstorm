using Systems.Inventory;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private GameObject GUI;

    [Header("Inventory Config")]
    [SerializeField] private Vector2 offset;
    [SerializeField] private int size = 3;

    [Space]

    [HideInInspector] public Container<Item> container;
    private InventoryGUI gui;

    private void Awake()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Load the resources.
        GUI = Resources.Load<GameObject>("InventoryPanel");

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("WorldCanvas").transform).GetComponent<InventoryGUI>();
        gui.Initialize(size, ref container);
    }

    /// <summary>
    /// Open the inventory GUI.
    /// </summary>
    public void Open()
    {
        gui.transform.position = transform.position + (Vector3)offset;
        CanvasGroup canvas = gui.GetComponent<CanvasGroup>();
        gui.gameObject.SetActive(true);
        LeanTween.cancel(gui.gameObject);
        LeanTween.value(gui.gameObject, a => canvas.alpha = a, canvas.alpha, 1.0f, 0.3f);
    }

    /// <summary>
    /// Close the inventory GUI.
    /// </summary>
    public void Close()
    {
        CanvasGroup canvas = gui.GetComponent<CanvasGroup>();
        LeanTween.cancel(gui.gameObject);
        LeanTween.value(gui.gameObject, a => canvas.alpha = a, canvas.alpha, 0.0f, 0.1f)
            .setOnComplete(() => gui.gameObject.SetActive(false));
    }

    /// <summary>
    /// Swap the item in the selected player slot with the first inventory slot.
    /// </summary>
    /// <param name="player">The player to swap with.</param>
    /// <param name="slot">The selected slot in the player inventory.</param>
    public void QuickSwap(PlayerInventory player, int slot)
    {
        bool invItemExists = container.PullItem(0, out ContainedItem<Item> inventoryItem);
        bool playerItemExists = player.container.PullItem(slot, out ContainedItem<Item> playerItem);

        if (invItemExists) player.container.InsertItem(inventoryItem, slot);
        if (playerItemExists) container.InsertItem(playerItem, 0);
    }

    /// <summary>
    /// Split the item in the selected player slot into the first inventory slot.
    /// </summary>
    /// <param name="player">The player to split with.</param>
    /// <param name="slot">The selected slot in the player inventory.</param>
    public void QuickSplit(PlayerInventory player, int slot)
    {
        int half = Mathf.CeilToInt(player.container.PeekAmount(slot) / 2.0f);

        // If the player has an item in this slot:
        if (player.container.PullItem(slot, half, out ContainedItem<Item> item))
        {
            container.InsertItem(item, 0);
        }
    }
}
