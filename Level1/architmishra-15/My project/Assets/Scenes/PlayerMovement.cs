using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isInverted = false;
    private bool isPancaked = false;

    void Start()
    {
        // Grab the Rigidbody2D component automatically
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G)) {
            rb.gravityScale = rb.gravityScale < 0 ? 0f : -1f; // Toggles float on and off
        }

        if (Input.GetKeyDown(KeyCode.R)) isInverted = !isInverted;

        if (Input.GetKeyDown(KeyCode.P)) {
            isPancaked = !isPancaked; // Toggle the state
            
            // Set Y scale to 0.2 if pancaked, or 1.0 (normal) if not
            float targetY = isPancaked ? 0.2f : 1f;
            transform.localScale = new Vector3(transform.localScale.x, targetY, transform.localScale.z);
        }

        // Capture WASD or Arrow Key inputs (-1, 0, or 1)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Normalize vector to prevent faster diagonal movement
        movement = movement.normalized;

        if (isInverted) movement *= -1;
    }

    void FixedUpdate()
    {
        // Apply the movement velocity to the Rigidbody
        rb.linearVelocity = movement * moveSpeed;
    }
}