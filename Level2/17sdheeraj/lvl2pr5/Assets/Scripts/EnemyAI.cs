using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 3f;
    public Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector3 spawnPoint;
    private Vector2 currentVelocity;
    private float pauseTimer = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        spawnPoint = transform.position;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0 && sr != null)
        {
            sr.flipX = true;
        }
        else if (direction.x < 0 && sr != null)
        {
            sr.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        if (pauseTimer > 0)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 targetDir = ((Vector2)player.position - rb.position).normalized;
        currentVelocity = Vector2.Lerp(currentVelocity, targetDir * moveSpeed, turnSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Capybara"))
        {
            collision.gameObject.transform.position = Vector3.zero;
            transform.position = spawnPoint;
            currentVelocity = Vector2.zero;
            pauseTimer = 1f;
        }
    }
}
