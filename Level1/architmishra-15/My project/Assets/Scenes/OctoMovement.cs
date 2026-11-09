using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private SpriteRenderer sr;
    private List<GameObject> activeObstacles = new List<GameObject>();

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

        bool isCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool isAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        SpriteRenderer sr = GetComponent<SpriteRenderer>(); 
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

        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            // Generate a visible primitive block 
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Barricade";
            
            // FIX: Use DestroyImmediate so the 3D collider is instantly wiped before adding 2D ones
            DestroyImmediate(wall.GetComponent<Collider>());
            wall.AddComponent<BoxCollider2D>();
            
            Rigidbody2D wallRb = wall.AddComponent<Rigidbody2D>();
            wallRb.bodyType = RigidbodyType2D.Static; // No more NullRef here!
            
            // Position it in front of the Octocat and scale it up
            wall.transform.position = transform.position + (sr.flipX ? Vector3.left : Vector3.right) * 3f;
            wall.transform.localScale = new Vector3(2f, 5f, 1f);
            
            // Paint it gray with a flat 2D material
            Renderer rend = wall.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.gray;
            
            activeObstacles.Add(wall);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            // First, destroy all the barricades we tracked in the list
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) {
                    Destroy(obs);
                }
            }
            activeObstacles.Clear();

            // Next, do a full scene sweep for any other rogue physics debris
            foreach (Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)) {
                // Make sure it doesn't accidentally delete the Octocat or the Chicken!
                if (body.gameObject != gameObject && (playerScript == null || body.gameObject != playerScript.gameObject)) {
                    Destroy(body.gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            // Create a visible block
            GameObject anvil = GameObject.CreatePrimitive(PrimitiveType.Cube);
            anvil.name = "Anvil";
            
            // Wipe 3D physics instantly and add 2D physics
            DestroyImmediate(anvil.GetComponent<Collider>());
            anvil.AddComponent<BoxCollider2D>();
            
            // Make it heavy and lock its rotation so it drops perfectly straight
            Rigidbody2D anvilRb = anvil.AddComponent<Rigidbody2D>();
            anvilRb.mass = 500f; 
            anvilRb.constraints = RigidbodyConstraints2D.FreezeRotation; // FIX: Prevents tumbling
            
            // FIX: Position it exactly 15 units directly above the Octocat's head
            anvil.transform.position = new Vector3(transform.position.x, transform.position.y + 15f, 0f);
            anvil.transform.localScale = new Vector3(3f, 3f, 1f);
            
            // Paint it pitch black
            Renderer rend = anvil.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.black;
            
            // Track it for the garbage collector and auto-destroy it after 5 seconds
            activeObstacles.Add(anvil);
            Destroy(anvil, 5f);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            // Generate a visible primitive block instead of an empty object
            GameObject voidTrap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            voidTrap.name = "VoidTrap";
            
            // FIX: Wipe 3D physics instantly before adding the 2D trigger
            DestroyImmediate(voidTrap.GetComponent<Collider>());
            BoxCollider2D col = voidTrap.AddComponent<BoxCollider2D>();
            col.isTrigger = true; // Objects pass into it, triggering deletion
            
            // Attach your custom deletion script
            voidTrap.AddComponent<VoidTrapLogic>(); 
            
            // Position it at the Octocat's feet and scale it into a wide, flat trap
            voidTrap.transform.position = transform.position + new Vector3(0, -1f, 0);
            voidTrap.transform.localScale = new Vector3(4f, 0.5f, 1f);
            
            // Paint it pitch black using the 2D shader
            Renderer rend = voidTrap.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.black;
            
            // Track it so Keypad 5 can clean it up later
            activeObstacles.Add(voidTrap);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = "JumpPad";
            
            DestroyImmediate(pad.GetComponent<Collider>());
            BoxCollider2D col = pad.AddComponent<BoxCollider2D>();
            
            Rigidbody2D padRb = pad.AddComponent<Rigidbody2D>();
            padRb.bodyType = RigidbodyType2D.Static; // Immovable
            
            // Position it at Octocat's feet (scale 8 means offset down by 4)
            pad.transform.position = transform.position + Vector3.down * 20f;
            pad.transform.localScale = new Vector3(8f, 8f, 1f);
            
            Renderer rend = pad.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.green;
            
            PhysicsMaterial2D bouncyMat = new PhysicsMaterial2D("Bouncy") { bounciness = 2.5f };
            col.sharedMaterial = bouncyMat;
            
            activeObstacles.Add(pad);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) Destroy(obs);
            }
            activeObstacles.Clear();

            foreach (Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)) {
                if (body.gameObject != gameObject && (playerScript == null || body.gameObject != playerScript.gameObject)) {
                    Destroy(body.gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad6)) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 20f);
            foreach(Collider2D hit in hits) {
                Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                
                if (hitRb != null) {
                    // 1. Turn off engine gravity for this object
                    hitRb.gravityScale = 0f;
                    
                    // 2. Instantly kill all its existing falling momentum
                    hitRb.linearVelocity = Vector2.zero;
                    
                    // 3. Give it a tiny, slow upward drift to sell the zero-G effect
                    hitRb.AddForce(Vector2.up * (hitRb.mass * 2f), ForceMode2D.Impulse);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad7)) {
            transform.position += Vector3.down * 5f; 
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 15f);
            foreach(Collider2D hit in hits) {
                Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                if (hitRb != null && hitRb != rb) {
                    hitRb.linearVelocity = Vector2.zero;
                    hitRb.AddForce(Vector2.up * 800f, ForceMode2D.Impulse);
                }
            }
        }

        // Git revert / Time Skip (Ctrl + Z)
        if (isCtrl && Input.GetKeyDown(KeyCode.Z)) {
            transform.position -= new Vector3(Vector2.Distance(Vector2.zero, transform.position) > 5f ? 5f : -5f, 0, 0);
        }

        // git interrupt / Freeze State (Ctrl + C)
        if (isCtrl && Input.GetKeyDown(KeyCode.C)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) obs.GetComponent<Rigidbody2D>().simulated = !obs.GetComponent<Rigidbody2D>().simulated;
            }
        }

        // git patch / Paste Last (Ctrl + V)
        if (isCtrl && Input.GetKeyDown(KeyCode.V) && activeObstacles.Count > 0) {
            GameObject last = activeObstacles[^1];
            if (last != null) activeObstacles.Add(Instantiate(last, transform.position + Vector3.up * 4f, Quaternion.identity));
        }

        // git branch / Decoy Clone (Ctrl + B)
        if (isCtrl && Input.GetKeyDown(KeyCode.B)) {
            GameObject decoy = Instantiate(gameObject, transform.position + Vector3.right * 3f, Quaternion.identity);
            DestroyImmediate(decoy.GetComponent<OctoMovement>()); // Strip inputs
            decoy.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); // Make it a ghost
            decoy.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 1000f, ForceMode2D.Impulse);
            Destroy(decoy, 3f);
        }

        // git prune / Random Deletion (Ctrl + X)
        if (isCtrl && Input.GetKeyDown(KeyCode.X)) {
            for (int i = activeObstacles.Count - 1; i >= 0; i--) {
                if (activeObstacles[i] != null && Random.value > 0.5f) {
                    Destroy(activeObstacles[i]);
                    activeObstacles.RemoveAt(i);
                }
            }
        }

        if (isAlt && Input.GetKeyDown(KeyCode.DownArrow)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) obs.transform.localScale = new Vector3(obs.transform.localScale.x, 0.2f, 1f);
            }
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.S) && activeObstacles.Count > 0) {
            GameObject target = activeObstacles[0];
            if (target != null) {
                Vector3 temp = transform.position;
                transform.position = target.transform.position;
                target.transform.position = temp;
            }
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.P)) {
            GameObject closest = null;
            float minDist = Mathf.Infinity;
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) {
                    float dist = Vector2.Distance(transform.position, obs.transform.position);
                    if (dist < minDist) { minDist = dist; closest = obs; }
                }
            }
            if (closest != null) closest.transform.position = transform.position + Vector3.up * 3f;
        }

        if (isAlt && Input.GetKeyDown(KeyCode.R)) {
            foreach (GameObject obs in activeObstacles) if (obs != null) Destroy(obs);
            activeObstacles.Clear();
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.D)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) obs.GetComponent<Renderer>().material.color = Random.value > 0.5f ? Color.red : Color.green;
            }
        }

        if (isAlt && Input.GetKeyDown(KeyCode.M)) {
            Vector3 mergePoint = transform.position + Vector3.up * 8f;
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) {
                    Rigidbody2D rb2 = obs.GetComponent<Rigidbody2D>();
                    rb2.linearVelocity = Vector2.zero;
                    rb2.AddForce((mergePoint - obs.transform.position).normalized * 800f, ForceMode2D.Impulse);
                }
            }
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.F)) {
            for (int i = 0; i < 5; i++) {
                GameObject payload = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(payload.GetComponent<Collider>());
                payload.AddComponent<BoxCollider2D>();
                payload.transform.position = transform.position + new Vector3(Random.Range(-10f, 10f), 15f, 0f);
                payload.transform.localScale = new Vector3(2f, 2f, 1f);
                payload.AddComponent<Rigidbody2D>().mass = 50f;
                payload.GetComponent<Renderer>().material.color = Color.blue;
                activeObstacles.Add(payload);
            }
        }

        if (isAlt && Input.GetKeyDown(KeyCode.L)) {
            GameObject crumb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DestroyImmediate(crumb.GetComponent<Collider>());
            crumb.AddComponent<BoxCollider2D>();
            crumb.transform.position = transform.position;
            crumb.transform.localScale = new Vector3(1f, 1f, 1f);
            crumb.GetComponent<Renderer>().material.color = Color.yellow;
            activeObstacles.Add(crumb);
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.T) && activeObstacles.Count > 0) {
            GameObject target = activeObstacles[^1];
            if (target != null) {
                GameObject tag = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DestroyImmediate(tag.GetComponent<Collider>());
                tag.transform.position = target.transform.position + Vector3.up * 2f;
                tag.transform.SetParent(target.transform);
                tag.GetComponent<Renderer>().material.color = Color.magenta;
            }
        }

        if (isAlt && Input.GetKeyDown(KeyCode.C)) {
            Rigidbody2D myRb = GetComponent<Rigidbody2D>();
            myRb.linearVelocity = Vector2.zero;
            myRb.AddForce(new Vector2(sr.flipX ? -1500f : 1500f, 1000f), ForceMode2D.Impulse);
        }

        if (isCtrl && Input.GetKeyDown(KeyCode.E)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) {
                    obs.transform.localScale *= 0.5f;
                    obs.GetComponent<Renderer>().material.color = new Color(0.6f, 0.4f, 0.2f); // Brown box
                }
            }
        }

        if (isAlt && Input.GetKeyDown(KeyCode.S)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) obs.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 500f, ForceMode2D.Impulse);
            }
        }

        // git pull --rebase / Anti-Gravity Inversion (Alt + UpArrow)
        if (isAlt && Input.GetKeyDown(KeyCode.UpArrow)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) obs.GetComponent<Rigidbody2D>().gravityScale = -2f;
            }
        }

        if (Input.GetKeyDown(KeyCode.F8)) {
            // Physically barrel-roll the camera 180 degrees to bypass URP matrix overrides
            Camera.main.transform.Rotate(0f, 0f, 180f);
        }

        if (Input.GetKeyDown(KeyCode.F9)) {
            transform.localScale = new Vector3(4f, 4f, 1f); // Half of 8f
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.25f, 0f); // Dirt brown
            GetComponent<Collider2D>().enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.F9)) {
            transform.localScale = new Vector3(8f, 8f, 1f);
            GetComponent<SpriteRenderer>().color = Color.white;
            GetComponent<Collider2D>().enabled = true;
        }

        if (Input.GetKey(KeyCode.F10)) {
            GameObject rain = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rain.name = "MatrixRain";
            
            // Disable the native 3D collider so it doesn't bump into anything
            rain.GetComponent<Collider>().enabled = false;
            
            // Push it back to Z = 5 so it falls visually behind the Octocat
            rain.transform.position = new Vector3(Random.Range(-25f, 25f), 15f, 5f);
            rain.transform.localScale = new Vector3(0.5f, 2f, 1f);
            
            Renderer rend = rain.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Sprites/Default"));
            rend.material.color = Color.green;
            
            // FIX: Use a 3D Rigidbody. It bypasses the 2D component conflict completely!
            Rigidbody rb3d = rain.AddComponent<Rigidbody>();
            rb3d.mass = 1f;
            rb3d.useGravity = true; 
            
            activeObstacles.Add(rain);
            Destroy(rain, 4f);
        }

        if (Input.GetKeyDown(KeyCode.F11)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null) {
                    // Toggles the active state of the GameObject in the engine hierarchy
                    obs.SetActive(!obs.activeSelf);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F12)) {
            foreach (GameObject obs in activeObstacles) {
                if (obs != null && obs.activeInHierarchy) {
                    Rigidbody2D obsRb = obs.GetComponent<Rigidbody2D>();
                    if (obsRb != null) {
                        // Calculate trajectory towards global center
                        Vector2 centerDir = (Vector3.zero - obs.transform.position).normalized;
                        
                        // Kill current momentum and apply massive converging force
                        obsRb.linearVelocity = Vector2.zero;
                        obsRb.AddForce(centerDir * 2500f, ForceMode2D.Impulse);
                        obsRb.AddTorque(Random.Range(-500f, 500f));
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            foreach(Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)) {
                // Exclude the Octocat and the Chicken
                if (body != rb && (playerScript == null || body.gameObject != playerScript.gameObject)) {
                    
                    // 1. Randomize mass for collision weight
                    body.mass = Random.Range(0.1f, 100f);
                    
                    // 2. Randomize gravity so they fall at completely different speeds
                    body.gravityScale = Random.Range(0.2f, 5f);
                    
                    // 3. Randomize their scale so you can visually see the change
                    float randomSize = Random.Range(0.5f, 3f);
                    body.transform.localScale = new Vector3(randomSize, randomSize, 1f);
                    
                    // 4. Give them a tiny random rotation bump to make it look like a glitch
                    body.AddTorque(Random.Range(-50f, 50f));
                }
            }
        }

        if (Input.GetKey(KeyCode.Keypad9)) {
            // Find all physics objects in the scene
            foreach(Rigidbody2D body in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)) {
                
                // Ensure we don't pull the Octocat itself or the dormant Chicken
                if (body != rb && (playerScript == null || body.gameObject != playerScript.gameObject)) {
                    
                    // Calculate the direction from the object to the Octocat
                    Vector2 pullDirection = (transform.position - body.transform.position).normalized;
                    
                    // Apply a continuous force to drag them towards you
                    body.AddForce(pullDirection * (body.mass * 15f));
                }
            }
        }

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // If we cross the right edge, instantly teleport to the left edge
        if (viewportPos.x > 1.05f) {
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.05f, viewportPos.y, viewportPos.z));
        }
        // If we cross the left edge, instantly teleport to the right edge
        else if (viewportPos.x < -0.05f) {
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.05f, viewportPos.y, viewportPos.z));
        }

        if (Input.GetKeyDown(KeyCode.F1) && playerScript != null) {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            Vector2 dir = (mouseWorldPos - playerScript.transform.position).normalized;
            
            GameObject egg = Instantiate(playerScript.eggPrefab, playerScript.transform.position + (Vector3)dir, Quaternion.identity);
            egg.AddComponent<Rigidbody2D>().linearVelocity = dir * 15f;
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            Vector2 facing = sr.flipX ? Vector2.left : Vector2.right;
            
            // Draw a cyan laser in the Unity Scene view (not the Game view) for 2 seconds so you can see where it aims
            Debug.DrawRay(transform.position, facing * 30f, Color.cyan, 2f);
            
            // Shoot a piercing raycast that detects every single object in its path
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, facing, 30f);
            
            foreach (RaycastHit2D hit in hits) {
                // Ignore the Octocat itself and the dormant Chicken
                if (hit.collider.gameObject != gameObject && (playerScript == null || hit.collider.gameObject != playerScript.gameObject)) {
                    
                    // We found the first valid obstacle! Clone it, track it, and break the loop
                    GameObject clone = Instantiate(hit.collider.gameObject, hit.point + Vector2.up * 4f, Quaternion.identity);
                    activeObstacles.Add(clone);
                    break; 
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F3) && playerScript != null) {
            StartCoroutine(GitPushRoutine());
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

        if (Input.GetKeyDown(KeyCode.F4)) {
            Collider2D closest = null;
            float minDist = Mathf.Infinity;
            
            foreach(Collider2D hit in Physics2D.OverlapCircleAll(transform.position, 15f)) {
                if (hit.gameObject != gameObject && hit.gameObject != playerScript.gameObject) {
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    if (dist < minDist) { closest = hit; minDist = dist; }
                }
            }
            
            if (closest != null) {
                // Swap positions and steal its appearance
                Vector3 tempPos = transform.position;
                transform.position = closest.transform.position;
                closest.transform.position = tempPos;
                GetComponent<SpriteRenderer>().sprite = closest.GetComponent<SpriteRenderer>().sprite;
            }
        }

        if (Input.GetKeyDown(KeyCode.F5)) {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Default"), true);
            sr.color = new Color(1f, 1f, 1f, 0.3f);
        }
        if (Input.GetKeyUp(KeyCode.F5)) {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Default"), false);
            sr.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.F6)) {
            Camera.main.transform.position += new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
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
        
        if (movement.x > 0) sr.flipX = false;
        else if (movement.x < 0) sr.flipX = true;

        if (Input.GetKey(KeyCode.F7)) {
            Camera.main.backgroundColor = Color.HSVToRGB(Mathf.Repeat(Time.time, 1f), 1f, 1f);
        }
    }

    private System.Collections.IEnumerator GitPushRoutine() {
        // FIX: Grab the component directly so we don't rely on the global 'sr' variable
        bool isFlipped = GetComponent<SpriteRenderer>().flipX;

        // 1. Move the REAL Chicken directly into the Octocat's hands
        Vector2 throwDir = isFlipped ? Vector2.left : Vector2.right;
        playerScript.transform.position = transform.position + (Vector3)throwDir * 3f + Vector3.up * 1f;
        
        // 2. Make the real Chicken fully visible and physical
        playerScript.GetComponent<SpriteRenderer>().enabled = true;
        playerScript.GetComponent<Collider2D>().enabled = true;
        
        // 3. Launch the real Chicken!
        Rigidbody2D chickenRb = playerScript.GetComponent<Rigidbody2D>();
        chickenRb.linearVelocity = Vector2.zero;
        chickenRb.AddForce(throwDir * 1500f + Vector2.up * 600f, ForceMode2D.Impulse);
        
        // FIX: Use the localized isFlipped boolean here as well
        chickenRb.AddTorque(1000f * (isFlipped ? 1f : -1f));
        
        // 4. Wait exactly 2 seconds while it flies through the air
        yield return new WaitForSeconds(2f);
        
        // 5. Stop it exactly where it landed, fix its rotation, and make it vanish again
        chickenRb.linearVelocity = Vector2.zero;
        chickenRb.angularVelocity = 0f;
        chickenRb.transform.rotation = Quaternion.identity; // Stands it upright 
        
        playerScript.GetComponent<SpriteRenderer>().enabled = false;
        playerScript.GetComponent<Collider2D>().enabled = false;
    }
    void FixedUpdate()
    {
        // Only apply movement physics if the Octocat is in control
        if (playerScript != null && !playerScript.isMainCharacter) {
            rb.linearVelocity = movement * moveSpeed;
        }
    }
}