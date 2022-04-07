using Systems.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryGUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject singleCell;
    [SerializeField] private GameObject leftCell, multiCell, rightCell;
    [SerializeField] private Sprite[] players;

    [Header("Config")]
    [SerializeField] private float slotSize = 136.5f;

    private Image[] itemGUI;
    private RectTransform selectArrow;

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

            // The following seems like its in reverse!
            // However this is intentional because the grid is reversed!
            if (size == 1)
                itemGUI[0] = InitCell(singleCell);
            else
            {
                itemGUI[size - 1] = InitCell(rightCell);
                for (int i = 0; i < size - 2; i++)
                    itemGUI[size - 2 - i] = InitCell(multiCell);
                itemGUI[0] = InitCell(leftCell);
            }
        }

        // Get the selection arrow transform.
        selectArrow = transform.Find("Panel").Find("Select").GetComponent<RectTransform>();

        // Set the player icon.
        transform.Find("Panel").Find("Icon").GetComponent<Image>().sprite = players[LobbyManager.players.Count - 1];

        // Subscribe to the containers update event.
        container.OnUpdate += OnContainerUpdate;
    }

    /// <summary>
    /// Instantiate a cell.
    /// </summary>
    /// <param name="prefab">The prefab of the cell.</param>
    /// <returns>The image within the cell.</returns>
    private Image InitCell(GameObject prefab) => Instantiate(prefab, transform.Find("Panel").Find("Inv")).transform.Find("Item").GetComponent<Image>();

    /// <summary>
    /// Update the selection arrow gui.
    /// </summary>
    /// <param name="slot">The new selected slot.</param>
    public void UpdateSelection(int slot)
    {
        LeanTween.cancel(selectArrow.gameObject);
        LeanTween.value(selectArrow.gameObject, e => selectArrow.anchoredPosition = e, selectArrow.anchoredPosition, new Vector2(slotSize * 1.5f + slotSize * slot, selectArrow.anchoredPosition.y), 0.1f)
            .setOnComplete(() => selectArrow.anchoredPosition = new Vector3(slotSize * 1.5f + slotSize * slot, selectArrow.anchoredPosition.y));
    }

    /// <summary>
    /// Called whenever the container has an update.
    /// </summary>
    private void OnContainerUpdate(int slot, ContainedItem<Item> item)
    {
        Image cell = itemGUI[slot];
        RectTransform panel = cell.transform.parent.Find("Panel").GetComponent<RectTransform>();

        if (item != null)
        {
            cell.enabled = true;
            cell.sprite = item.item.sprite;

            // Animate the item sprite to scale in.
            LeanTween.cancel(cell.gameObject);
            RectTransform itemTransform = cell.GetComponent<RectTransform>();
            if (itemTransform.localScale == Vector3.zero)
            {
                LeanTween.value(cell.gameObject, s => itemTransform.localScale = s, Vector3.zero, Vector3.one, 0.4f)
                .setEaseOutBack().setOnComplete(() => itemTransform.localScale = Vector3.one);
            }
            else
            {
                LeanTween.value(cell.gameObject, s => itemTransform.localScale = s, Vector3.one * .75f, Vector3.one, 0.2f)
                .setEaseOutBack().setOnComplete(() => itemTransform.localScale = Vector3.one);
            }
        }
        else
        {
            // Animate the item sprite to scale out.
            LeanTween.cancel(cell.gameObject);
            RectTransform itemTransform = cell.GetComponent<RectTransform>();
            LeanTween.value(cell.gameObject, s => itemTransform.localScale = s, itemTransform.localScale, Vector3.zero, 0.2f)
            .setOnComplete(() =>
            {
                cell.enabled = false;
                cell.sprite = null;
                itemTransform.localScale = Vector3.zero;
            });
        }

        if (item != null && item.item.maximumStack > 1)
        {
            // Animate the item amount panel to move in.
            if (panel.localScale.y != 1)
            {
                Debug.Log($"Panel Fadein ({ item.item.name })");
                LeanTween.cancel(panel.gameObject);
                LeanTween.value(panel.gameObject, v => panel.localScale = v, panel.localScale, Vector3.one, 0.2f).setEaseOutBack()
                    .setOnComplete(() => panel.localScale = Vector3.one);
            }

            panel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = item.num.ToString();
        }
        else
        {
            // Animate the item amount panel to move out.
            if (panel.localScale.y != 0)
            {
                Debug.Log($"Panel Fadeout");
                LeanTween.cancel(panel.gameObject);
                LeanTween.value(panel.gameObject, v => panel.localScale = v, panel.localScale, new Vector3(1, 0, 1), 0.2f).setEaseInBack()
                    .setOnComplete(() => panel.localScale = new Vector3(1, 0, 1));
            }
        }
    }
}
