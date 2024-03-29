using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Food food;
    private Rigidbody2D rb;

    private int hitCount = 0;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = (transform.up.normalized * food.speed);
        StartCoroutine(DestroyLoop());
    }

    IEnumerator DestroyLoop() {
        yield return new WaitForSeconds(10f);
        food.Destroy(gameObject, 99);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        food.Collision(collision);

        if (collision.CompareTag("Enemy")){
            hitCount++;
            collision.gameObject.GetComponent<CowEnemy>().Feed(food);
            food.Destroy(gameObject, hitCount);
        }   
    }
}
