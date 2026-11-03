using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public GameObject eggPrefab;
    public GameObject obstaclePrefab;
    public Transform spawnPoint;

    void Start()
    {
#if UNITY_EDITOR
        // Auto-load prefabs if not explicitly assigned in Inspector
        if (eggPrefab == null)
        {
            eggPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/egg_0.prefab");
        }
        if (obstaclePrefab == null)
        {
            obstaclePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/obstacle_0.prefab");
            if (obstaclePrefab == null)
            {
                obstaclePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/obstacle.prefab");
            }
        }
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnEgg();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnObstacle();
        }
    }

    public void SpawnEgg()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        if (eggPrefab != null)
        {
            Instantiate(eggPrefab, position, Quaternion.identity);
        }
    }

    public void SpawnObstacle()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        if (obstaclePrefab != null)
        {
            Instantiate(obstaclePrefab, position, Quaternion.identity);
        }
        else
        {
            SpawnObstacleDynamic(position);
        }
    }

    private void SpawnObstacleDynamic(Vector3 position)
    {
        GameObject obstacle = new GameObject("obstacle_0");
        obstacle.transform.position = position;
        obstacle.transform.localScale = new Vector3(0.5569f, 0.9367f, 1f);

        SpriteRenderer sr = obstacle.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
        Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/obstacle.png");
        if (sprite != null)
        {
            sr.sprite = sprite;
        }
#endif

        Rigidbody2D rb = obstacle.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
    }
}
