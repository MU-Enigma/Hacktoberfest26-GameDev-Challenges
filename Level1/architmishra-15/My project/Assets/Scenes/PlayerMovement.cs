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

        // Matrix Slow-Mo (hold 1)
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // keep physics smooth
        }
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        if (Input.GetKey(KeyCode.LeftAlt)) {
            float s = Mathf.Clamp(transform.localScale.x + Input.mouseScrollDelta.y * 0.1f, 0.5f, 3f);
            transform.localScale = new Vector3(s, s, 1f);
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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            moveSpeed *= 2f; // Double speed when pressed
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            moveSpeed /= 2f; // Return to normal when released
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



        if (Input.GetKeyDown(KeyCode.Y)) {
            // Create an empty object and attach it above the character's head
            GameObject speech = new GameObject("SpeechBubble");
            speech.transform.position = transform.position + new Vector3(0, 1.5f, 0);
            speech.transform.SetParent(transform);

            // Add Unity's built-in Text component
            TextMesh textMesh = speech.AddComponent<TextMesh>();
            textMesh.text = "KAIZOKU OU!"; // Change this to whatever you want
            textMesh.characterSize = 0.5f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.white;

            // Remove the text automatically after 2 seconds
            Destroy(speech, 2f);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            // Create the laser object and attach it to the player
            GameObject laser = new GameObject("LaserBeam");
            laser.transform.position = transform.position;
            laser.transform.SetParent(transform);

            // Add and configure the LineRenderer to draw a red beam
            LineRenderer lr = laser.AddComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            // Draw the laser from the chicken to 10 units to the right
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position + new Vector3(10f, 0, 0));

            // Flash the laser for 0.5 seconds then delete it
            Destroy(laser, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 8f);
            foreach(Collider2D hit in colliders) {
                Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                // Pull objects toward the chicken
                if (hitRb != null && hitRb != rb) {
                    Vector2 pullDirection = transform.position - hit.transform.position;
                    hitRb.AddForce(pullDirection.normalized * 500f);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            // Toggles camera size between default (5) and zoomed out (10)
            Camera.main.orthographicSize = Camera.main.orthographicSize == 5f ? 10f : 5f;
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            Camera.main.backgroundColor = Color.white;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.I)) {
            // Unity's default solid background blue color
            Camera.main.backgroundColor = new Color(0.19f, 0.3f, 0.47f);
            GetComponent<SpriteRenderer>().enabled = true; 
        }

        if (Input.GetKey(KeyCode.U)) {
            // Offset the camera randomly on the X and Y axes
            Camera.main.transform.position = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -10f);
        }
        if (Input.GetKeyUp(KeyCode.U)) {
            // Snap back to the default 2D camera position
            Camera.main.transform.position = new Vector3(0, 0, -10f);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            // Toggles the camera's Z-rotation between 180 and 0
            float currentZ = Camera.main.transform.eulerAngles.z;
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, currentZ == 0 ? 180f : 0f);
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (movement.x > 0) {
            sr.flipX = false; // Facing right
        } else if (movement.x < 0) {
            sr.flipX = true; // Facing left
        }

        if (isInverted) movement *= -1;
    }

    void FixedUpdate()
    {
        // Apply the movement velocity to the Rigidbody
        rb.linearVelocity = movement * moveSpeed;
    }
}