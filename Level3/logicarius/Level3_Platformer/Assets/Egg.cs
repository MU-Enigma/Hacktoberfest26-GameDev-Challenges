using UnityEngine;

// Created by EggLayer at runtime. Falls, slides away from fire, then becomes the checkpoint.
public class Egg : MonoBehaviour {
    public float slideSpeed = 3f;       // how fast it slides away from a fire
    public float settleTime = 0.4f;     // how long it must be still before it counts
    public float checkpointLift = 0.5f; // chicken respawns this far above the egg
    public float safeRadius = 0.8f;     // egg must rest at least this far from any fire (chicken is wider than the egg)

    public Collider2D Collider { get; private set; }

    PlayerLife owner;
    Rigidbody2D body;
    bool settled;
    float calmTimer, hazardTimer, hazardX;
    float slideDir = 1f;

    public void Setup(PlayerLife player, Sprite sprite, float size, SpriteRenderer reference) {
        owner = player;

        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerID = reference.sortingLayerID;
        sr.sortingOrder = reference.sortingOrder - 1;

        float s = size / sprite.bounds.size.x;
        transform.localScale = new Vector3(s, s, 1f);

        body = gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 3f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = sprite.bounds.extents.x * 0.9f;
        col.sharedMaterial = new PhysicsMaterial2D { friction = 0.6f, bounciness = 0f };
        Collider = col;
    }

    // true if any fire is within safeRadius of the egg
    bool NearFire() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, safeRadius);
        foreach (Collider2D h in hits) {
            if (h.GetComponent<Hazard>() != null) {
                hazardX = h.bounds.center.x;
                return true;
            }
        }
        return false;
    }

    void FixedUpdate() {
        if (settled) return;
        if (transform.position.y < -50f) { Destroy(gameObject); return; }

        if (NearFire()) {
            // push away from the fire; flip direction if stuck against a wall
            if (hazardTimer == 0f) slideDir = Mathf.Sign(transform.position.x - hazardX);
            hazardTimer += Time.fixedDeltaTime;
            if (hazardTimer > 1.5f) { slideDir = -slideDir; hazardTimer = 0.01f; }
            body.linearVelocity = new Vector2(slideDir * slideSpeed, body.linearVelocity.y);
            calmTimer = 0f;
        } else {
            hazardTimer = 0f;
            if (body.linearVelocity.sqrMagnitude < 0.01f) {
                calmTimer += Time.fixedDeltaTime;
                if (calmTimer >= settleTime) Settle();
            } else {
                calmTimer = 0f;
            }
        }
    }

    void Settle() {
        settled = true;
        owner.SetCheckpoint(transform.position + Vector3.up * checkpointLift);
    }
}