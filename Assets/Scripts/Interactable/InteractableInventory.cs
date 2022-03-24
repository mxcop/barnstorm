using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableInventory : Inventory, Interactable
{
    public bool inUse { get; set; }

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
}
