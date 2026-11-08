using UnityEngine;

public class ThrownSnack : MonoBehaviour
{
    public float lifetime = 8f;
    public bool isLanded = false;

    private Rigidbody2D rb;
    private float landTimer = 0.35f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isLanded)
        {
            landTimer -= Time.deltaTime;
            if (landTimer <= 0f)
            {
                isLanded = true;
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }

    public void Launch(Vector2 force)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = force;
        }
    }

    public void Eat()
    {
        Destroy(gameObject);
    }
}

