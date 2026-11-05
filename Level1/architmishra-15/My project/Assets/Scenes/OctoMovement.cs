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

        if (Input.GetKeyDown(KeyCode.F)) {
            // Check if currently normal size (scale 8). If so, shrink to 4. Otherwise, restore to 8.
            bool isNormalSize = transform.localScale.x >= 8f;
            transform.localScale = isNormalSize ? new Vector3(4f, 4f, 1f) : new Vector3(8f, 8f, 1f);
            
            // Double speed if shrunk, revert to normal if restored
            moveSpeed = isNormalSize ? 10f : 5f;
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