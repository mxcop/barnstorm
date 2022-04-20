using Systems.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInventoryGUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform cycleItem;
    [SerializeField] private RectTransform leftItem, centerItem, rightItem;

    private RectTransform[] GUI { get => GUIBasedOnRotation(); }
    private Container<Item> container;
    private int containerSize;
    private int rotation;

    private bool initialized = false;

    public void Setup(ref Container<Item> container)
    {
        // Subscribe to the containers update event.
        container.OnUpdate += OnContainerUpdate;
    }

    /// <returns>The item gui elements based on the current inventory rotation.</returns>
    private RectTransform[] GUIBasedOnRotation()
    {
        RectTransform[] res = new RectTransform[containerSize];

        res[(int)Mathf.Repeat(rotation - 1, containerSize)] = leftItem;
        res[(int)Mathf.Repeat(rotation, containerSize)] = centerItem;
        res[(int)Mathf.Repeat(rotation + 1, containerSize)] = rightItem;

        return res;
    }

    /// <returns>Whether this slot is rendered.</returns>
    private bool IsRendered(int slot)
    {
        return 
            slot == (int)Mathf.Repeat(rotation - 1, containerSize) ||
            slot == (int)Mathf.Repeat(rotation, containerSize) ||
            slot == (int)Mathf.Repeat(rotation + 1, containerSize);
    }

    /// <summary>
    /// Initialize the inventory GUI.
    /// </summary>
    /// <param name="size">The size of the inventory in slots.</param>
    /// <param name="container">The container this inventory is linked to.</param>
    public void Initialize(int size, ref Container<Item> container)
    {
        // If this happens something went very wrong.
        if (size < 3) throw new System.Exception("Container size is smaller than the GUI");

        // Setup globals.
        containerSize = size;
        this.container = container;
        rotation = 0;

        // Setup the inventory GUI:
        if (size > 0)
        {
            // Update the sprites of the GUI.
            for (int i = 0; i < size; i++)
            {
                Image image = GUI[i].GetComponent<Image>();
                image.enabled = true;
                if (IsRendered(i) && container.Peek(i, out Item item))
                    image.sprite = item.sprite;
                else
                    image.enabled = false;
            }
        }

        initialized = true;
    }

    public void RotateLeft()
    {
        rotation--;
    }

    public void RotateRight()
    {
        rotation++;
    }

    /// <summary>
    /// Called whenever the container has an update.
    /// </summary>
    private void OnContainerUpdate(int slot, ContainedItem<Item> item)
    {
        if (initialized)
        {
            // Update the sprites of the GUI.
            for (int i = 0; i < containerSize; i++)
            {
                Image image = GUI[i].GetComponent<Image>();
                image.enabled = true;
                if (IsRendered(i) && container.Peek(i, out Item _item))
                    image.sprite = _item.sprite;
                else
                    image.enabled = false;
            }
        }
    }
}
