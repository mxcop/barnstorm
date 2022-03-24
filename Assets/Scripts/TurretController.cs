using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Inventory))]
public class TurretController : MonoBehaviour
{
    public Food ammunition;

    [SerializeField] private Sprite[] sprite;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private GameObject turret;
    [SerializeField] private float reloadTime;

    public List<EnemyBase> targetEnemies = new List<EnemyBase>();
    private SpriteRenderer turretsr;
    private Inventory inv;
    private Transform barn;
    private int rotationState = 0;
    private bool shooting = false;
    private float shootAngle;

    /// <summary>
    /// Calculating the angle between 2 points in degrees. 
    /// Also add a 180 degrees offset so Vector2.right equals 0.
    /// </summary>
    float AngleBetweenPoints(Vector2 p1, Vector2 p2)
    {
        Vector2 dir = p1 - p2;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
    }

    void Start()
    {
        turretsr = turret.GetComponent<SpriteRenderer>();
        inv = GetComponent<Inventory>();
        barn = GameObject.FindGameObjectWithTag("Barn").transform;

        // ---------Temporary---------
        inv.Open();
        inv.container.PushItem(ammunition, 25);
        // ---------Temporary---------
    }

    void Update()
    {
        // Get the Position of the mouse in world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());  
        
        // if we don't have a enemy in range dont go any further
        if (targetEnemies.Count <= 0)
            return;

        // Sort enemies if current target is defeated
        if (targetEnemies[0].state == EnemyBase.EnemyState.eating)
        {
            targetEnemies.RemoveAt(0);
            sortTargets();
            return;
        }
            

        // Get the angle from the turret to the enemy to determine 
        float spriteAngle = AngleBetweenPoints(turret.transform.position, targetEnemies[0].transform.position);
      
        // Mapping the angle to a value between 0-8 so we can get the sprite direction that faces the correct position
        int round = Mathf.RoundToInt(spriteAngle / 360 * 16);
        rotationState = round == 16 ? 0 : round;
        turretsr.sprite = sprite[rotationState];

        // If we are not shooting, the enemy is in a "attackable" state and we have more than 0 food in our turret we start the shooting coroutine
        if (!shooting && 
            (targetEnemies[0].state != EnemyBase.EnemyState.eating && targetEnemies[0].state != EnemyBase.EnemyState.retreating) && 
            inv.container.PeekAmount(0) > 0) {  
            StartCoroutine(Shoot());
        }
            
    }

    IEnumerator Shoot()
    {
        // Set shooting true so we don't shoot mulitple times
        shooting = true;

        // Calculate the intercepting shoot point
        Vector2 targetpos = targetEnemies[0].transform.position;
        float distance = Vector2.Distance(shootPoints[rotationState].position, targetpos);  //distance in between in meters
        float travelTime = distance / ammunition.speed;                                     //time in seconds the shot would need to arrive at the target
        Vector2 aimPoint = targetpos + targetEnemies[0].rb.velocity * travelTime;

        // Set the shoot angle to the intercepting point
        shootAngle = AngleBetweenPoints(shootPoints[rotationState].position, aimPoint + Vector2.up * 0.2f);

        // Instantiate bullet and remove a item from the inventory
        GameObject proj = Instantiate(ammunition.projectile, shootPoints[rotationState].position, Quaternion.identity);
        proj.transform.localRotation = Quaternion.Euler(0,0, shootAngle - 90);
        inv.container.PullItem(0, 1, out ContainedItem<Item> _);

        // Apply shoot delay adn set shooting false so we can shoot again
        yield return new WaitForSeconds(reloadTime);
        shooting = false;
    }

    public void sortTargets()
    {
        if (targetEnemies.Count <= 0)
            return;

        // Default the closest enemy to not exist
        EnemyBase closest = null;
        float closestDist = 99;

        // Loop over every Enemy and set it to the closest if it is the closest to the barn
        foreach (EnemyBase script in targetEnemies){
            if (script.state != EnemyBase.EnemyState.eating) {
                float dist = Vector2.Distance(script.transform.position, barn.position);
                if (dist < closestDist) {
                    closest = script;
                    closestDist = dist;
                }
            }
        }

        // Set the closest enemy as the first enemy
        targetEnemies.Remove(closest);
        targetEnemies.Insert(0, closest);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (!coll.CompareTag("Enemy"))
            return;

        targetEnemies.Add(coll.gameObject.GetComponent<EnemyBase>());
        sortTargets();
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (!coll.CompareTag("Enemy"))
            return;

        targetEnemies.Remove(coll.gameObject.GetComponent<EnemyBase>());
        sortTargets();
    }

}
