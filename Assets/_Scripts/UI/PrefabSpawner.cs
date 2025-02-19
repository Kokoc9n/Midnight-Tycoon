using System.Collections;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabs;
    [SerializeField] float[] spawnTimes;

    public float gameTime; //////

    void Start()
    {
        gameTime = Random.Range(0f, 24f);
        SpawnPrefabBasedOnTime();
    }

    void SpawnPrefabBasedOnTime()
    {
        float[] weights = new float[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            float diff = Mathf.Abs(gameTime - spawnTimes[i]);
            if (diff > 12f)
            {
                diff = 24f - diff; // Adjust for circular time (wrap-around at 0/24)
            }

            weights[i] = 1f / (1f + diff); // Inversely proportional to the time difference
        }

        // Normalize the weights so they sum up to 1
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] /= totalWeight;
        }

        // Randomly select prefab based on the weighted probabilities
        float randomValue = Random.Range(0f, 1f);
        float cumulativeWeight = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                Instantiate(prefabs[i], transform.position, Quaternion.identity); // Spawn the prefab
                break;
            }
        }
    }
}
