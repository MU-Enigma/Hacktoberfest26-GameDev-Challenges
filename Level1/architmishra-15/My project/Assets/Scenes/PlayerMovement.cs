using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isInverted = false;
    private bool isPancaked = false;
    public GameObject eggPrefab;

    void Start()
    {
        // Grab the Rigidbody2D component automatically
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Toggle gravity inversion with the G key
        if (Input.GetKeyDown(KeyCode.G)) {
            rb.gravityScale = rb.gravityScale < 0 ? 0f : -1f; // Toggles float on and off
        }

        // Movement inversion toggle with the R key
        if (Input.GetKeyDown(KeyCode.R)) isInverted = !isInverted;

        // flattening
        if (Input.GetKeyDown(KeyCode.P)) {
            isPancaked = !isPancaked; // Toggle the state
            
            // Set Y scale to 0.2 if pancaked, or 1.0 (normal) if not
            float targetY = isPancaked ? 0.2f : 1f;
            transform.localScale = new Vector3(transform.localScale.x, targetY, transform.localScale.z);
        }

        // Change color randomly with the C key
        if (Input.GetKeyDown(KeyCode.C)) {
            GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        }

        // Add torque with the B key and X to stop the rotation
        if (Input.GetKeyDown(KeyCode.B)) {
            rb.AddTorque(500f);
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            // Kill the spinning momentum
            rb.angularVelocity = 0f; 
            
            // Snap the rotation back to upright (0,0,0)
            transform.rotation = Quaternion.identity; 
        }

        // Trailing afterimage
        if (Input.GetKeyDown(KeyCode.T)) {
            TrailRenderer tr = gameObject.GetComponent<TrailRenderer>();
            if (tr == null) {
                tr = gameObject.AddComponent<TrailRenderer>();
                tr.time = 0.5f;
            } else {
                tr.enabled = !tr.enabled;
            }
        }

        // Egg Projectile with Space key
        if (Input.GetKeyDown(KeyCode.Space) && eggPrefab != null) {
            GameObject egg = Instantiate(eggPrefab, transform.position, Quaternion.identity);
            egg.AddComponent<Rigidbody2D>().AddForce(transform.right * 500f);
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