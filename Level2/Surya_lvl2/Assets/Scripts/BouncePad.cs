
using UnityEngine;
using System.Collections;

public class BouncePad : MonoBehaviour
{
    IEnumerator CooldownRoutine()
    {
    yield return new WaitForSeconds(cooldownDuration);

    isOnCooldown = false;

    if (spriteRenderer != null)
        spriteRenderer.color = normalColor;

    Debug.Log("Bounce pad is ready again!");
    }
    [Header("Bounce Settings")]
    public float bounceForce = 14f;
    public float burgerBounceForce = 10f;

    [Header("Visual Feedback")]
    public Color normalColor = Color.green;
    public Color activatedColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private bool isOnCooldown = false;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 3f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    if (isOnCooldown)
        return;

    GameObject hitObject = collision.gameObject;

    bool isPlayer = hitObject.CompareTag("Player");
    bool isBurger = hitObject.name == "Spawned Burger";

    if (!isPlayer && !isBurger)
        return;

    Rigidbody2D rb = hitObject.GetComponent<Rigidbody2D>();

    if (rb == null)
        return;

    float force = isPlayer ? bounceForce : burgerBounceForce;

    rb.linearVelocity = new Vector2(
        rb.linearVelocity.x,
        force
    );

    if (spriteRenderer != null)
        spriteRenderer.color = activatedColor;

    isOnCooldown = true;
    StartCoroutine(CooldownRoutine());

    Debug.Log(isPlayer
        ? "Bounce pad activated by player!"
        : "Bounce pad activated by burger!");
}

    void ResetColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }
}
