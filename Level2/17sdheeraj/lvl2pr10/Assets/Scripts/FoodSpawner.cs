using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public float spawnInterval = 4f;
    public int maxFoodCount = 3;

    private float timer = 0f;

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
    }

    public void SpawnFood()
    {
        if (foodPrefab == null) return;

        for (int i = 0; i < 15; i++)
        {
            float rx = Random.Range(-7f, 7f);
            float ry = Random.Range(-3.5f, 3.5f);
            Vector3 spawnPos = new Vector3(rx, ry, 0f);

            // Avoid pond area
            if (spawnPos.x > 3.0f && spawnPos.y < -0.5f) continue;

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
