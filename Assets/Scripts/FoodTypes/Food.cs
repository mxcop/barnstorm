using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float nutrition;
    public float speed;
    public bool hasSplash;
    public GameObject projectile;
    public virtual void Collision(Collider2D collider){
    }
}
