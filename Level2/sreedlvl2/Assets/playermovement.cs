using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    public Rigidbody2D body;

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
        //float yInput = Input.GetAxis("Vertical");

        movementDirection = new Vector2(xInput, 0).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            body.linearVelocity = movementDirection * speed;
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        Vector2 dashDirection = movementDirection;

        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.right;
        }

        body.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        body.linearVelocity = Vector2.zero;
    }
}