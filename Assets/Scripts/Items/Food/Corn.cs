using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : Food
{
    public float blastRadius;
    public override void Collision(Collider2D collider)
    {
        if (!collider.CompareTag("Enemy"))
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(collider.transform.position, blastRadius);
        for (int i = 0; i < colliders.Length; i++) {
            if (!(colliders[i] == collider || !colliders[i].TryGetComponent<EnemyBase>(out EnemyBase Enemy))) {
                Enemy.Feed(this);
            }
        }
        base.Collision(collider);
    }
}
