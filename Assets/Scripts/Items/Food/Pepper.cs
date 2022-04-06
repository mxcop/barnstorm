using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepper : Food
{
    public float stunTime;
    public override void Collision(Collider2D collider)
    {
        if (!collider.CompareTag("Enemy") || !collider.TryGetComponent<EnemyBase>(out EnemyBase Enemy))
            return;

        Enemy.StartStun(stunTime);
        base.Collision(collider);
    }
}
