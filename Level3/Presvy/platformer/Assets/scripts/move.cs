
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sprintMultiplier = 1.5f;
    public Rigidbody2D body;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;

    [Header("Jump")]
    public float jumpSpeed = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Teleport")]
    public Transform teleportDestination;

    [Header("Bird Poop")]
    public GameObject birdPoopPrefab;
    public Transform poopSpawnPoint;

    private float horizontalInput;
    private float facingDirection = 1f;
    private bool isSprinting;
    private bool jumpRequested;
    private bool wasGrounded;
    private bool isDashing;

    private int jumpsUsed = 2;
    private float nextDashTime;

    void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Sprite flipping
        if (horizontalInput != 0f)
        {
            facingDirection = Mathf.Sign(horizontalInput);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0f;
            }
        }

        // Sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Ground detection
        bool isGrounded =
            groundCheck != null &&
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            ) != null;

        if (isGrounded && !wasGrounded)
        {
            jumpsUsed = 0;
        }

        wasGrounded = isGrounded;

        // Jump and double jump
        if (Input.GetKeyDown(KeyCode.Space) &&
            jumpsUsed < 2 && !isDashing)
        {
            jumpRequested = true;
            jumpsUsed++;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.F) &&
            !isDashing &&
            Time.time >= nextDashTime)
        {
            StartCoroutine(Dash());
        }

        // Teleport
        if (Input.GetKeyDown(KeyCode.T) &&
            teleportDestination != null &&
            !isDashing)
        {
            body.position = teleportDestination.position;
            body.linearVelocity = Vector2.zero;
            wasGrounded = false;
        }

        // Spawn bird poop
        if (Input.GetKeyDown(KeyCode.E) &&
            birdPoopPrefab != null)
        {
            Vector3 spawnPosition =
                poopSpawnPoint != null
                ? poopSpawnPoint.position
                : transform.position + Vector3.down * 0.5f;

            GameObject poop = Instantiate(
                birdPoopPrefab,
                spawnPosition,
                Quaternion.identity
            );

            Destroy(poop, 5f);
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            body.linearVelocity = new Vector2(
                facingDirection * dashSpeed,
                0f
            );
            return;
        }

        float currentSpeed = speed;

        if (isSprinting)
        {
            currentSpeed *= sprintMultiplier;
        }

        body.linearVelocity = new Vector2(
            horizontalInput * currentSpeed,
            body.linearVelocity.y
        );

        if (jumpRequested)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpSpeed
            );

            jumpRequested = false;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        jumpRequested = false;
        nextDashTime = Time.time + dashCooldown;

        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;

        yield return new WaitForSeconds(dashDuration);

        body.gravityScale = originalGravity;
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}
