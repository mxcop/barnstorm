using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Food food;
    private Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up.normalized * food.speed);
    }

    private void OnCollisionEnter2D(Collision2D collision) => food.Collision(collision);
}
