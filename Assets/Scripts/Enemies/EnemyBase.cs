using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour
{
    // public Food favoriteFood;
    public bool inAnimation = false;
    public bool doneEating;
    public enum EnemyState
    {
        running,
        attacking,
        eating,
        retreating,
    }
    public EnemyState state = EnemyState.running;

    public Rigidbody2D rb;
    public float speed;
    public float bounceStrength;
    public float bounceTime;

    private Animator anim;
    private SpriteRenderer sr;
    private bool isBouncing = false;

    [SerializeField] private float maxHunger;
    [SerializeField] private float hunger;
    [SerializeField] private Transform target;

    public void Feed(Food food) { hunger -= food.nutrition; if (hunger <= 0 && state != EnemyState.retreating) state = EnemyState.eating; }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Barn").transform;
        hunger = maxHunger;
    }

    private void FixedUpdate()
    {
        if (isBouncing)
            return;

        switch (state)
        {
            case EnemyState.running:
                Running();
                break;
            case EnemyState.attacking:
                break;
            case EnemyState.eating:
                if (!inAnimation){
                    inAnimation = true;
                    rb.velocity = Vector2.zero;
                    anim.SetTrigger("Eat");
                }
                if (doneEating)
                    state = EnemyState.retreating;
                break;
            case EnemyState.retreating:
                 Retreating();
                break;
        }
    }

    protected virtual void Running() {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        if (direction.x < 0) sr.flipX = true;
        else sr.flipX = false;
        rb.velocity = (direction * speed);
    }

    protected virtual void Retreating()
    {
        speed = 5f;
        anim.speed = 1.5f;
        Vector2 direction = (target.transform.position - transform.position).normalized;
        
        if (direction.x > 0) sr.flipX = true;
        else sr.flipX = false;
        rb.velocity = (-direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Untagged"))
            StartCoroutine(Bounce(collision.transform, (collision.transform.position.x - transform.position.x > 0 ? true : false)));
    }

    IEnumerator Bounce(Transform other, bool rightSide)
    {
        isBouncing = true;
        rb.AddForceAtPosition(-(other.transform.position - transform.position + (Vector3)(rightSide ? Vector2.right : Vector2.left) * 2).normalized * bounceStrength, other.transform.position);
        yield return new WaitForSeconds(bounceTime);
        isBouncing = false;
    }
}