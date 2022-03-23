using System.Collections;
using Systems.Inventory;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject GUI;

    [Header("Config")]
    [SerializeField] private int size = 3;

    public Item[] testItems;

    [HideInInspector] public Container<Item> container;
    private InventoryGUI gui;

    private void Start()
    {
        // Initialize the container of the inventory.
        container = new Container<Item>(size);

        // Create the GUI of the inventory.
        gui = Instantiate(GUI, GameObject.FindWithTag("MainCanvas").transform).GetComponent<InventoryGUI>();
        gui.Initialize(size, ref container);

        /// StartCoroutine(nameof(InvTest));
    }

#if UNITY_EDITOR
    IEnumerator InvTest()
    {
        yield return new WaitForSeconds(1);

        container.PushItem(testItems[Random.Range(0, testItems.Length)], 14);

        yield return new WaitForSeconds(1);

        container.PushItem(testItems[Random.Range(0, testItems.Length)], 43);

        yield return new WaitForSeconds(1);

        container.PullItem(0, out ContainedItem<Item> item);

        yield return new WaitForSeconds(1);

        container.InsertItem(item, 2);

        for (int i = 0; i < 1000; i++)
        {
            yield return new WaitForSeconds(.5f);

            int slot = Random.Range(0, size);

            if (container.Peek(slot, out Item _))
            {
                container.PullItem(slot, 64, out ContainedItem<Item> _);
            }
            else container.InsertItem(testItems[Random.Range(0, testItems.Length)], 64, slot);
        }
    }

#endif
}
