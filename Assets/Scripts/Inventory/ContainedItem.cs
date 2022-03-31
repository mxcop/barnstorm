namespace Systems.Inventory
{
    /// <summary>
    /// A container for a type of item.
    /// Which keeps track of the number of the items.
    /// </summary>
    public class ContainedItem<T> where T : Item
    {
        /// <summary>
        /// The number of items contained.
        /// </summary>
        public int num;

        /// <summary>
        /// The type of item contained.
        /// </summary>
        public T item;

        public ContainedItem(T item, int num)
        {
            this.item = item;
            this.num = num;
        }

        /// <param name="num">The number of items to check for.</param>
        /// <returns>Whether there is space for the given number of items.</returns>
        public bool HasSpaceFor(int num) => this.num + num <= item.maximumStack;

        public ContainedItem<T> Clone() => new ContainedItem<T>(item, num);
    }
}
