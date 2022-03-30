using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    public float nutrition;
    public float speed;
    public bool hasSplash;
    public GameObject projectile;
    public GameObject destroyObject;
    public virtual void Collision(Collider2D collider){}
    public virtual void Destroy(GameObject self, int hitcount = 0) {
        Instantiate(destroyObject, self.transform.position, Quaternion.identity);
        GameObject.Destroy(self);
    }
}
