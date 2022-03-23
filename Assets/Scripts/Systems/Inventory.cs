using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject GUI;

    [Header("Config")]
    [SerializeField] private int size = 3;

    public Item testItem;

    [HideInInspector] public Container<Item> container;
    private InventoryGUI gui;

    private void Start()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("MainCanvas").transform).GetComponent<InventoryGUI>();
        gui.Initialize(size, ref container);

        container.PushItem(testItem, 14);
    }
}
