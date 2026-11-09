using UnityEngine;

// Put this on the Player. Lays an egg every 'interval' seconds while alive.
public class EggLayer : MonoBehaviour {
    public Sprite eggSprite;               // drag egg here
    public float interval = 15f;           // seconds between eggs
    public float eggSize = 0.5f;           // egg width in world units
    public Vector2 spawnOffset = new Vector2(0f, -0.2f);
    public bool keepOldEggs = false;       // false = only the latest egg stays

    PlayerLife life;
    SpriteRenderer sr;
    Collider2D[] myColliders;
    Egg lastEgg;
    float timer;

    public float SecondsToNextEgg { get { return interval - timer; } }

    void Awake() {
        life = GetComponent<PlayerLife>();
        sr = GetComponent<SpriteRenderer>();
        myColliders = GetComponentsInChildren<Collider2D>();
    }

    void Start() {
        life.Respawned += ResetTimer;
    }

    void OnDestroy() {
        if (life != null) life.Respawned -= ResetTimer;
    }

    void ResetTimer() { timer = 0f; }

    void Update() {
        if (!life.IsAlive) return;
        timer += Time.deltaTime;
        if (timer >= interval) {
            timer = 0f;
            Lay();
        }
    }

    void Lay() {
        if (eggSprite == null) { Debug.LogWarning("EggLayer: drag the egg sprite into Egg Sprite"); return; }

        GameObject go = new GameObject("Egg");
        go.transform.position = transform.position + (Vector3)spawnOffset;
        Egg egg = go.AddComponent<Egg>();
        egg.Setup(life, eggSprite, eggSize, sr);

        foreach (Collider2D c in myColliders) Physics2D.IgnoreCollision(egg.Collider, c);

        if (!keepOldEggs && lastEgg != null) Destroy(lastEgg.gameObject);
        lastEgg = egg;
    }
}