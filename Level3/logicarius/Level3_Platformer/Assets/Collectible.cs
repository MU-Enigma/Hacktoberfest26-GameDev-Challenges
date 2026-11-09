using UnityEngine;

public enum CollectKind { Rock, Worm, Heart }

// Put this on every rock, worm and heart pickup. It needs a Collider2D with Is Trigger ticked.
public class Collectible : MonoBehaviour {
    public CollectKind kind = CollectKind.Rock;
    public int points = 10;            // only used by worms
    bool taken;

    void OnTriggerEnter2D(Collider2D other) { TryCollect(other); }
    void OnTriggerStay2D(Collider2D other) { TryCollect(other); }

    void TryCollect(Collider2D other) {
        if (taken) return;
        PlayerLife life = other.GetComponentInParent<PlayerLife>();
        PlayerStats stats = other.GetComponentInParent<PlayerStats>();
        if (life == null || !life.IsAlive) return;

        bool collected = false;
        if (kind == CollectKind.Heart) {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            collected = health != null && health.TryHeal();
        } else if (stats != null) {
            collected = (kind == CollectKind.Rock) ? stats.TryAddRock() : stats.TryEatWorm(points);
        }

        if (collected) {
            taken = true;
            Destroy(gameObject);       // stays gone, even after you die and respawn
        }
    }
}