using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    public Food ammunition;

    [SerializeField] private Sprite[] sprite;
    [SerializeField] private Transform[] shootPoints;
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject targetEmemy;
    [SerializeField] private float reloadTime;
    
    private SpriteRenderer turretsr;
    private int rotationState = 0;
    private bool shooting = false;
    [SerializeField] private float shootAngle;

    void Start()
    {
        turretsr = turret.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get the Position of the mouse in world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        float spriteAngle = AngleBetweenPoints(turret.transform.position, targetEmemy.transform.position);
      
        // Mapping the angle to a value between 0-8 so we can get the sprite direction that faces the correct position
        int round = Mathf.RoundToInt(spriteAngle / 360 * 16);
        rotationState = round == 16 ? 0 : round;
        turretsr.sprite = sprite[rotationState];

        shootAngle = AngleBetweenPoints(shootPoints[rotationState].position, targetEmemy.transform.position + (Vector3)(Vector2.up * 0.25f));

        if (!shooting && targetEmemy != null)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        shooting = true;
        GameObject proj = Instantiate(ammunition.projectile, shootPoints[rotationState].position, Quaternion.identity);
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
