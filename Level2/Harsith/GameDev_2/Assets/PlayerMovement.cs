using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;

    Rigidbody2D rb;
    Vector2 move;
    Vector2 lastDir = Vector2.right; // dash direction if you're standing still
    bool isDashing;
    float dashTimeLeft;
    float cooldownLeft;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
        move = move.normalized;

        if (move != Vector2.zero) lastDir = move;

        // Flip left/right (delete this if you didn't add the flip earlier)
        if (move.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move.x) * Mathf.Abs(transform.localScale.x),
                                               transform.localScale.y, 1);
        }

        // Dash
        cooldownLeft -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && cooldownLeft <= 0f && !isDashing)
        {
            isDashing = true;
            dashTimeLeft = dashDuration;
            cooldownLeft = dashCooldown;
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f) isDashing = false;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isDashing ? dashSpeed : speed;
        Vector2 dir = isDashing ? lastDir : move;
        rb.MovePosition(rb.position + dir * currentSpeed * Time.fixedDeltaTime);
    }
}