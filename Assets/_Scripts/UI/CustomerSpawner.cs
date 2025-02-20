using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    private const float SPAWN_INTERVAL = 20;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] float[] spawnTimes;
    private float timer;
    void Start()
    {
        SpawnPrefabBasedOnTime();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= SPAWN_INTERVAL)
        {
            SpawnPrefabBasedOnTime();
            timer -= SPAWN_INTERVAL;
        }
    }

    void SpawnPrefabBasedOnTime()
    {
        float[] weights = new float[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            float diff = Mathf.Abs(TimeManager.Instance.GameTime - spawnTimes[i]);
            if (diff > 12f)
            {
                diff = 24f - diff;
            }

            weights[i] = 1f / (1f + diff);
        }

        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] /= totalWeight;
        }

        float randomValue = Random.Range(0f, 1f);
        float cumulativeWeight = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                Instantiate(prefabs[i]);
                break;
            }
        }
    }
}
