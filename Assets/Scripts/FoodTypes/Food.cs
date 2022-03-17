using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float nutrition;
    public float speed;
    public bool hasSplash;
    public virtual void Collision(Collision2D collider){}
}
