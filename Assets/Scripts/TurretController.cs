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
    [SerializeField] private List<EnemyBase> targetEnemies = new List<EnemyBase>();
    [SerializeField] private float reloadTime;
    
    private SpriteRenderer turretsr;
    private Inventory inv;
    private int rotationState = 0;
    private bool shooting = false;
    private float shootAngle;

    void Start()
    {
        turretsr = turret.GetComponent<SpriteRenderer>();
        inv = GetComponent<Inventory>();
        inv.Open();
        inv.container.PushItem(ammunition, 99);
    }

    void Update()
    {
        // Get the Position of the mouse in world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        float spriteAngle = AngleBetweenPoints(turret.transform.position, targetEnemies[0].transform.position);
      
        // Mapping the angle to a value between 0-8 so we can get the sprite direction that faces the correct position
        int round = Mathf.RoundToInt(spriteAngle / 360 * 16);
        rotationState = round == 16 ? 0 : round;
        turretsr.sprite = sprite[rotationState];

        shootAngle = AngleBetweenPoints(shootPoints[rotationState].position, targetEnemies[0].transform.position + (Vector3)(Vector2.up * 0.25f));

        if (!shooting && targetEnemies[0] != null && inv.container.PeekAmount(0) > 0)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        shooting = true;
        GameObject proj = Instantiate(ammunition.projectile, shootPoints[rotationState].position, Quaternion.identity);
        inv.container.PullItem(0, 1, out ContainedItem<Item> _);
        proj.transform.localRotation = Quaternion.Euler(0,0, shootAngle - 90);
        yield return new WaitForSeconds(reloadTime);
        shooting = false;
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
}
