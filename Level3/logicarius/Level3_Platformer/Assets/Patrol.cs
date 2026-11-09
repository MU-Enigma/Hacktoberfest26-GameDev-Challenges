using UnityEngine;

// A flame that slides back and forth. Added by ArenaBuilder (kinematic Rigidbody2D + Hazard).
public class Patrol : MonoBehaviour {
    public float range = 2.4f;     // how far it moves each way
    public float speed = 1.8f;     // top speed in units per second

    Rigidbody2D rb;
    Vector2 start;
    float phase;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        start = transform.position;
        phase = Random.value * 6.28f;
    }

    void FixedUpdate() {
        float x = start.x + Mathf.Sin(Time.time * speed / Mathf.Max(0.1f, range) + phase) * range;
        rb.MovePosition(new Vector2(x, start.y));
    }
}