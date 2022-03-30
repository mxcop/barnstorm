using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : Food
{
    public override void Collision(Collider2D collider) {
    }

    public override void Destroy(GameObject self, int hitCount) {
        if (hitCount >= 2)
            base.Destroy(self);
    }
}
