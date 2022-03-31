using Systems.Inventory;
using TMPro;
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
    private Image InitCell(GameObject prefab) => Instantiate(prefab, transform.Find("Grid")).transform.Find("Item").GetComponent<Image>();

    /// <summary>
    /// Called whenever the container has an update.
    /// </summary>
    private void OnContainerUpdate(int slot, ContainedItem<Item> item)
    {
        Image cell = itemGUI[slot];
        RectTransform panel = cell.transform.parent.Find("Panel").GetComponent<RectTransform>();
        bool displayPanel = item != null && item.item.maximumStack > 1;

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

        if (displayPanel)
        {
            // Animate the item amount panel to move in.
            if (panel.localPosition.y != -0.1905f)
            {
                panel.gameObject.SetActive(true);
                LeanTween.cancel(panel.gameObject);
                LeanTween.value(panel.gameObject, v => panel.anchoredPosition = v, panel.anchoredPosition, new Vector2(panel.anchoredPosition.x, -0.1905f), 0.2f)
                    .setOnComplete(() => panel.anchoredPosition = new Vector3(panel.anchoredPosition.x, -0.1905f));
            }

            panel.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = item.num.ToString();
        }
        else
        {
            // Animate the item amount panel to move out.
            if (panel.localPosition.y != 0.25f)
            {
                LeanTween.cancel(panel.gameObject);
                LeanTween.value(panel.gameObject, v => panel.anchoredPosition = v, panel.anchoredPosition, new Vector2(panel.anchoredPosition.x, 0.25f), 0.2f)
                .setOnComplete(() =>
                {
                    panel.gameObject.SetActive(false);
                    panel.localPosition = new Vector3(panel.anchoredPosition.x, 0.25f);
                });
            }
        }
    }
}
