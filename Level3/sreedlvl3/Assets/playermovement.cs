using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D body;

    // Jump settings
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Dash settings
    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    private Vector2 movementDirection;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");

        movementDirection = new Vector2(xInput, 0f).normalized;

        // SPACE = Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isDashing)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );
        }

        // SHIFT = Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            !isDashing &&
            dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
        }

        // Dash cooldown
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            body.linearVelocity = new Vector2(
                movementDirection.x * speed,
                body.linearVelocity.y
            );
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = movementDirection;

        // If standing still, dash in facing direction
        if (dashDirection == Vector2.zero)
        {
            dashDirection = transform.localScale.x >= 0
                ? Vector2.right
                : Vector2.left;
        }

        body.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        body.linearVelocity = new Vector2(
            0f,
            body.linearVelocity.y
        );
    }
}