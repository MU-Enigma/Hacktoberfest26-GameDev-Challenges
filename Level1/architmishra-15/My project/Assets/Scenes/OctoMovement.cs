using UnityEngine;

public class OctoMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 savedCheckpoint;
    
    // We reference the chicken's script to know who is currently in control
    public PlayerMovement playerScript; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Octo starts completely invisible and uncollidable
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    void Update()
    {
        // If the chicken is the active character, ignore all Octo inputs
        if (playerScript != null && playerScript.isMainCharacter) {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // --- OCTOCAT MECHANICS GO HERE --- //
        if (Input.GetKeyDown(KeyCode.C)) {
            savedCheckpoint = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            // Snap back to the last saved checkpoint and kill falling momentum
            transform.position = savedCheckpoint;
            rb.linearVelocity = Vector2.zero;
        }

        // Standard Movement Capture
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Dynamic Sprite Flipping
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (movement.x > 0) sr.flipX = false;
        else if (movement.x < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // Only apply movement physics if the Octocat is in control
        if (playerScript != null && !playerScript.isMainCharacter) {
            rb.linearVelocity = movement * moveSpeed;
        }
    }
}