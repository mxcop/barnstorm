using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YLayering : MonoBehaviour
{
    [SerializeField] private bool isStatic = false;
    [SerializeField] private int layerOffset = 0;
    
    private SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();

        if (isStatic)
        {
            sp.sortingOrder = Mathf.FloorToInt(sp.bounds.min.y * -1.0f) * 2 + layerOffset;
        }
    }

    private void LateUpdate()
    {
        if (!isStatic && sp.isVisible) sp.sortingOrder = Mathf.FloorToInt(sp.bounds.min.y * -1.0f) * 2 + layerOffset;
    }
}
