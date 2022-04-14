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
    [SerializeField] [Tooltip("The collison layer that the enemies will avoid")] static LayerMask collisionLayer = 1;
    [SerializeField] [Tooltip("The amount of times 1 tile will be itterated uppon")] static int stepSize = 2;
    [SerializeField] [Tooltip("The radius the algorithm will work with from 0,0")] static int collisionRange = 30;

    static int[][] collisionMap;

    void Awake() {
        PopulateCollisionMap();
    }

    /// <summary>Generates a path with Astar pathfinding</summary>
    /// <param name="start">The position the pathfinding starts from</param>
    /// <param name="end">The position the pathfinding ends on</param>
    /// <param name="radius">The extra border radius on the collision map</param>
    /// <returns>Returns a scaled and acurate list with all the positions to make a path from the start to the end</returns>
    public static List<Vector2> GeneratePathList(Vector2 start, Vector2 end, int radius = 1) {
        int[][] map = copyMap(collisionMap);

        // Loop the amount of times the extra radius needs to be aplied
        for (int t = 0; t < radius; t++) {
            int[][] oldMap = copyMap(map);
            // Add a extra border radius to the collision map
            for (int i = 0; i < oldMap.Length; i++) {
                for (int j = 0; j < oldMap[i].Length; j++) {
                    // Check if we are on a collision point
                    if (oldMap[i][j] == 1) {
                        // If we are not on a Vertical border
                        if (i != 0 && i != oldMap.Length - 1) {
                            // Add a extra collision point with a vertical offset
                            map[i - 1][j] = 1;
                            map[i + 1][j] = 1;
                        }

                        // If we are not on a Horizontal border
                        if (j != 0 && j != oldMap.Length - 1) {
                            // Add a extra collision point with a Horizontal offset
                            map[i][j - 1] = 1;
                            map[i][j + 1] = 1;
                        }
                    }
                }
            }
        }

        // Generate unscaled Path using Astar
        List<Vector2Int> resultInt = new Astar(Astar.ConvertToBoolArray(map), 
            new Vector2Int((int)(start.x * stepSize + collisionRange * stepSize), (int)(start.y * stepSize + collisionRange * stepSize)), 
            new Vector2Int((int)(end.x * stepSize + collisionRange * stepSize), (int)(end.y * stepSize + collisionRange * stepSize)), 
            Astar.Type.DiagonalFree).Result;

        // Scale and convert Path to List<Vector2>
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < resultInt.Count; i++) {
            result.Insert(0, new Vector2((float)(resultInt[i].x / (float)stepSize - collisionRange), (float)(resultInt[i].y / (float)stepSize - collisionRange)));
        }

        result.Reverse();

        return result;
    }

    /// <summary>Makes it simple to create 2d int arrays and copy them</summary>
    /// <param name="m">The map that you want to copy</param>
    /// <returns>The copied map</returns>
    private static int[][] copyMap(int[][] m) {
        int[][] nm = new int[m.Length][];
        for (int i = 0; i < m.Length; i++) {
            nm[i] = new int[m[i].Length];
            for (int j = 0; j < m[i].Length; j++) {
                nm[i][j] = m[i][j];
            }
        }

        return nm;
    }

    /// <summary>
    /// This method creates and updates the collision map in the form of a 2d int array
    /// </summary>
    public static void PopulateCollisionMap() {
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
    /// A temporary method to display the A* path in the scene
    /// </summary>
    void DisplayResults(List<Vector2Int> result) {
        for (int i = 0; i < result.Count; i++)
        {
            Instantiate(resultObj, new Vector2((float)(result[i].x / (float)stepSize - collisionRange), (float)(result[i].y / (float)stepSize - collisionRange)), Quaternion.identity);
        }
    }
}
