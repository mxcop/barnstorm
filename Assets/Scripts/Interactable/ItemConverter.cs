using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConverter : MonoBehaviour, Interactable
{
    [SerializeField] ItemConversion[] _conversionTable;
    Dictionary<Item, ItemConversion> conversionTable = new Dictionary<Item, ItemConversion>();

    public bool inUse { get; set; }

    private void Awake()
    {
        foreach(ItemConversion c in _conversionTable)
        {
            conversionTable.Add(c.input, c);
        }
    }

    public void BreakInteraction()
    {
        // do nothing
    }

    public bool Interact(Player player)
    {
        if(player.container.Peek(player.slot, out Item item))
        {
            if (conversionTable.ContainsKey(item))
            {
                ItemConversion conv = conversionTable[item];
                DroppedItem.DropOut(conv.output, conv.exchangeRate, transform.position, Vector2.up);

                player.container.PullItem(player.slot, 1, out var _);

                return true;
            }
        }

        return false;
    }

    [System.Serializable]
    struct ItemConversion
    {
        public Item input;
        public Item output;
        [Space]
        public int exchangeRate;
    }
}


