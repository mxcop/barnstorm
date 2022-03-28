using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YLayering : MonoBehaviour
{
    [SerializeField] private bool isStatic = false;
    [SerializeField] private int layerOffset = 0;
    
    private SpriteRenderer sp;
    private Camera cam;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        if (isStatic)
        {
            sp.sortingOrder = Mathf.FloorToInt(cam.WorldToScreenPoint(sp.bounds.min).y * -1.0f) + layerOffset;
        }
    }

    private void LateUpdate()
    {
        if (!isStatic && sp.isVisible) sp.sortingOrder = Mathf.FloorToInt(cam.WorldToScreenPoint(sp.bounds.min).y * -1.0f) + layerOffset;
    }
}
