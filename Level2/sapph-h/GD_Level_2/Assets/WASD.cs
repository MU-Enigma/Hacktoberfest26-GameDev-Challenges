using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioClip quackSound;
    private AudioSource audioSource;

    [Header("Abilities")]
    public float disappearDuration = 2f;
    private bool isDisappeared = false;
    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] colliders;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        // Auto-load quack sound if not explicitly assigned in Inspector
        if (quackSound == null)
        {
            quackSound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/quack-sound-effect.mp3");
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

        // Disappear for 2 seconds when pressing X
        if (Input.GetKeyDown(KeyCode.X) && !isDisappeared)
        {
            StartCoroutine(DisappearCoroutine());
        }
    }

    void Quack()
    {
        if (quackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(quackSound);
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