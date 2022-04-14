using UnityEngine;
using UnityEngine.UI;

public class IndicatorManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject indicator;

    public static IndicatorManager instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Spawn a new indicator.
    /// </summary>
    /// <param name="sprite">The button sprite for this indicator.</param>
    /// <param name="worldpos">The world position for this indicator.</param>
    /// <returns>The indicator object.</returns>
    public static GameObject Spawn(Sprite sprite, Vector2 worldpos)
    {
        Transform canvas = GameObject.FindWithTag("WorldCanvas").transform;

        // Initialize the indicator object.
        GameObject obj = Instantiate(instance.indicator, canvas);
        obj.transform.position = worldpos;
        Image button = obj.transform.Find("Button").GetComponent<Image>();
        button.sprite = sprite;

        // Animate the indicator object.
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        LeanTween.value(obj, e => obj.transform.localScale = new Vector3(e, 1, 1), 0, 1, 0.2f).setEaseOutBack();
        LeanTween.value(obj, e => group.alpha = e, 0, 1, 0.15f);

        return obj;
    }

    public static void Despawn(GameObject obj)
    {
        // Animate the indicator object.
        LeanTween.cancel(obj);
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        LeanTween.value(obj, e => obj.transform.localScale = new Vector3(e, 1, 1), obj.transform.localScale.x, 0, 0.2f).setEaseInBack();
        LeanTween.value(obj, e => group.alpha = e, group.alpha, 0, 0.2f).setOnComplete(() => Destroy(obj));
    }
}