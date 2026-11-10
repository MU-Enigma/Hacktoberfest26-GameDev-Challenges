using UnityEngine;

public class PondTorpedo : MonoBehaviour
{
    public float dashSpeed = 16f;
    public float dashDuration = 0.35f;

    [HideInInspector]
    public bool isDashing = false;

    private float dashTimer = 0f;
    private Vector2 dashDir;
    private Rigidbody2D rb;
    private CapybaraBath bath;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bath = GetComponent<CapybaraBath>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
            return;
        }

        bool nearPond = bath != null && bath.isBathing;
        if (nearPond && Input.GetKeyDown(KeyCode.Space))
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing && rb != null)
        {
            rb.MovePosition(rb.position + dashDir * dashSpeed * Time.fixedDeltaTime);
        }
    }

    public void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(h, v).normalized;

        if (inputDir == Vector2.zero)
        {
            float dirX = (sr != null && sr.flipX) ? -1f : 1f;
            dashDir = new Vector2(dirX, 0f);
        }
        else
        {
            dashDir = inputDir;
        }

        if (bath != null)
        {
            bath.StopBathing();
        }
    }
}
