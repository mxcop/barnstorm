using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private GameObject resultObj;
    [SerializeField] private GameObject activeObj;
    [SerializeField] private GameObject obstacleObj;

    [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    [Header("Collision Map")]
    [SerializeField] [Tooltip("The collison layer that the enemies will avoid")] private LayerMask collisionLayer;
    [SerializeField] private int stepSize;
    [SerializeField] [Tooltip("The radius the algorithm will work with from 0,0")] private int collisionRange;

    private int[][] collisionMap;

    /// <summary>
    /// This method creates and updates the collision map in the form of a 2d int array
    /// </summary>
    void PopulateCollisionMap() {
        int rangeScale = collisionRange * 2 * stepSize;
        // Initialize the First dimension of the array and loop over it
        collisionMap = new int[rangeScale][];
        for (int i = 0; i < rangeScale; i++){

            // Initialize Second dimension of the array and loop over it
            collisionMap[i] = new int[rangeScale];
            for (int j = 0; j < rangeScale; j++){
                // If we detect a collision the collisionMap[i][j] = 1 otherwise its 0
                collisionMap[i][j] = Physics2D.OverlapCircle(new Vector2(j / (float)stepSize - collisionRange, i / (float)stepSize - collisionRange), 0.49f / stepSize, collisionLayer) ? 1 : 0;
            }
        }
    }

    /// <summary>
    /// A temporary method to display the obstacles and walkeble areas in the scene
    /// </summary>
    void DisplayMap() {
        // Loop over 2d int array
        for (int i = 0; i < collisionMap.Length; i++) {
            for (int j = 0; j < collisionMap[i].Length; j++) {

                // If it is a obstacle spawn the inactive object else spawn the active object
                if (collisionMap[i][j] == 1) {
                    Instantiate(obstacleObj, new Vector2((float)(j / (float)stepSize - collisionRange), (float)(i / (float)stepSize - collisionRange)), Quaternion.identity);
                } 
                else {
                    Instantiate(activeObj, new Vector2((float)(j / (float)stepSize - collisionRange), (float)(i / (float)stepSize - collisionRange)), Quaternion.identity);
                }     
            }
        }
    }

    /// <summary>
    /// A temporary method to display the A* path in the scene
    /// </summary>
    void DisplayResults(List<Vector2Int> result) {
        for (int i = 0; i < result.Count; i++) {
            //Instantiate(resultObj, new Vector2((float)(result[i].x - collisionRange) / stepSize, (float)(result[i].y - collisionRange) / stepSize), Quaternion.identity);
            Instantiate(resultObj, new Vector2((float)(result[i].x / (float)stepSize - collisionRange) , (float)(result[i].y / (float)stepSize - collisionRange)), Quaternion.identity);
        }
    }

    void Start() {
        StartCoroutine(UpdateLoop());
    }

    /// <summary>
    /// A temporary interface to update the map and path
    /// </summary>
    IEnumerator UpdateLoop() {
        yield return new WaitForSeconds(1f);
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < objects.Length; i++) {
            Destroy(objects[i]);
        }
        PopulateCollisionMap();
        DisplayMap();
        DisplayResults(new Astar(Astar.ConvertToBoolArray(collisionMap), new Vector2Int((int)(start.position.x * stepSize + collisionRange * stepSize), (int)(start.position.y * stepSize + collisionRange * stepSize)), new Vector2Int((int)(end.position.x * stepSize + collisionRange * stepSize), (int)(end.position.y * stepSize + collisionRange * stepSize)), Astar.Type.DiagonalFree).Result);
        StartCoroutine(UpdateLoop());
    }
}
