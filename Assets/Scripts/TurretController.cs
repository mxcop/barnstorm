using System.Collections;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;

public class TurretController : Inventory, Interactable, ITriggerRerouting
{
    public Food ammunition;

    [SerializeField] private GameObject hint;
    [SerializeField] private Sprite[] sprite;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private GameObject turret;
    [SerializeField] private float reloadTime;
    [SerializeField] int startingAmmuntion;

    public List<CowEnemy> targetEnemies = new List<CowEnemy>();
    private SpriteRenderer turretsr;
    private Transform barn;
    private int rotationState = 0;
    private bool shooting = false;
    private float shootAngle;

    private GameObject indicator;

    public bool inUse { get; set; }
    public InteractButton interactButton { get => InteractButton.West; }

    public bool Interact(Player player)
    {
        if (!inUse)
        {
            Open();
            inUse = true;
            return true;
        }
        else return false;
    }

    public void BreakInteraction()
    {
        Close();
        inUse = false;
    }

    /// <summary>
    /// Calculating the angle between 2 points in degrees. 
    /// Also add a 180 degrees offset so Vector2.right equals 0.
    /// </summary>
    float AngleBetweenPoints(Vector2 p1, Vector2 p2)
    {
        Vector2 dir = p1 - p2;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
    }

    private void Start()
    {
        turretsr = turret.GetComponent<SpriteRenderer>();
        barn = GameObject.FindGameObjectWithTag("Barn").transform;

        container.PushItem(ammunition, startingAmmuntion);
    }

    void Update()
    {
        // if we don't have a enemy in range dont go any further
        if (targetEnemies.Count <= 0)
            return;

        // Sort enemies if current target is defeated
        if (targetEnemies[0].state == CowEnemy.EnemyState.eating)
        {
            targetEnemies.RemoveAt(0);
            SortTargets();
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
            (targetEnemies[0].state != CowEnemy.EnemyState.eating && targetEnemies[0].state != CowEnemy.EnemyState.retreating) &&
            container.Peek(0, out Item item)) {
            if(item is Food food)
            {
                ammunition = food;
                StartCoroutine(Shoot());
            }            
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
        Vector2 aimPoint = targetpos + targetEnemies[0].velocity * travelTime;

        // Set the shoot angle to the intercepting point
        shootAngle = AngleBetweenPoints(shootPoints[rotationState].position, aimPoint + Vector2.up * 0.1f);

        // Instantiate bullet and remove a item from the inventory
        GameObject proj = Instantiate(ammunition.projectile, shootPoints[rotationState].position, Quaternion.identity);
        proj.transform.localRotation = Quaternion.Euler(0,0, shootAngle - 90);
        container.PullItem(0, 1, out ContainedItem<Item> _);

        //noise
        SFXManager.PlayClip("shoot");

        // Apply shoot delay adn set shooting false so we can shoot again
        yield return new WaitForSeconds(reloadTime);
        shooting = false;
    }

    public void SortTargets()
    {

        // Remove all defeated enemies
        for (int i = targetEnemies.Count - 1; i >= 0; i--) {
            if (targetEnemies[i].state ==  CowEnemy.EnemyState.eating)
                targetEnemies.RemoveAt(i);
        }

        // Check if there are enemies left
        if (targetEnemies.Count <= 0)
            return;

        // Default the closest enemy to not exist
        CowEnemy closest = null;
        float closestDist = 99;

        // Loop over every Enemy and set it to the closest if it is the closest to the barn
        foreach (CowEnemy script in targetEnemies){
            if (script.state != CowEnemy.EnemyState.eating) {
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

    public void ReTriggerEnter(Collider2D coll)
    {
        if (!coll.CompareTag("Enemy"))
            return;

        targetEnemies.Add(coll.gameObject.GetComponent<CowEnemy>());
        SortTargets();
    }

    public void ReTriggerStay(Collider2D coll) { }

    public void ReTriggerExit(Collider2D coll)
    {
        if (!coll.CompareTag("Enemy"))
            return;

        targetEnemies.Remove(coll.gameObject.GetComponent<CowEnemy>());
        SortTargets();
    }
}
