using UnityEngine;

// The golden egg at the top. Touch it to win. Added by ArenaBuilder.
public class Goal : MonoBehaviour {
    Vector3 basePos, baseScale;

    void Awake() { basePos = transform.position; baseScale = transform.localScale; }

    void Update() {
        float t = Time.time;
        transform.position = basePos + Vector3.up * Mathf.Sin(t * 2.5f) * 0.12f;
        transform.localScale = baseScale * (1f + Mathf.Sin(t * 5f) * 0.04f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth h = other.GetComponentInParent<PlayerHealth>();
        if (h != null) h.Win();
    }
}