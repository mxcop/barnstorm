using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;
    public ItemAction useAction;

    [Tooltip("Only applicable if useAction is set to Plant")]
    public CropType useAction_cropType;

    [Range(1, 999)]
    public int maximumStack = 999;
}

public enum ItemAction
{
    None,
    Plant,
    Till
}