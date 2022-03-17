using System;
using Systems.Inventory;

/// <summary>
/// Keeps track of the data of a container.
/// </summary>
/// <typeparam name="T">The type to be stored inside the container.</typeparam>
public class Inventory<T> where T : Item
{
    public ContainedItem<T>[] data;

    public Inventory(int slots = 3) 
    {
        // Initlialize the data array.
        data = new ContainedItem<T>[slots];
    }

    /// <summary>
    /// Finds first open slot in the inventory.
    /// </summary>
    /// <param name="slot">The index of the open slot.</param>
    /// <returns>Whether there is an open slot.</returns>
    private bool FirstOpen(out int slot)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == null)
            {
                slot = i; return true;
            }
        }
        slot = -1; return false;
    }

    /// <summary>
    /// Check if a slot is empty / open.
    /// </summary>
    /// <param name="slot">The index of the slot to check.</param>
    private bool IsOpen(int slot) => data[slot] == null;

    /// <summary>
    /// Check if a slot exists.
    /// </summary>
    /// <param name="slot">The index of the slot to check.</param>
    private bool Exists(int slot) => slot >= 0 && slot < data.Length;

    /// <summary>
    /// Push an item into the inventory.
    /// </summary>
    /// <param name="item">The item to push.</param>
    /// <returns>If the inventory had space to push the item.</returns>
    public bool PushItem(ContainedItem<T> item)
    {
        if (!(item is null) && FirstOpen(out int slot))
        {
            return InsertItem(item, slot);
        }
        return false;
    }

    /// <summary>
    /// Insert an item into the inventory.
    /// </summary>
    /// <param name="item">The item to insert.</param>
    /// <param name="slot">The index of the slot to insert into.</param>
    /// <returns>If the slot was empty.</returns>
    public bool InsertItem(ContainedItem<T> item, int slot)
    {
        if (!(item is null) && Exists(slot) && IsOpen(slot))
        {
            data[slot] = item;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Pull an item from the inventory.
    /// </summary>
    /// <param name="slot">The index of the slot to pull from.</param>
    /// <param name="item">The item that has been pulled.</param>
    /// <returns>If the item exists.</returns>
    public bool PullItem(int slot, out ContainedItem<T> item)
    {
        if (Exists(slot) && IsOpen(slot) == false)
        {
            item = data[slot]; // Select the item.
            data[slot] = null; // Remove the item.
            return true;
        }
        item = null;
        return false;
    }

    /// <summary>
    /// Peek a slot to see what's inside of it.
    /// </summary>
    /// <param name="slot">The index of the slot to check.</param>
    /// <param name="item">The item inside the slot.</param>
    /// <returns>If the slot has an item.</returns>
    public bool Peek(int slot, out T item)
    {
        if (Exists(slot) && IsOpen(slot) == false)
        {
            item = data[slot].item;
            return true;
        }
        item = null;
        return false;
    }

    /// <summary>
    /// Check if the inventory contains an item of this type.
    /// </summary>
    /// <param name="item">The type of item to look for.</param>
    public bool Contains(Type item)
    {
        for (int i = 0; i < data.Length; i++)
            if (data[i].item.GetType() == item) return true;
        return false;
    }
}
