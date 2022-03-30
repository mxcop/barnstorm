using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;

    [Header("Wave")]
    public int waves;
    public float waveDelay;

    [Header("Spawn")]
    public List<Vector2> spawnAngles;
    public float spawnRadius;

    [Header("Group")]
    public float groupRadius;
    public Vector2 groupSize;

    public int groups;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        for (int j = 0; j < waves; j++)
        {
            yield return new WaitForSeconds(waveDelay);
            for (int i = 0; i < groups; i++)
                SpawnGroup();

            waveDelay += 10;
            groupSize.x++;
            groupSize.y++;
            groups++;
        }
        yield return null;
    }

    void SpawnGroup()
    {
        // Random Angle
        int randomIndex = Random.Range(0, spawnAngles.Count);
        float angle = Random.Range(spawnAngles[randomIndex].x, spawnAngles[randomIndex].y);

        // Generate vector from angle
        Vector2 groupPoint = new Vector2(Mathf.Sin(angle * Mathf.PI / 180), Mathf.Cos(angle * Mathf.PI / 180)) * spawnRadius;

        // Random group size
        int randomSize = Random.Range((int)groupSize.x, (int)groupSize.y);

        // Spawn the generated Group
        for (int i = 0; i < randomSize; i++)
        {
            Instantiate(enemyPrefabs[0], groupPoint + (Random.insideUnitCircle * Random.Range(0, groupRadius)), Quaternion.identity);
        }
    }
}
