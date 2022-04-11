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
    [SerializeField] [Tooltip("How many times a single tile will be devided into small chunks")] private float resolution;
    [SerializeField] [Tooltip("The radius the algorithm will work with from 0,0")] private int collisionRange;

    private int[][] collisionMap;

    void PopulateCollisionMap() {
        // Initialize the First dimension of the array and loop over it
        collisionMap = new int[collisionRange * 2][];
        for (int i = 0; i < collisionRange * 2; i++){

            // Initialize Second dimension of the array and loop over it
            collisionMap[i] = new int[collisionRange * 2];
            for (int j = 0; j < collisionRange * 2; j++){
                
                // If we detect a collision the collisionMap[i][j] = 1 otherwise its 0
                collisionMap[i][j] = Physics2D.OverlapCircle(new Vector2(j - collisionRange, i - collisionRange), 0.45f, collisionLayer) ? 1 : 0;
            }
        }
    }

    void DisplayMap() {
        // Loop over 2d int array
        for (int i = 0; i < collisionMap.Length; i++) {
            for (int j = 0; j < collisionMap[i].Length; j++) {

                // If it is a obstacle spawn the inactive object else spawn the active object
                if (collisionMap[i][j] == 1) {
                    Instantiate(obstacleObj, new Vector2(j - collisionRange, i - collisionRange), Quaternion.identity);
                } 
                else {
                    Instantiate(activeObj, new Vector2(j - collisionRange, i - collisionRange), Quaternion.identity);
                }     
            }
        }
    }

    void DisplayResults(List<Vector2Int> result) {
        for (int i = 0; i < result.Count; i++) {
            Instantiate(resultObj, new Vector2(result[i].x - collisionRange, result[i].y - collisionRange), Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(UpdateLoop());
    }

    IEnumerator UpdateLoop() {
        yield return new WaitForSeconds(1f);
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < objects.Length; i++) {
            Destroy(objects[i]);
        }
        PopulateCollisionMap();
        DisplayMap();
        DisplayResults(new Astar(Astar.ConvertToBoolArray(collisionMap), new Vector2Int((int)(start.position.x + collisionRange), (int)(start.position.y + collisionRange)), new Vector2Int((int)(end.position.x + collisionRange), (int)(end.position.y + collisionRange)), Astar.Type.DiagonalFree).Result);
        StartCoroutine(UpdateLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
