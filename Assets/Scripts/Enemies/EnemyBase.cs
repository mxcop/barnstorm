using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour
{
    // public Food favoriteFood;
    public bool inAnimation = false;
    public bool doneEating;

    private enum EnemyState
    {
        running,
        attacking,
        eating,
        retreating,
    }
    [SerializeField] private EnemyState state = EnemyState.running;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    
    [SerializeField] private float maxHunger;
    [SerializeField] private float hunger;
    [SerializeField] private float speed;
    [SerializeField] private Transform target;

    public void Feed(Food food) { hunger -= food.nutrition; if (hunger <= 0 && state != EnemyState.retreating) state = EnemyState.eating; }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        hunger = maxHunger;
    }

    private void FixedUpdate()
    {
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
        transform.Translate(direction * speed);
    }

    protected virtual void Retreating()
    {
        speed = 0.03f;
        anim.speed = 1.5f;
        Vector2 direction = (target.transform.position - transform.position).normalized;
        
        if (direction.x > 0) sr.flipX = true;
        else sr.flipX = false;
        transform.Translate(-direction * speed);
    }
}