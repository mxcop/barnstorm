using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : Food
{
    public override void Collision(Collider2D collider)
    {
        if (!collider.CompareTag("Enemy") || !collider.TryGetComponent<EnemyBase>(out EnemyBase Enemy))
            return;

       Enemy.StartBounce(transform, collider);

        base.Collision(collider);
    }
}
