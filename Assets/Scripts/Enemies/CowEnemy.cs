using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowEnemy : AstarEnemy
{
    [SerializeField] private float maxHunger;
    [HideInInspector] public float hunger;
    [SerializeField] private GameObject particle;

    [HideInInspector] public bool inAnimation;
    [HideInInspector] public bool doneEating;
    [HideInInspector] public bool isStunned;

    [HideInInspector] public EnemyState state = EnemyState.running;
    public enum EnemyState
    {
        running,
        attacking,
        eating,
        retreating,
    }

    [HideInInspector]
    public Rigidbody2D rb;   
    private SpriteRenderer sr;
    private Animator anim;

    protected override void Start() {
        spawnPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        target = GameObject.FindGameObjectWithTag("Barn").transform.position;
        hunger = maxHunger;

        base.Start();
    }

    void FixedUpdate()
    {
        // Don't go to the state machine if we are stunned
        if (isStunned)
            return;

        if (transform.position.x - target.x > 0) sr.flipX = true;
        else sr.flipX = false;

        // Basic State Machine
        switch (state) {
            case EnemyState.running:
                Running();
                break;
            case EnemyState.attacking:
                break;
            case EnemyState.eating:
                if (!inAnimation) {
                    inAnimation = true;
                    rb.velocity = Vector2.zero;
                    gameObject.layer = 9;
                    anim.SetTrigger("Eat");
                }
                if (doneEating) {
                    ChangeToRetreatPath();
                    state = EnemyState.retreating;
                }   
                break;
            case EnemyState.retreating:
                Retreating();
                break;
        }
    }

    private void Running() {
        FollowPath();
    }

    private void Retreating() {
        FollowPath();
    }

    public void StartStun(float stunTime) => StartCoroutine(Stun(stunTime));

    public IEnumerator Stun(float stunTime)
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

    public void Feed(Food food)
    {
        hunger -= food.nutrition;

        if (hunger <= 0 && state != EnemyState.retreating)
            state = EnemyState.eating;

        Instantiate(particle, transform.position + (Vector3)(Vector2.up * 0.75f + (sr.flipX ? Vector2.left : Vector2.right) * 0.25f), particle.transform.rotation);
    }
}
