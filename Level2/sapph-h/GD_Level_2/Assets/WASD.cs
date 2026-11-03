using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioClip quackSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
    }

    void Quack()
    {
        if (quackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(quackSound);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}