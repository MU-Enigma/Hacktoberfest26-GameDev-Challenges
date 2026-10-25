
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 14f;
    public float burgerBounceForce = 10f;

    [Header("Visual Feedback")]
    public Color normalColor = Color.green;
    public Color activatedColor = Color.yellow;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        bool isPlayer = hitObject.CompareTag("Player");
        bool isBurger = hitObject.name == "Spawned Burger";

        if (!isPlayer && !isBurger)
            return;

        Rigidbody2D rb = hitObject.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        float force = isPlayer ? bounceForce : burgerBounceForce;

        // Bounce the player or burger upward.
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            force
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = activatedColor;
            CancelInvoke(nameof(ResetColor));
            Invoke(nameof(ResetColor), 0.25f);
        }

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
