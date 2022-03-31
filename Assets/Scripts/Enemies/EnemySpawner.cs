using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public bool canSpawn;
    public List<GameObject> enemyPrefabs;
    
    [Header("Wave")]
    public int waves;
    public float waveDelay;

    [Header("Spawn")]
    public List<Vector2> spawnAngles;
    public float spawnRadius;

    [Header("Group")]
    public GameObject groupIndicator;
    public float groupRadius;
    public Vector2 groupSize;

    public int groups;

    private int currentWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true) {
                currentWave++;
                yield return new WaitForSeconds(waveDelay);
                for (int i = 0; i < groups; i++)
                    SpawnGroup();

                waveDelay += 4;

                float v = 1.5f;
                float t = 3.5f;
                float p;

                groupSize.x = Mathf.RoundToInt(Mathf.Pow(Mathf.Sqrt(currentWave), 1.25f) / t) + 1;
                groupSize.y = Mathf.RoundToInt(Mathf.Pow(Mathf.Sqrt(currentWave), 1.3f) / v) + 1;

                v = 5000000f;
                p = 2.7f;
                t = 1600f;

                groups = 1 + Mathf.RoundToInt((Mathf.Sqrt(currentWave * v) + Mathf.Pow(currentWave, 2f) * p) / t);
        }
       
    }

    void SpawnGroup()
    {
        // Random Angle
        int randomIndex = Random.Range(0, spawnAngles.Count);
        float angle = Random.Range(spawnAngles[randomIndex].x, spawnAngles[randomIndex].y);

        // Generate the direction from angle
        Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.PI / 180), Mathf.Cos(angle * Mathf.PI / 180));

        // Create Group Indicator from the direciton and face it towards the enemies
        GameObject indicator = Instantiate(groupIndicator, direction * 7.5f, Quaternion.identity);
        indicator.transform.rotation = Quaternion.FromToRotation(Vector3.down, direction);

        // Calculate the groupPoint from the direction
        Vector2 groupPoint = direction* spawnRadius;

        // Random group size
        int randomSize = Random.Range((int)groupSize.x, (int)groupSize.y);

        // Spawn the generated Group
        for (int i = 0; i < randomSize; i++)
        {
            Instantiate(enemyPrefabs[0], groupPoint + (Random.insideUnitCircle * Random.Range(0, groupRadius)), Quaternion.identity);
        }
    }
}
