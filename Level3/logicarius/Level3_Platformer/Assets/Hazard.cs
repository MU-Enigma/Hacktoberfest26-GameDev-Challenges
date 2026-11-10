using UnityEngine;

// Put this on anything that hurts the chicken (fire).
// The object also needs a Collider2D with "Is Trigger" ticked.
public class Hazard : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        Hurt(other);
    }

    void OnTriggerStay2D(Collider2D other) {
        Hurt(other);
    }

    void Hurt(Collider2D other) {
        PlayerLife life = other.GetComponentInParent<PlayerLife>();
        if (life != null) life.Die();
    }
}