using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float speed = 6f;
    public Rigidbody2D body;

    [Header("Jumping")]
    public float jumpSpeed = 12f;        // W / Up arrow
    public float fastFallSpeed = 20f;    // S / Down arrow while in the air

    bool grounded;

    void Awake() {
        if (body == null) body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;   // smooth movement between physics steps
        QualitySettings.vSyncCount = 1;                              // no tearing
        body.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    // reset every physics step; OnCollisionStay2D sets it back to true while we stand on something
    void FixedUpdate() {
        grounded = false;
    }

    void OnCollisionStay2D(Collision2D collision) {
        foreach (ContactPoint2D contact in collision.contacts) {
            if (contact.normal.y > 0.5f) grounded = true;   // something is under our feet
        }
    }

    void Update() {
        float xInput = Input.GetAxisRaw("Horizontal");   // A / D / Left arrow / Right arrow
        float yVelocity = body.linearVelocity.y;

        if (grounded) {
            bool normalJump = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            bool superJump = Input.GetKeyDown(KeyCode.Space);

            if (normalJump) {
                yVelocity = jumpSpeed;
                grounded = false;
            } else if (superJump) {
                yVelocity = jumpSpeed * Mathf.Sqrt(2f);   // twice the height of a normal jump
                grounded = false;
            }
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            yVelocity = Mathf.Min(yVelocity, -fastFallSpeed);   // fast fall
        }

        body.linearVelocity = new Vector2(xInput * speed, yVelocity);
    }
}