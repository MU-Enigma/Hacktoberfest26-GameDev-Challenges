using UnityEngine;

public class OctoMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 savedCheckpoint;

    private GameObject expandingInk;

    // We reference the chicken's script to know who is currently in control
    public PlayerMovement playerScript; 
    public Sprite originalOctoSprite;
    private float logTimer = 0f;
    private bool isLogActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Octo starts completely invisible and uncollidable
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        originalOctoSprite = GetComponent<SpriteRenderer>().sprite;
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

        if (Input.GetKeyDown(KeyCode.O)) {
            GameObject status = new GameObject("GitStatus");
            // Position it 2 units above the Octocat's center
            status.transform.position = transform.position + new Vector3(0, 2f, 0);
            status.transform.SetParent(transform); // Lock it to the Octocat so it moves with it
            
            TextMesh tm = status.AddComponent<TextMesh>();
            tm.text = "Working tree clean";
            tm.characterSize = 0.5f;
            tm.anchor = TextAnchor.MiddleCenter;
            
            Destroy(status, 1.5f); // Fade it out quickly
        }

        if (Input.GetKey(KeyCode.Space)) {
    // Spin rapidly around the Z axis (2D rotation)
    transform.Rotate(0, 0, 1080f * Time.deltaTime);
}
if (Input.GetKeyUp(KeyCode.Space)) {
    // Instantly snap the rotation back to default
    transform.rotation = Quaternion.identity;
}
    
        if (Input.GetKeyDown(KeyCode.H)) {
            // Check if it is currently stashed (scale 0)
            bool isStashed = transform.localScale.x == 0f;
            
            // Toggle between scale 8 (default) and scale 0 (hidden)
            transform.localScale = isStashed ? new Vector3(8f, 8f, 1f) : Vector3.zero;
            
            // Disable physics collisions while stashed
            GetComponent<Collider2D>().enabled = isStashed;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (movement != Vector2.zero) {
                // Instantly teleport/dash 4 units in the current walking direction
                transform.position += (Vector3)(movement * 4f);
            } else {
                // Default to a vertical upward dash if standing completely still
                transform.position += Vector3.up * 4f;
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && expandingInk == null) {
            // Generate a perfect circle using Unity's primitive sphere
            expandingInk = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            // Strip the 3D physics so it doesn't break your 2D game
            Destroy(expandingInk.GetComponent<Collider>()); 
            
            // Center it on the Octocat, but pull it forward on the Z-axis so it covers the map
            expandingInk.transform.position = transform.position + new Vector3(0, 0, -5f);
            
            // Apply a flat 2D shader to the 3D object to make it a solid, unlit black circle
            Renderer rend = expandingInk.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.black;
            
            // Start it completely invisible (size 0)
            expandingInk.transform.localScale = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.I) && expandingInk != null) {
            // Rapidly scale the circle outward in all directions while held
            expandingInk.transform.localScale += Vector3.one * 50f * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.I) && expandingInk != null) {
            // Erase the ink void immediately when the key is released
            Destroy(expandingInk);
        }

        if (Input.GetKeyDown(KeyCode.P) && playerScript != null) {
            SpriteRenderer mySr = GetComponent<SpriteRenderer>();
            SpriteRenderer targetSr = playerScript.GetComponent<SpriteRenderer>();
            
            // If we look like the Octocat, steal the Chicken's graphic and color tint
            if (mySr.sprite == originalOctoSprite) {
                mySr.sprite = targetSr.sprite;
                mySr.color = targetSr.color;
            } 
            // Otherwise, drop the disguise and revert to the Octocat
            else {
                mySr.sprite = originalOctoSprite;
                mySr.color = Color.white; 
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            // Check if currently normal size (scale 8). If so, shrink to 4. Otherwise, restore to 8.
            bool isNormalSize = transform.localScale.x >= 8f;
            transform.localScale = isNormalSize ? new Vector3(4f, 4f, 1f) : new Vector3(8f, 8f, 1f);
            
            // Double speed if shrunk, revert to normal if restored
            moveSpeed = isNormalSize ? 10f : 5f;
        }

        if (Input.GetKey(KeyCode.J)) {
            // Uses a sine wave to smoothly oscillate the scale between 6 and 10
            float pulse = 8f + Mathf.Sin(Time.time * 15f) * 2f; 
            transform.localScale = new Vector3(pulse, pulse, 1f);
        }
        if (Input.GetKeyUp(KeyCode.J)) {
            // Snap cleanly back to the default scale of 8
            transform.localScale = new Vector3(8f, 8f, 1f); 
        }

        if (Input.GetKey(KeyCode.N) && playerScript != null && originalOctoSprite != null) {
            // Swap sprites back and forth every couple of frames to create a strobe glitch
            GetComponent<SpriteRenderer>().sprite = (Time.frameCount % 4 < 2) 
                ? originalOctoSprite 
                : playerScript.GetComponent<SpriteRenderer>().sprite;
        }
        if (Input.GetKeyUp(KeyCode.N) && originalOctoSprite != null) {
            // Resolve the conflict and snap back to the Octocat
            GetComponent<SpriteRenderer>().sprite = originalOctoSprite;
        }

        if (Input.GetKeyDown(KeyCode.L) && playerScript != null) {
            GameObject blameLaser = new GameObject("BlameLaser");
            LineRenderer lr = blameLaser.AddComponent<LineRenderer>();
            
            // Make the laser thin and red
            lr.startWidth = 0.05f; 
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red; 
            lr.endColor = Color.red;
            
            // Point it from the Octocat directly to the dormant Chicken
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, playerScript.transform.position);
            
            // Fade out and delete after 1 second
            Destroy(blameLaser, 1f);
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            isLogActive = !isLogActive; // Toggle the history trail on/off
        }

        // If active and the Octocat is actually moving, drop a green afterimage every 0.1 seconds
        if (isLogActive && movement != Vector2.zero) {
            logTimer += Time.deltaTime;
            if (logTimer >= 0.1f) {
                logTimer = 0f;
                
                // Create an empty visual ghost
                GameObject commitGhost = new GameObject("CommitGhost");
                commitGhost.transform.position = transform.position;
                commitGhost.transform.localScale = transform.localScale;
                
                // Copy the sprite and direction perfectly
                SpriteRenderer mySr = GetComponent<SpriteRenderer>();
                SpriteRenderer ghostSr = commitGhost.AddComponent<SpriteRenderer>();
                ghostSr.sprite = mySr.sprite;
                ghostSr.flipX = mySr.flipX;
                
                // Tint it GitHub green and fade it out
                ghostSr.color = new Color(0.2f, 1f, 0.2f, 0.4f); 
                ghostSr.sortingOrder = -1; // Push it behind the player
                
                // Delete the ghost automatically after half a second
                Destroy(commitGhost, 0.5f); 
            }
        }

        if (Input.GetKey(KeyCode.M)) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f);
            foreach(Collider2D hit in colliders) {
                Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                // If it has physics and isn't the Octocat, pull it in
                if (hitRb != null && hitRb != rb) {
                    Vector2 pullDir = transform.position - hit.transform.position;
                    // Using continuous Force to smoothly drag them
                    hitRb.AddForce(pullDir.normalized * 25f); 
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && playerScript != null) {
            // "Pull" the dormant main branch (Chicken) to your current location
            playerScript.transform.position = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            // Clone the Octocat exactly as it is
            GameObject branch = Instantiate(gameObject, transform.position, transform.rotation);
            
            // Strip its movement script and collider so it is just a ghost image
            Destroy(branch.GetComponent<OctoMovement>()); 
            Destroy(branch.GetComponent<Collider2D>()); 
            
            // Drop the opacity to 50%
            branch.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            
            // Auto-clean the branch after 2 seconds
            Destroy(branch, 2f);
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