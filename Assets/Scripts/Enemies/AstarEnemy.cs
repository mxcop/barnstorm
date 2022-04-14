using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarEnemy : MonoBehaviour
{
    [Header("Astar")]
    [SerializeField] protected int collisionSize;
    [SerializeField] protected float movementSpeed;

    protected Vector2 target;
    protected Vector2 spawnPosition;

    [HideInInspector] public Vector2 velocity;

    protected float nextWaypointDistance = 1f;
    protected int currentWaypoint;
    protected bool reachedEndOfPath = false;

    [HideInInspector] public List<Vector2> pathList = new List<Vector2>();

    protected virtual void Start() {
        // Get a generated path from the EnemyPathfinding script
        pathList = EnemyPathFinding.GeneratePathList(transform.position, target, collisionSize);
    }

    /// <summary>
    /// Moves the enemy towards the targetObject with the generated pathlist
    /// </summary>
    protected void FollowPath() {
        reachedEndOfPath = false;
        float distanceToWaypoint;

        while (true) {
            // Check if we are following the correct waypoint
            distanceToWaypoint = Vector2.Distance(transform.position, pathList[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                // Go to the next waypoint if we can otherwise we reached the end
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

        // Makes it so we smoothly go from and to waypoints
        float speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

        // Gets the direction to the current waypoint
        Vector2 dir = (pathList[currentWaypoint] - (Vector2)transform.position).normalized;

        // Applies the direction and smooting with the movementspeed
        velocity = dir * movementSpeed * speedFactor;

        // Moves the enemy
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    /// <summary>
    /// Update the pathList but instead of the targetObject being the target, the spawnPosition is the target
    /// </summary>
    protected void ChangeToRetreatPath() {
        // Get a new generated path from the EnemyPathfinding script with the spawnposition as target and reset the current waypoint
        target = spawnPosition;
        pathList = EnemyPathFinding.GeneratePathList(transform.position, spawnPosition, collisionSize);
        currentWaypoint = 0;
    }
}
