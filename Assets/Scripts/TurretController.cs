using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    public Food ammunition;

    [SerializeField] private Sprite[] sprite;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject targetEmemy;

    private SpriteRenderer turretsr;
    private int rotationState = 0;
    private bool shooting = false;
    private float shootAngle;

    void Start()
    {
        turretsr = turret.GetComponent<SpriteRenderer>();   
    }

    void Update()
    {
        // Get the Position of the mouse in world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        shootAngle = AngleBetweenPoints(turret.transform.position, mousePos);

        // Mapping the angle to a value between 0-8 so we can get the sprite direction that faces the correct position
        int round = Mathf.RoundToInt(shootAngle / 360 * 16);
        turretsr.sprite = sprite[round == 16 ? 0 : round];

        if (!shooting && targetEmemy != null)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        shooting = true;
        GameObject proj = Instantiate(ammunition.projectile, turret.transform.position, Quaternion.identity);
        proj.transform.localRotation = Quaternion.Euler(0,0, shootAngle - 90);
        Debug.Log(proj.transform.eulerAngles);
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
