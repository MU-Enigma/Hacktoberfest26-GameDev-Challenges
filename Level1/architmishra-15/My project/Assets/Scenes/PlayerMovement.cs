using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isInverted = false;
    private bool isPancaked = false;
    public GameObject eggPrefab;
    // private bool isGiant = false;
    private Vector3 originalScale;
    public GameObject katanaPrefab;
    public GameObject burgerPrefab;

    void Start()
    {

        // Grab the Rigidbody2D component automatically
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale; // Save the default size
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

        // Trailing afterimage with the T key
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

        // Decoy clone with N key
        if (Input.GetKeyDown(KeyCode.N)) {
            Instantiate(gameObject, transform.position, Quaternion.identity);
        }

        // remove the clones with K key
        if (Input.GetKeyDown(KeyCode.K)) {
            // Find all objects running this script
            PlayerMovement[] allChickens = FindObjectsOfType<PlayerMovement>();
            
            foreach (PlayerMovement chicken in allChickens) {
                // Only destroy the ones that are clones
                if (chicken.gameObject.name.Contains("(Clone)")) {
                    Destroy(chicken.gameObject);
                }
            }
        }

        // Katana projectile with F key
        if (Input.GetKeyDown(KeyCode.F) && katanaPrefab != null) {
            // Spawn it 1 unit to the right, angled at -45 degrees
            GameObject katana = Instantiate(katanaPrefab, transform.position + new Vector3(1f, 0, 0), Quaternion.Euler(0, 0, -45));
            // Add physics and shoot it
            katana.AddComponent<Rigidbody2D>().linearVelocity = new Vector2(15f, 0); 
            Destroy(katana, 2f); // Clean up the clone after 2 seconds
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) { // Mouse1 is Right-Click
            // Convert screen pixel coordinates to game world coordinates
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // Keep the chicken on the 2D plane
            transform.position = mouseWorldPos;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            // Find everything within a 5-unit circle
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);
            foreach(Collider2D hit in colliders) {
                Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                // If it has physics and isn't the player, blast it away
                if (hitRb != null && hitRb != rb) {
                    Vector2 pushDirection = hit.transform.position - transform.position;
                    hitRb.AddForce(pushDirection.normalized * 1000f);
                }
            }
        }

        // Burger projectile with J key
        if (Input.GetKeyDown(KeyCode.J) && burgerPrefab != null) {
        for (int i = -2; i <= 2; i++) {
            // Space them out horizontally by 1.5 units, spawn 5 units high
            Vector3 spawnPos = transform.position + new Vector3(i * 1.5f, 5f, 0);
            Instantiate(burgerPrefab, spawnPos, Quaternion.identity);
        }
    }

        // Giant mode toggle with M key
        if (Input.GetKeyDown(KeyCode.M)) {
            // If giant, double the original scale; if not, revert to original
            transform.localScale *= 2f;
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            // If giant, revert to original scale; if not, double the original scale
            transform.localScale = originalScale;
        }

        // Transport to origin
        if (Input.GetKeyDown(KeyCode.O)) {
            transform.position = Vector3.zero;
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