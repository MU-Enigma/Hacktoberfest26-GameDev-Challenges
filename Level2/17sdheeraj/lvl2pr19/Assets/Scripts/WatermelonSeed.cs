using UnityEngine;

public class WatermelonSeed : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3f;
    public GameObject hitPrefab;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("Crocodile"))
        {
            var ai = other.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.Die();
            }

            if (hitPrefab != null)
            {
                var fx = Instantiate(hitPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 0.25f);
            }

            Destroy(gameObject);
        }
    }
}
