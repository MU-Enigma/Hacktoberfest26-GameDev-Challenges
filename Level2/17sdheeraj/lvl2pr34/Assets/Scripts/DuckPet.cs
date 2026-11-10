using UnityEngine;

public class DuckPet : MonoBehaviour
{
    public float moveSpeed = 4.5f;

    private Transform player;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        var p = GameObject.Find("Capybara");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;
        if (player == null) return;

        FoodItem nearestFood = null;
        float minDist = 3.5f;
        var foods = FindObjectsOfType<FoodItem>();
        foreach (var f in foods)
        {
            float d = Vector2.Distance(transform.position, f.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearestFood = f;
            }
        }

        Vector3 targetPos = player.position - new Vector3(0.8f, 0.3f, 0f);
        if (nearestFood != null)
        {
            targetPos = nearestFood.transform.position;
            if (Vector2.Distance(transform.position, nearestFood.transform.position) <= 0.6f)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.AddScore(nearestFood.points);
                }
                Destroy(nearestFood.gameObject);
            }
        }

        Vector2 dir = (targetPos - transform.position);
        if (dir.magnitude > 0.2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (sr != null)
            {
                sr.flipX = dir.x < 0;
            }
        }

        float waddle = Mathf.Sin(Time.time * 8f) * 0.05f;
        transform.rotation = Quaternion.Euler(0, 0, waddle * 30f);
    }
}

