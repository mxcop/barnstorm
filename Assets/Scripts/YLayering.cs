using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YLayering : MonoBehaviour
{
    private SpriteRenderer sp;
    private Camera cam;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (sp.isVisible) sp.sortingOrder = Mathf.FloorToInt(cam.WorldToScreenPoint(sp.bounds.min).y * -1.0f);
    }
}
