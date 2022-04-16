using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemSucker : MonoBehaviour
{
    enum InventoryType { Player, World}

    Container<Item> container;

    [SerializeField] InventoryType containerToUse;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] Inventory inventory;

    [Space]
    [SerializeField] ContactFilter2D cf;
    [SerializeField] float range;

    private void FixedUpdate()
    {
        if (container == null)
        {
            switch (containerToUse)
            {
                default: container = playerInventory.container; break;
                case InventoryType.World: container = inventory.container; break;
            }
        }

        else
        {
            Collider2D[] colls = new Collider2D[6];
            if (Physics2D.OverlapCircle(transform.position, range, cf, colls) > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    Collider2D coll = colls[i];
                    if (coll != null)
                    {
                        coll.GetComponent<DroppedItem>().AttemptPickup(container);
                    }
                }
            }
        }
    }
}
