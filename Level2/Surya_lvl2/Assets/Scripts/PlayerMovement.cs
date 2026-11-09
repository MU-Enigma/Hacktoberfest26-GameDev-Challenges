
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    IEnumerator SlowPlayer()
{
    isSlowed = true;
    moveSpeed = normalMoveSpeed * 0.5f;

    yield return new WaitForSeconds(10f);

    moveSpeed = normalMoveSpeed;
    isSlowed = false;
    slowCoroutine = null;
}
    private Coroutine slowCoroutine;
    private bool isSlowed = false;
    private float normalMoveSpeed;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.7f;
    public float jumpForce = 8f;

    [Header("Variable Jump Height")]
    public float jumpCutMultiplier = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 1f;

    [Header("Teleport")]
    public float teleportDistance = 5f;

    [Header("Special Ability")]
    public float specialDuration = 4f;
    public float specialSpeedMultiplier = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 originalScale;
    private int jumpsRemaining = 4;
    private bool isGrounded;
    private bool isDashing;
    private bool dashReady = true;
    private bool specialActive;

    void Awake()
    {
        normalMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Keyboard.current != null)
{
    bool shiftPressed =
        Keyboard.current.leftShiftKey.isPressed ||
        Keyboard.current.rightShiftKey.isPressed;

    if (shiftPressed && Keyboard.current.sKey.wasPressedThisFrame)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowPlayer());
    }
}
        ReadMovement();
        HandleAbilities();

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasReleasedThisFrame &&
            rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    void ReadMovement()
    {
        movement = Vector2.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
            movement.x = -1f;

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
            movement.x = 1f;

        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float speed = moveSpeed;

        if (Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed ||
             Keyboard.current.rightShiftKey.isPressed))
            speed *= sprintMultiplier;

        if (specialActive)
            speed *= specialSpeedMultiplier;

        rb.linearVelocity = new Vector2(
            movement.x * speed,
            rb.linearVelocity.y
        );
    }

    void HandleAbilities()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }

        bool shiftPressed =
        Keyboard.current.leftShiftKey.isPressed ||
        Keyboard.current.rightShiftKey.isPressed;

        if (Keyboard.current.sKey.isPressed &&
        !shiftPressed &&
        !isSlowed)

        {
            if (dashReady)
                StartCoroutine(Dash());
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
            Teleport();

        if (Keyboard.current.qKey.wasPressedThisFrame)
            StartCoroutine(SpecialAbility());

        if (Keyboard.current.rKey.wasPressedThisFrame &&
        !Keyboard.current.leftShiftKey.isPressed &&
        !Keyboard.current.rightShiftKey.isPressed)
            ChangeSize();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void CheckGround(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Ground")) return;

    foreach (ContactPoint2D contact in collision.contacts)
    {
        if (contact.normal.y > 0.5f)
        {
            isGrounded = true;
            jumpsRemaining = 4;
            return;
        }
    }
}

    IEnumerator Dash()
    {
        dashReady = false;
        isDashing = true;

        float direction = movement.x;

        if (direction == 0)
            direction = transform.localScale.x >= 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    void Teleport()
    {
        float direction = movement.x;

        if (direction == 0)
            direction = transform.localScale.x >= 0 ? 1f : -1f;

        Vector2 destination = rb.position +
            Vector2.right * direction * teleportDistance;

        rb.position = destination;
    }

    IEnumerator SpecialAbility()
    {
        if (specialActive) yield break;

        specialActive = true;
        yield return new WaitForSeconds(specialDuration);
        specialActive = false;
    }

    void ChangeSize()
    {
        if (transform.localScale.x == originalScale.x)
            transform.localScale = originalScale * 1.5f;
        else
            transform.localScale = originalScale;
    }
}
