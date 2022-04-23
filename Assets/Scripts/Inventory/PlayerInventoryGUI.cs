using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventoryGUI : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("The time it takes for the inventory to fade out after having been used.")]
    [SerializeField] private float inventoryMuteDelay = 1.0f;

    [Header("References")]
    [SerializeField] private RectTransform cycleItem;
    [SerializeField] private RectTransform leftItem, centerItem, rightItem;
    [SerializeField] private Sprite emptySlot;
    [SerializeField] private CanvasGroup amountGroup;
    [SerializeField] private TextMeshProUGUI amountText, amountShadowText;

    private RectTransform[] GUI { get => GUIBasedOnRotation(); }
    private Container<Item> container;
    private Queue<RotationQueueEntry> rotationQueue;
    private int containerSize;
    [HideInInspector] public int rotation;

    private bool initialized = false;
    private bool isAnimating = false;

    public void Setup(ref Container<Item> container)
    {
        rotationQueue = new Queue<RotationQueueEntry>();
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
                // GUI array is a complete mind **** but it works
                SetItemSprite(GUI[i], i, true);
            }
        }

        // Setup the inventory amount.
        SetItemAmount(rotation, 0.0f);

        // Start the mute delay.
        StartCoroutine(nameof(MuteDelay));

        initialized = true;
    }

    /// <summary>
    /// Set the image sprite of an item in the GUI.
    /// </summary>
    private void SetItemSprite(RectTransform itemUI, int slot, bool hasToBeRendered = false, float alpha = -1.0f) 
    {
        Image image = itemUI.GetComponent<Image>();
        image.enabled = true;

        // Update the alpha of the image.
        if (alpha != -1.0f) image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

        // Check whether the slot is empty.
        if ((!hasToBeRendered || IsRendered(slot)) && container.Peek(slot, out Item item))
            image.sprite = item.sprite;
        else image.sprite = emptySlot;
    }

    /// <summary>
    /// Set the item amount GUI.
    /// </summary>
    private void SetItemAmount(int slot, float animationTime = 0.1f) 
    {
        // Peek at the amount of items in the selected slot.
        int amount = container.PeekAmount((int)Mathf.Repeat(slot, containerSize));
        if (amount > 1) {
            // Update the text and animate in.
            amountText.text = amount.ToString();
            amountShadowText.text = amount.ToString();
            LeanTween.value(amountGroup.gameObject, a => amountGroup.alpha = a, amountGroup.alpha, 1, animationTime);
        } else {
            LeanTween.value(amountGroup.gameObject, a => amountGroup.alpha = a, amountGroup.alpha, 0, animationTime);
        }
    }

    /// <summary>
    /// Will rotate towards the goal slot, isLeft dicates the direction of the animation.
    /// </summary>
    public void RotateTo(int slot, bool isLeft = false) 
    {
        // Make sure the slot is within range.
        slot = (int)Mathf.Repeat(slot, containerSize);

        // Figure out the distance between the current rotation and the goal.
        int distance = 0;
        int current = rotation;
        do {
            distance++;
            current = (int)Mathf.Repeat(isLeft ? current - 1 : current + 1, containerSize);
        } while (current != slot);

        // Enqueue the rotation (Only one queued rotation is allowed).
        if (rotationQueue.Count > 0) rotationQueue.Dequeue();
        rotationQueue.Enqueue(new RotationQueueEntry(isLeft, distance));

        if (isAnimating == false) 
            StartCoroutine(nameof(Rotate));
    }

    /// <summary>
    /// Massive animation method for rotating the inventory GUI.
    /// </summary>
    public IEnumerator Rotate()
    {
        isAnimating = true;

        while (rotationQueue.Count > 0)
        {
            // Restart the mute delay.
            StopCoroutine(nameof(MuteDelay));
            StartCoroutine(nameof(MuteDelay));

            RotationQueueEntry entry = rotationQueue.Dequeue();

            for (int i = 0; i < entry.distance; i++)
            {
                if (entry.isLeft)
                {
                    // Animate the movement ...
                    Vector3 outPos = leftItem.localPosition - new Vector3(centerItem.localPosition.x, centerItem.localPosition.y);
                    outPos.x = -outPos.x;

                    Vector3 leftPos = leftItem.localPosition;
                    Vector3 centerPos = centerItem.localPosition;
                    Vector3 rightPos = rightItem.localPosition;

                    // Setup the cycle item.
                    outPos.y = -outPos.y;
                    cycleItem.position = leftItem.position - outPos;
                    outPos.y = -outPos.y;
                    if (container.Peek((int)Mathf.Repeat(rotation - 2, containerSize), out Item item)) 
                    {
                        Image cycleImage = cycleItem.GetComponent<Image>();
                        cycleImage.color = new Color(1, 1, 1, 0);
                        cycleImage.sprite = item.sprite;
                        cycleItem.gameObject.SetActive(true);
                    } else cycleItem.gameObject.SetActive(false);

                    // Tween the items.
                    LeanTween.cancel(cycleItem);
                    LeanTween.alpha(cycleItem, 0.5f, 0.2f);
                    LeanTween.moveLocal(cycleItem.gameObject, leftPos, 0.2f);
                    LeanTween.cancel(leftItem);
                    LeanTween.alpha(leftItem, 1.0f, 0.2f);
                    LeanTween.moveLocal(leftItem.gameObject, centerPos, 0.2f);
                    LeanTween.cancel(centerItem);
                    LeanTween.alpha(centerItem, 0.5f, 0.2f);
                    LeanTween.moveLocal(centerItem.gameObject, rightPos, 0.2f);
                    LeanTween.cancel(rightItem);
                    LeanTween.alpha(rightItem, 0.0f, 0.2f);
                    LeanTween.moveLocal(rightItem.gameObject, rightPos + outPos, 0.2f);

                    // Update the item amount text.
                    SetItemAmount(rotation - 1);

                    yield return new WaitForSeconds(0.2f);

                    // Move Left
                    rotation = (int)Mathf.Repeat(rotation - 1, containerSize);

                    // Reset the items positons and swap their sprites.
                    leftItem.localPosition = leftPos;
                    SetItemSprite(leftItem, (int)Mathf.Repeat(rotation - 1, containerSize), false, 0.5f);
                    centerItem.localPosition = centerPos;
                    SetItemSprite(centerItem, (int)Mathf.Repeat(rotation, containerSize), false, 1.0f);
                    rightItem.localPosition = rightPos;
                    SetItemSprite(rightItem, (int)Mathf.Repeat(rotation + 1, containerSize), false, 0.5f);
                    cycleItem.gameObject.SetActive(false);
                }
                else 
                {
                    // Animate the movement ...
                    Vector3 outPos = leftItem.localPosition - new Vector3(centerItem.localPosition.x, centerItem.localPosition.y);
                    outPos.x = -outPos.x;

                    Vector3 leftPos = leftItem.localPosition;
                    Vector3 centerPos = centerItem.localPosition;
                    Vector3 rightPos = rightItem.localPosition;

                    // Setup the cycle item.
                    cycleItem.position = rightItem.position + outPos;
                    outPos.y = -outPos.y;
                    if (container.Peek((int)Mathf.Repeat(rotation + 2, containerSize), out Item item)) 
                    {
                        Image cycleImage = cycleItem.GetComponent<Image>();
                        cycleImage.color = new Color(1, 1, 1, 0);
                        cycleImage.sprite = item.sprite;
                        cycleItem.gameObject.SetActive(true);
                    } else cycleItem.gameObject.SetActive(false);

                    // Tween the items.
                    LeanTween.cancel(cycleItem);
                    LeanTween.alpha(cycleItem, 0.5f, 0.2f);
                    LeanTween.moveLocal(cycleItem.gameObject, rightPos, 0.2f);
                    LeanTween.cancel(rightItem);
                    LeanTween.alpha(rightItem, 1.0f, 0.2f);
                    LeanTween.moveLocal(rightItem.gameObject, centerPos, 0.2f);
                    LeanTween.cancel(centerItem);
                    LeanTween.alpha(centerItem, 0.5f, 0.2f);
                    LeanTween.moveLocal(centerItem.gameObject, leftPos, 0.2f);
                    LeanTween.cancel(leftItem);
                    LeanTween.alpha(leftItem, 0.0f, 0.2f);
                    LeanTween.moveLocal(leftItem.gameObject, leftPos - outPos, 0.2f);

                    // Update the item amount text.
                    SetItemAmount(rotation + 1);

                    yield return new WaitForSeconds(0.2f);

                    // Move Right
                    rotation = (int)Mathf.Repeat(rotation + 1, containerSize);

                    // Reset the items positons and swap their sprites.
                    leftItem.localPosition = leftPos;
                    SetItemSprite(leftItem, (int)Mathf.Repeat(rotation - 1, containerSize), false, 0.5f);
                    centerItem.localPosition = centerPos;
                    SetItemSprite(centerItem, (int)Mathf.Repeat(rotation, containerSize), false, 1.0f);
                    rightItem.localPosition = rightPos;
                    SetItemSprite(rightItem, (int)Mathf.Repeat(rotation + 1, containerSize), false, 0.5f);
                    cycleItem.gameObject.SetActive(false);
                }
            }
        }

        isAnimating = false;
    }

    /// <summary>
    /// Called and reset each time the inventory is updated.
    /// </summary>
    private IEnumerator MuteDelay() 
    {
        yield return new WaitForSeconds(inventoryMuteDelay);

        LeanTween.alpha(leftItem, 0.2f, 0.4f);
        LeanTween.alpha(centerItem, 0.5f, 0.4f);
        LeanTween.alpha(rightItem, 0.2f, 0.4f);
    }

    /// <summary>
    /// Called whenever the container has an update.
    /// </summary>
    private void OnContainerUpdate(int slot, ContainedItem<Item> item)
    {
        if (initialized)
        {
            // Restart the mute delay.
            StopCoroutine(nameof(MuteDelay));
            StartCoroutine(nameof(MuteDelay));

            // Make the items visible again.
            LeanTween.alpha(leftItem, 0.5f, 0.4f);
            LeanTween.alpha(centerItem, 1.0f, 0.4f);
            LeanTween.alpha(rightItem, 0.5f, 0.4f);

            // Update the sprites of the GUI.
            for (int i = 0; i < containerSize; i++)
            {
                // GUI array is a complete mind **** but it works
                SetItemSprite(GUI[i], i, true);

                if (i == slot && item != null)
                {
                    int ii = i;
                    LeanTween.scale(GUI[ii], Vector3.one * 1.4f, 0.2f).setEaseInBack().setOnComplete(() => 
                    {
                        LeanTween.scale(GUI[ii], Vector3.one, 0.1f);
                    });
                }
            }

            // Update the item amount text.
            SetItemAmount(rotation);
        }
    }

    private struct RotationQueueEntry 
    {
        public bool isLeft;
        public int distance;

        public RotationQueueEntry(bool isLeft, int distance) {
            this.isLeft = isLeft;
            this.distance = distance;
        }
    }
}
