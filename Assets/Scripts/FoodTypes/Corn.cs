using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : Food
{
    public override void Collision(Collision2D collider)
    {
        Debug.Log(collider);
    }
}
