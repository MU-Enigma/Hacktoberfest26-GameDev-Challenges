using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject goldenYuzuPrefab;
    public float spawnInterval = 4f;
    public float goldenInterval = 60f;
    public int maxFoodCount = 5;

    private float timer = 0f;
    private float goldenTimer = 0f;

    void Start()
    {
        SpawnFood();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            int currentCount = FindObjectsOfType<FoodItem>().Length;
            if (currentCount < maxFoodCount)
            {
                SpawnFood();
            }
        }

        goldenTimer += Time.deltaTime;
        if (goldenTimer >= goldenInterval)
        {
            goldenTimer = 0f;
            SpawnGoldenYuzu();
        }
    }

    public void SpawnGoldenYuzu()
    {
        if (goldenYuzuPrefab == null) return;
        float rx = Random.Range(-14f, 14f);
        float ry = Random.Range(-7f, 7f);
        Instantiate(goldenYuzuPrefab, new Vector3(rx, ry, 0f), Quaternion.identity);
    }

    public void SpawnFood()
    {
        if (foodPrefab == null) return;

        for (int i = 0; i < 20; i++)
        {
            float rx = Random.Range(-15.5f, 15.5f);
            float ry = Random.Range(-8.2f, 8.2f);
            Vector3 spawnPos = new Vector3(rx, ry, 0f);

            // Avoid pond area
            if (spawnPos.x > 7.0f && spawnPos.y > 2.0f) continue;

            Instantiate(foodPrefab, spawnPos, Quaternion.identity);
            break;
        }
    }

    public void ClearFood()
    {
        var foods = FindObjectsOfType<FoodItem>();
        foreach (var f in foods)
        {
            Destroy(f.gameObject);
        }
    }
}
