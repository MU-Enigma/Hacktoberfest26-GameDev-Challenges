using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 3f;
    public float spawnInterval = 2.5f;
    public Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private Vector3 spawnPoint;
    private Vector2 currentVelocity;
    private float pauseTimer = 1f;
    private float respawnTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
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
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (isDead)
        {
            if (respawnTimer > 0f)
            {
                respawnTimer -= Time.deltaTime;
            }
            else
            {
                if (!IsCapybaraBusy())
                {
                    RespawnAtRandom();
                }
            }
            return;
        }

        Vector2 targetPosition = player != null ? (Vector2)player.position : (Vector2)transform.position;
        ThrownSnack targetSnack = FindObjectOfType<ThrownSnack>();
        if (targetSnack != null && targetSnack.isLanded)
        {
            targetPosition = targetSnack.transform.position;
        }

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        if (direction.x > 0 && sr != null)
        {
            sr.flipX = true;
        }
        else if (direction.x < 0 && sr != null)
        {
            sr.flipX = false;
        }
    }

    private bool IsCapybaraBusy()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                var capy = GameObject.Find("Capybara");
                if (capy != null) player = capy.transform;
            }
        }

        if (player == null) return false;

        var sleep = player.GetComponent<CapybaraSleep>();
        if (sleep != null && sleep.isSleeping) return true;

        var eat = player.GetComponent<CapybaraEat>();
        if (eat != null && eat.isEating) return true;

        return false;
    }

    void FixedUpdate()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;
        if (isDead) return;

        if (pauseTimer > 0)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        ThrownSnack targetSnack = FindObjectOfType<ThrownSnack>();
        if (targetSnack != null && targetSnack.isLanded)
        {
            Vector2 snackPos = targetSnack.transform.position;
            Vector2 snackDir = (snackPos - rb.position).normalized;

            currentVelocity = Vector2.Lerp(currentVelocity, snackDir * moveSpeed, turnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

            if (Vector2.Distance(rb.position, snackPos) <= 0.8f)
            {
                targetSnack.Eat();
                pauseTimer = 2.5f;
                currentVelocity = Vector2.zero;
            }
            return;
        }

        if (player == null || rb == null) return;

        Vector2 targetDir = ((Vector2)player.position - rb.position).normalized;
        currentVelocity = Vector2.Lerp(currentVelocity, targetDir * moveSpeed, turnSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("Capybara"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        respawnTimer = spawnInterval;

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
        currentVelocity = Vector2.zero;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(1);
        }
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        currentVelocity = Vector2.zero;
        pauseTimer = 1f;
        isDead = false;
        respawnTimer = 0f;

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }

    public void RespawnAtRandom()
    {
        Vector3 newPos = spawnPoint;
        for (int i = 0; i < 20; i++)
        {
            float rx = Random.Range(-7.5f, 7.5f);
            float ry = Random.Range(-4.0f, 4.0f);
            Vector3 testPos = new Vector3(rx, ry, 0f);

            if (player != null && Vector2.Distance(testPos, player.position) >= 4f)
            {
                newPos = testPos;
                break;
            }
        }

        transform.position = newPos;
        currentVelocity = Vector2.zero;
        pauseTimer = 0.5f;
        isDead = false;

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }
}
