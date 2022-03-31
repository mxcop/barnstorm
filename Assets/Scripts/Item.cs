using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;

    [Range(1, 999)]
    public int maximumStack = 999;
}