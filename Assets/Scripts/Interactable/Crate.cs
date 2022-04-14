using Systems.Inventory;
using UnityEngine;

public class Crate : Inventory, Interactable
{
    [Header("Static Config")]
    [SerializeField] private GameObject hint;
    [SerializeField] private Sprite[] crates;
    [SerializeField] private Sprite lid;
    [SerializeField] private ItemSprite[] items;

    private SpriteRenderer crate;
    private SpriteRenderer overlay;

    [HideInInspector] public bool inUse { get; set; }

    private void Start()
    {
        crate = GetComponent<SpriteRenderer>();
        overlay = transform.Find("Overlay").GetComponent<SpriteRenderer>();

        RandomCrate();

        container.OnUpdate += OnUpdate;

        if (container.Peek(0, out Item item))
        {
            // Call an update on start if there is an item in the first slot.
            OnUpdate(0, new ContainedItem<Item>(item, 1));
        }
    }

    public void Interact()
    {
        Open();
        inUse = true;
    }

    public void BreakInteraction()
    {
        Close();
        inUse = false;
    }

    /// <summary> Select a random sprite for the crate. </summary>
    private void RandomCrate() => crate.sprite = crates[Random.Range(0, crates.Length)];

    /// <summary>
    /// Called whenever there is an update to the container.
    /// </summary>
    private void OnUpdate(int _, ContainedItem<Item> item)
    {
        Sprite overlay = lid;

        // Match the items list with the updated item:
        if (item != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].name == item.item.GetType().ToString())
                {
                    overlay = items[i].sprite;
                    break;
                }
            }
        }

        // Update the overlay to the correct sprite.
        this.overlay.sprite = overlay;
    }

    [System.Serializable]
    private struct ItemSprite
    {
        [Tooltip("Name of the item class")]
        public string name;
        [Tooltip("Overlay sprite for this item")]
        public Sprite sprite;
    }
}