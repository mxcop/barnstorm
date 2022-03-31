using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;
    public ItemAction useAction;

    [Range(1, 999)]
    public int maximumStack = 999;
}

public enum ItemAction
{
    None,
    Plant,
    Till
}