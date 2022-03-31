using System;
using Systems.Inventory;

/// <summary>
/// Keeps track of the data of a container.
/// </summary>
/// <typeparam name="T">The type to be stored inside the container.</typeparam>
public class Container<T> where T : Item
{
    public ContainedItem<T>[] data;
    public int size;

    public delegate void ContainerUpdateEventHandler(int slot, ContainedItem<T> item);
    public event ContainerUpdateEventHandler OnUpdate;

    public Container(int slots = 3) 
    {
        // Initlialize the data array.
        data = new ContainedItem<T>[slots];
        size = slots;
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
    /// Finds first matching item in the inventory.
    /// </summary>
    /// <param name="item">The item to match.</param>
    /// <param name="num">The number of items that will be added.</param>
    /// <param name="slot">The index of the matched slot.</param>
    /// <returns>Whether there is a matching slot.</returns>
    private bool FirstMatch(T item, int num, out int slot)
    {
        for (int i = 0; i < data.Length; i++)
        {
            ContainedItem<T> match = data[i];

            if (match != null && match.item.GetType() == item.GetType() && match.HasSpaceFor(num))
            {
                slot = i; return true;
            }
        }
        slot = -1; return false;
    }

    /// <summary>
    /// Find the first item of a specific type in the container.
    /// </summary>
    /// <param name="item">The item to match.</param>
    /// <param name="slot">The index of the matched slot.</param>
    /// <returns>Whether there is a matching slot.</returns>
    public bool FirstItemOfType(Type item, out int slot)
    {
        for (int i = 0; i < data.Length; i++)
        {
            ContainedItem<T> match = data[i];

            if (match != null && match.item.GetType() == item)
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
        if (!(item is null))
        {
            if (FirstMatch(item.item, item.num, out int match))
                return InsertItem(item, match);
            if (FirstOpen(out int slot))
                return InsertItem(item, slot);
        }
        return false;
    }

    /// <summary>
    /// Push an item into the inventory.
    /// </summary>
    /// <param name="item">The item to push.</param>
    /// <param name="amount">The amount of items to push.</param>
    /// <returns>If the inventory had space to push the item.</returns>
    public bool PushItem(T item, int amount)
    {
        if (!(item is null))
        {
            if (FirstMatch(item, amount, out int match))
                return InsertItem(item, amount, match);
            if (FirstOpen(out int slot))
                return InsertItem(item, amount, slot);
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
        if (!(item is null) && Exists(slot))
        {
            if (IsOpen(slot))
            {
                data[slot] = item.Clone();
                OnUpdate.Invoke(slot, item);
                return true;
            }
            else if (data[slot].item.GetType() == item.item.GetType() && data[slot].HasSpaceFor(item.num))
            {
                data[slot].num += item.num;
                OnUpdate.Invoke(slot, data[slot]);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Insert an item into the inventory.
    /// </summary>
    /// <param name="item">The item to insert.</param>
    /// <param name="amount">The amount of items to insert.</param>
    /// <param name="slot">The index of the slot to insert into.</param>
    /// <returns>If the slot was empty.</returns>
    public bool InsertItem(T item, int amount, int slot)
    {
        if (!(item is null) && Exists(slot))
        {
            if (IsOpen(slot))
            {
                data[slot] = new ContainedItem<T>(item, amount);
                OnUpdate.Invoke(slot, data[slot]);
                return true;
            }
            else if (data[slot].item.GetType() == item.GetType() && data[slot].HasSpaceFor(amount))
            {
                data[slot].num += amount;
                OnUpdate.Invoke(slot, data[slot]);
                return true;
            }
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
            OnUpdate.Invoke(slot, null);
            return true;
        }
        item = null;
        return false;
    }

    /// <summary>
    /// Pull a specific number of items from the inventory.
    /// </summary>
    /// <param name="slot">The index of the slot to pull from.</param>
    /// <param name="num">The number of items to pull out of the slot.</param>
    /// <param name="item">The item that has been pulled.</param>
    /// <returns>If the item exists and num is bigger than zero.</returns>
    public bool PullItem(int slot, int num, out ContainedItem<T> item)
    {
        if (Exists(slot) && IsOpen(slot) == false && num > 0)
        {
            if (data[slot].num <= num)
            {
                item = data[slot]; // Select the item.
                data[slot] = null; // Remove the item.
                OnUpdate.Invoke(slot, null);
            } 
            else
            {
                item = new ContainedItem<T>(data[slot].item, num); // Create new item with taken amount.
                data[slot].num -= num; // Decrease the item with taken amount.
                OnUpdate.Invoke(slot, data[slot]);
            }
            return true;
        }
        item = null;
        return false;
    }

    public void RemoveItem(int slot, int num) => PullItem(slot, num, out _);

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
    /// Peek a slot to see how many items are inside.
    /// </summary>
    /// <param name="slot">The index of the slot to check.</param>
    /// <returns>The number of items in the slot.</returns>
    public int PeekAmount(int slot)
    {
        if (Exists(slot) && IsOpen(slot) == false)
        {
            return data[slot].num;
        }
        return 0;
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

    /// <summary>
    /// Check if the inventory contains an item of this type.
    /// </summary>
    /// <param name="item">The type of item to look for.</param>
    /// /// <param name="slot">The index of the slot to check.</param>
    public bool ContainsAt(Type item, int slot)
    {
        return !(data[slot] is null) && data[slot].item.GetType() == item;
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != null)
                str += "\n" + data[i].item.ToString() + ":" + data[i].num;
            else
                str += "\nEmpty:0";
        }
        return str;
    }
}
