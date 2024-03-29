using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private LevelSettings level;
    private LevelSettings.Wave currentWave { get { return level.waves[currentWaveIndex]; } }
    private int currentWaveIndex = 0;

    private const float groupRadius = 30;
    private float totalWeight = 0;

    public void StartWaves(LevelSettings l)
    {
        level = l;
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop() {
        // Delay Before the first round starts
        yield return new WaitForSeconds(5);

        // Loop until the last round
        while (currentWaveIndex != level.waves.Count) {
            // Create Spawn weights and assign it to the enemies
            totalWeight = 0;
            for (int i = 0; i < currentWave.enemies.Count; i++) {
                currentWave.enemies[i].dropRange.x = totalWeight;
                totalWeight += currentWave.enemies[i].weight;
                currentWave.enemies[i].dropRange.y = totalWeight;
            }

            // Add a small amount of delay between each Wave
            yield return new WaitForSeconds(currentWave.waveDelay);

            // Spawn every group with a random delay
            for (int i = 0; i < currentWave.groups; i++) {
                float delay = Random.Range(currentWave.groupDelay.x, currentWave.groupDelay.y);
                SpawnGroup();
                yield return new WaitForSeconds(delay);
            }

            currentWaveIndex++;
        }
    }

    void SpawnGroup() {
        // Random Angle
        float angle = Random.Range(0, 360);

        // Generate the direction from angle
        Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.PI / 180), Mathf.Cos(angle * Mathf.PI / 180));

        // Create Group Indicator from the direciton and face it towards the enemies
        // GameObject indicator = Instantiate(groupIndicator, direction * 7.5f, Quaternion.identity);
        // indicator.transform.rotation = Quaternion.FromToRotation(Vector3.down, direction);

        // Calculate the groupPoint from the direction
        Vector2 groupPoint = direction.normalized * groupRadius;

        // Random group size
        int randomSize = Random.Range((int)currentWave.groupSize.x, (int)currentWave.groupSize.y);

        // Spawn the generated Group
        for (int i = 0; i < randomSize; i++) {
            Instantiate(RandomEnemy().prefab, groupPoint + (Random.insideUnitCircle * Random.Range(0, 3)), Quaternion.identity);
        }
    }

    private LevelSettings.Enemy RandomEnemy() {
        // Set default index and "Roll" number to get a match
        int index = -1;
        float number = Random.Range(1, totalWeight);

        // Loop over table to find match
        for (int i = 0; i < currentWave.enemies.Count; i++) {
            if (number >= currentWave.enemies[i].dropRange.x && number <= currentWave.enemies[i].dropRange.y)
                index = i;
        }

        // Return null or the correct enemy to spawn
        return index < 0 ? null : currentWave.enemies[index];
    }
}
