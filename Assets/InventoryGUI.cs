using Systems.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject singleCell;
    [SerializeField] private GameObject leftCell, multiCell, rightCell;

    [Header("Config")]
    [SerializeField] private float slotSize = 136.5f;

    private Image[] itemGUI;

    /// <summary>
    /// Initialize the inventory GUI.
    /// </summary>
    /// <param name="size">The size of the inventory in slots.</param>
    /// <param name="container">The container this inventory is linked to.</param>
    public void Initialize(int size, ref Container<Item> container)
    {
        // Set the size of the inventory object.
        GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize * size, slotSize);

        // Setup the inventory GUI:
        if (size > 0)
        {
            itemGUI = new Image[size];

            if (size == 1)
                itemGUI[0] = InitCell(singleCell);
            else
            {
                itemGUI[0] = InitCell(leftCell);
                for (int i = 0; i < size - 2; i++)
                    itemGUI[i + 1] = InitCell(multiCell);
                itemGUI[size - 1] = InitCell(rightCell);
            }
        }

        // Subscribe to the containers update event.
        container.OnUpdate += OnContainerUpdate;
    }

    /// <summary>
    /// Instantiate a cell.
    /// </summary>
    /// <param name="prefab">The prefab of the cell.</param>
    /// <returns>The image within the cell.</returns>
    private Image InitCell(GameObject prefab) => Instantiate(prefab, transform).transform.GetChild(0).GetComponent<Image>();

    /// <summary>
    /// Called whenever the container has an update.
    /// </summary>
    private void OnContainerUpdate(int slot, ContainedItem<Item> item)
    {
        Image cell = itemGUI[slot];

        if (item != null)
        {
            cell.enabled = true;
            cell.sprite = item.item.sprite;
        }
        else
        {
            cell.enabled = false;
            cell.sprite = null;
        }
    }
}
