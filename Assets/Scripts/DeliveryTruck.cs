using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTruck : MonoBehaviour
{
    [SerializeField] private SpriteRenderer coverRenderer;
    [SerializeField] private Sprite coverSprite;
    private List<GameObject> players = new List<GameObject>();

    private void FixedUpdate()
    {
        if(players.Count > 0)
        {
            coverRenderer.sprite = null;
            //show sprite
        }
        else if (!coverRenderer.sprite)
        {
            coverRenderer.sprite = coverSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(transform);
            players.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.SetParent(null);
            players.Remove(collision.gameObject);
        }
    }
}
