using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public InteractButton interactButton { get;}
    public bool inUse { get; set; }

    /// <summary>
    /// Interacts with this interactable, returns false if the interaction failed
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public abstract bool Interact(Player player);
    public abstract void BreakInteraction();
}

public enum InteractButton { West, North, East, South}
