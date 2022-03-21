using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : Food
{
    public override void Collision(Collision2D collider){
        Debug.Log(collider);
    }
}
