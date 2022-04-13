using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarEnemy : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private GameObject targetObject;

    float nextWaypointDistance = 1f;
    int currentWaypoint;
    bool reachedEndOfPath = false;

    Rigidbody2D rb;
    [SerializeField] List<Vector2> pathList = new List<Vector2>();

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        pathList = EnemyPathFinding.GeneratePathList(transform.position, targetObject.transform.position, Mathf.CeilToInt(transform.localScale.x));
    }

    private void Update()
    {
        reachedEndOfPath = false;

        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector2.Distance(transform.position, pathList[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < pathList.Count) {
                    currentWaypoint++;
                }
                else {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
                break;
        }

        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
        Vector2 dir = (pathList[currentWaypoint] - (Vector2)transform.position).normalized;
        Vector2 velocity = dir * movementSpeed * speedFactor;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
