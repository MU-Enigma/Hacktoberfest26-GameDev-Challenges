using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioClip quackSound;
    public AudioClip fartSound;
    private AudioSource audioSource;

    [Header("Abilities")]
    public float disappearDuration = 2f;
    private bool isDisappeared = false;
    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] colliders;
    private Vector3 initialPosition;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();

        // Set up AudioSource for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        audioSource.spatialBlend = 0f; // 2D sound

#if UNITY_EDITOR
        // Auto-load audio clips if not explicitly assigned in Inspector
        if (quackSound == null)
        {
            quackSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/quack-sound-effect.mp3");
        }
        if (fartSound == null)
        {
            fartSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/fart-sound-effect.mp3");
        }
#endif
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Prevent diagonal movement from being faster
        movement = movement.normalized;

        // Play quack sound when pressing Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Quack();
        }

        // Play fart sound when pressing F
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fart();
        }

        // Disappear for 2 seconds when pressing X
        if (Input.GetKeyDown(KeyCode.X) && !isDisappeared)
        {
            StartCoroutine(DisappearCoroutine());
        }

        // Teleport back to initial position (center) when pressing I
        if (Input.GetKeyDown(KeyCode.I))
        {
            TeleportToStart();
        }
    }

    public void TeleportToStart()
    {
        transform.position = initialPosition;
        if (rb != null)
        {
            rb.position = initialPosition;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
        }
    }

    void Quack()
    {
        if (quackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(quackSound);
        }
    }

    void Fart()
    {
        if (fartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fartSound);
        }
    }

    private IEnumerator DisappearCoroutine()
    {
        isDisappeared = true;
        SetVisible(false);

        yield return new WaitForSeconds(disappearDuration);

        SetVisible(true);
        isDisappeared = false;
    }

    private void SetVisible(bool visible)
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
        if (colliders == null || colliders.Length == 0)
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                sr.enabled = visible;
            }
        }

        foreach (var col in colliders)
        {
            if (col != null)
            {
                col.enabled = visible;
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}