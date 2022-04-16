using System.Collections;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [HideInInspector] public Item item;
    [HideInInspector] public int num;

    private bool dropped = false;

    /// <summary>
    /// Drop the item with an animation.
    /// </summary>
    public static void DropUp(Item item, int num, Vector2 pos)
    {
        GameObject prefab = Resources.Load<GameObject>("DroppedItem");

        DroppedItem instance = Instantiate(prefab, pos, Quaternion.identity).GetComponent<DroppedItem>();

        // Setup the instance:
        instance.dropped = false;
        instance.item = item;
        instance.num = num;

        SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
        renderer.sprite = instance.item.sprite;
        renderer.color = new Color(1, 1, 1, 0);

        if (num > 1)
        {
            SpriteRenderer stacked = instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
            stacked.sprite = instance.item.sprite;
            stacked.color = new Color(1, 1, 1, 0);
        }

        // Start the animation.
        instance.StartDropUp(pos);
    }

    /// <summary>
    /// Drop the item with an animation.
    /// </summary>
    public static void DropOut(Item item, int num, Vector2 pos, Vector2 dir)
    {
        GameObject prefab = Resources.Load<GameObject>("DroppedItem");

        DroppedItem instance = Instantiate(prefab, pos, Quaternion.identity).GetComponent<DroppedItem>();

        // Setup the instance:
        instance.dropped = false;
        instance.item = item;
        instance.num = num;

        SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
        renderer.sprite = instance.item.sprite;
        renderer.color = new Color(1, 1, 1, 0);

        if (num > 1)
        {
            SpriteRenderer stacked = instance.GetComponent<SpriteRenderer>();
            stacked.sprite = instance.item.sprite;
            stacked.color = new Color(1, 1, 1, 0);
        }

        // Start the animation.
        instance.StartDropOut(pos, dir);
    }

    /// <summary>
    /// Start the drop animation.
    /// </summary>
    /// <param name="pos">The position of the animation.</param>
    private void StartDropUp(Vector2 pos)
    {
        StartCoroutine(AnimateDropUp(pos));
    }

    /// <summary>
    /// Start the drop animation.
    /// </summary>
    /// <param name="pos">The position of the animation.</param>
    /// <param name="dir">The direction of the animation.</param>
    private void StartDropOut(Vector2 pos, Vector2 dir)
    {
        StartCoroutine(AnimateDropOut(pos, dir));
    }

    public void AttemptPickup(Container<Item> container)
    {
        if (dropped && container.PushItem(item, num))
        {
            AnimatePickup();
            SFXManager.PlayClip("collect");
            dropped = false;
        }
    }

    /// <summary>
    /// Animate the drop of the item.
    /// </summary>
    private IEnumerator AnimateDropUp(Vector2 pos)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        transform.position = pos;

        // Animate the alpha of the item sprite.
        LeanTween.value(gameObject, c => renderer.color = c, new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.1f)
            .setOnComplete(() => renderer.color = new Color(1, 1, 1, 1));

        if (num > 1)
        {
            SpriteRenderer stackedRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            LeanTween.value(gameObject, c => stackedRenderer.color = c, new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.1f)
            .setOnComplete(() => stackedRenderer.color = new Color(1, 1, 1, 1));
        }

        // Make the item bounce.
        transform.localScale = Vector3.one * 0.5f;
        LeanTween.moveY(gameObject, pos.y + 1.0f, 0.4f).setEaseOutBack();
        LeanTween.scale(gameObject, Vector3.one * 1.0f, 0.4f).setEaseOutBack();

        yield return new WaitForSeconds(0.4f);

        LeanTween.moveY(gameObject, pos.y, 0.4f).setEaseInSine();

        yield return new WaitForSeconds(0.6f);

        dropped = true;
    }

    /// <summary>
    /// Animate the drop of the item.
    /// </summary>
    private IEnumerator AnimateDropOut(Vector2 pos, Vector2 dir)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        transform.position = pos;

        // Animate the alpha of the item sprite.
        LeanTween.value(gameObject, c => renderer.color = c, new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.1f)
            .setOnComplete(() => renderer.color = new Color(1, 1, 1, 1));

        // Make the item bounce.
        transform.localScale = Vector3.one * 0.4f;
        LeanTween.scale(gameObject, Vector3.one, 0.5f).setEaseOutBack();
        LeanTween.move(gameObject, pos + dir * 2.0f, 0.7f).setEaseInOutSine();

        yield return new WaitForSeconds(0.65f);

        dropped = true;
    }

    /// <summary>
    /// Animate the pickup of the item.
    /// </summary>
    private void AnimatePickup()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        // Animate the alpha of the item sprite.
        LeanTween.value(gameObject, c => renderer.color = c, new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.3f)
            .setOnComplete(() => Destroy(gameObject));

        // Make the item bounce.
        transform.localScale = Vector3.one;
        LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEaseInBack();
        LeanTween.move(gameObject, transform.position + Vector3.up * 2.0f, 0.3f).setEaseOutSine();
    }
}
