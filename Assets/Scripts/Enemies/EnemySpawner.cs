using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawn {
        public GameObject prefab;
        public int startRound;
        public float weight;
        [HideInInspector] public Vector2 dropRange;
    }

    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    [SerializeField] int deliveryTruckWave;

    [SerializeField] private GameObject deliveryTruck;
    private DeliveryTruck truckScript;
    
    [Header("Wave")]
    public int waves;
    public float waveDelay;

    [Header("Spawn")]
    public List<Vector2> spawnAngles;
    public float spawnRadius;

    [Header("Group")]
    public GameObject groupIndicator;
    public Vector2 groupDelay;
    public float groupRadius;
    public Vector2 groupSize;

    public int groups;

    private int currentWave = 0;
    [SerializeField] private float totalWeight = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnGameStarted()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop() {
        yield return new WaitForSeconds(5);
        while (true) {
            currentWave++;

            // Create Spawn weights and assign it to the enemies
            totalWeight = 0;
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].startRound <= currentWave) {
                    enemies[i].dropRange.x = totalWeight;
                    totalWeight += enemies[i].weight;
                    enemies[i].dropRange.y = totalWeight;
                }
            }

            // Every thenth wave we spawn the delivery truck and wait until its finished
            /*
            if (currentWave % deliveryTruckWave == 0) {
                yield return new WaitForSeconds(8f);
                truckScript = Instantiate(deliveryTruck).GetComponent<DeliveryTruck>();
                yield return new WaitUntil(() => truckScript.isFinished == true);
            }*/

            // Add a small amount of delay between each Wave
            yield return new WaitForSeconds(waveDelay);

            // Spawn every group with a random delay
            for (int i = 0; i < groups; i++) {
                float delay = Random.Range(groupDelay.x, groupDelay.y);
                SpawnGroup();
                yield return new WaitForSeconds(delay);
            }
            
            // Add a small amount to the Wave delay
            waveDelay += 2;

            // Group min and max size progression calculation
            float v = 1.5f; 
            float t = 3.5f;
            float p;

            groupSize.x = Mathf.RoundToInt(Mathf.Pow(Mathf.Sqrt(currentWave), 1.25f) / t) + 1;
            groupSize.y = Mathf.RoundToInt(Mathf.Pow(Mathf.Sqrt(currentWave), 1.3f) / v) + 1;

            // Group count progression calculation
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
            Instantiate(RandomEnemy().prefab, groupPoint + (Random.insideUnitCircle * Random.Range(0, groupRadius)), Quaternion.identity);
        }
    }

    private EnemySpawn RandomEnemy()
    {
        // Set default index and "Roll" number to get a match
        int index = -1;
        float number = Random.Range(1, totalWeight);

        // Loop over table to find match
        for (int i = 0; i < enemies.Count; i++)
        {
            if (number >= enemies[i].dropRange.x && number <= enemies[i].dropRange.y)
                index = i;
        }

        // Return null or the correct enemy to spawn
        return index < 0 ? null : enemies[index];
    }
}
