using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public bool inUse { get; set; }
    public abstract void Interact(int playerID);
    public abstract void BreakInteraction();
}
