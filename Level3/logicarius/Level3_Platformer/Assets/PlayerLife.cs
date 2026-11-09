using System.Collections;
using UnityEngine;

// Put this on the Player. Handles dying (turns into a roasted drumstick) and coming back at the last checkpoint.
public class PlayerLife : MonoBehaviour {
    [Header("Death look")]
    public Sprite deadSprite;          // the roasted drumstick
    public float deadSize = 1.2f;      // how big the drumstick is, in world units
    public float deathPause = 1.2f;    // seconds the drumstick stays before the chicken comes back

    [Header("Falling")]
    public float fallLimit = -30f;     // if the chicken falls below this height it counts as dead

    [Header("After coming back")]
    public float graceTime = 1f;       // seconds you cannot be burned after respawning (the chicken blinks)

    public bool IsAlive { get; private set; } = true;
    public bool Invulnerable { get; set; }                 // set when you win
    public bool BlockRespawn { get; set; }                 // set on game over so the chicken stays dead
    public Vector3 RespawnPoint { get; private set; }
    public event System.Action Respawned;
    public event System.Action Died;

    SpriteRenderer sr;
    Rigidbody2D body;
    PlayerMovement movement;
    Sprite aliveSprite;
    Vector3 aliveScale;
    float graceUntil;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        aliveSprite = sr.sprite;
        aliveScale = transform.localScale;
    }

    void Start() {
        // the start point is simply wherever the chicken was placed (ArenaBuilder places it before this runs)
        RespawnPoint = transform.position;
    }

    void Update() {
        if (!IsAlive) return;
        if (transform.position.y < fallLimit) Die();
        bool grace = Time.time < graceUntil;
        sr.enabled = !grace || ((int)(Time.time * 12f) % 2 == 0);   // blink while protected
    }

    public void SetCheckpoint(Vector3 position) {
        RespawnPoint = position;
    }

    public void Die() {
        if (!IsAlive || Invulnerable || Time.time < graceUntil) return;
        IsAlive = false;
        sr.enabled = true;

        if (movement != null) movement.enabled = false;
        body.linearVelocity = Vector2.zero;
        body.simulated = false;            // freeze in place: no gravity, no collisions

        if (deadSprite != null) {
            sr.sprite = deadSprite;
            float s = deadSize / deadSprite.bounds.size.x;
            transform.localScale = new Vector3(s, s, 1f);
        }
        if (Died != null) Died();
        StartCoroutine(ComeBack());
    }

    IEnumerator ComeBack() {
        yield return new WaitForSeconds(deathPause);
        if (BlockRespawn) yield break;

        transform.position = RespawnPoint;
        sr.sprite = aliveSprite;
        transform.localScale = aliveScale;
        body.simulated = true;
        body.linearVelocity = Vector2.zero;
        if (movement != null) movement.enabled = true;
        IsAlive = true;
        graceUntil = Time.time + graceTime;

        if (Respawned != null) Respawned();
    }
}