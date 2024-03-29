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

    public GameObject particle;
    public float speed;
    public float bounceStrength;
    public float bounceTime;

    private Animator anim;
    private SpriteRenderer sr;
    private bool isBouncing = false;
    private bool isStunned = false;

    [HideInInspector] public Rigidbody2D rb;
    [SerializeField] private float maxHunger;

    [HideInInspector] public float hunger;
    private Transform target;

    public void Feed(Food food) { 
        hunger -= food.nutrition; 

        if (hunger <= 0 && state != EnemyState.retreating) 
            state = EnemyState.eating;

        Instantiate(particle, transform.position + (Vector3)(Vector2.up * 0.75f + (sr.flipX ? Vector2.left : Vector2.right) * 0.25f), particle.transform.rotation);
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Barn").transform;
        hunger = maxHunger;
    }

    private void FixedUpdate()
    {
        if (isBouncing || isStunned)
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
                    gameObject.layer = 9;
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
        if (transform.position.magnitude > 22)
            Destroy(gameObject);

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
            StartCoroutine(Bounce(collision.transform, collision.collider));
    }

    public void StartBounce(Transform other, Collider2D collision) => StartCoroutine(Bounce(other, collision));

    public IEnumerator Bounce(Transform other, Collider2D collision)
    {
        isBouncing = true;
        rb.AddForceAtPosition(((Vector2)transform.position -collision.ClosestPoint(transform.position))  * bounceStrength, other.transform.position);
        yield return new WaitForSeconds(bounceTime);
        isBouncing = false;
    }

    public void StartStun(float stunTime) => StartCoroutine(Stun(stunTime));

    public IEnumerator Stun(float stunTime)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

}