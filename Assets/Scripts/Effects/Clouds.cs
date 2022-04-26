using UnityEngine;

public class Clouds : MonoBehaviour
{
    private SpriteRenderer image;

    private Vector2 time;

    private void Start()
    {
        image = GetComponent<SpriteRenderer>();

        time = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
    }

    private void Update()
    {
        time.x += Time.deltaTime / Random.Range(50.0f, 100.0f) * Random.Range(1, -1);
        time.y += Time.deltaTime / Random.Range(50.0f, 100.0f) * Random.Range(1, -1);

        image.material.SetFloat("_offsetX", time.x);
        image.material.SetFloat("_offsetY", time.y);
    }
}