using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

// Put this on the Player. Hearts, rocks scatter ahead when you burn, win at the golden egg,
// game over after the last heart. Both endings go back to the title screen.
public class PlayerHealth : MonoBehaviour {
    [Header("Hearts")]
    public int maxHearts = 3;
    public Sprite heartSprite;             // the heart sprite

    [Header("Rocks lost when you die")]
    public GameObject rockPrefab;          // the rock prefab (Assets/Prefabs/rock_0)
    public float minAhead = 3f;            // new rocks appear this far ahead of your respawn point...
    public float maxAhead = 14f;           // ...up to this far
    public float rockLift = 0.4f;          // how high above the ground a rock floats

    [Header("Ending")]
    public float endDelay = 8f;            // seconds the WIN / GAME OVER screen stays (Enter skips it)
    public int winBonus = 100;

    public int Hearts { get; private set; }

    PlayerLife life;
    PlayerStats stats;
    bool gameOver, won, restarting;
    float showAt, exitAt, winTime;
    GUIStyle bigStyle, smallStyle, hudStyle;

    void Awake() {
        life = GetComponent<PlayerLife>();
        stats = GetComponent<PlayerStats>();
        Hearts = maxHearts;
    }

    void Start() { life.Died += OnDied; }
    void OnDestroy() { if (life != null) life.Died -= OnDied; }

    public bool TryHeal() {
        if (Hearts >= maxHearts) return false;
        Hearts++;
        return true;
    }

    void OnDied() {
        Hearts = Mathf.Max(0, Hearts - 1);
        int lost = (stats != null) ? stats.LoseAllRocks() : 0;

        if (Hearts <= 0) {
            gameOver = true;
            life.BlockRespawn = true;                       // no coming back after the last heart
            showAt = Time.unscaledTime + life.deathPause;   // show the screen after the drumstick moment
            exitAt = showAt + endDelay;
            return;
        }
        ScatterRocks(lost);
    }

    // called by the golden egg
    public void Win() {
        if (won || gameOver) return;
        won = true;
        life.Invulnerable = true;
        life.BlockRespawn = true;
        PlayerMovement mv = GetComponent<PlayerMovement>();
        if (mv != null) mv.enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (stats != null) stats.AddBonus(winBonus);
        winTime = Time.timeSinceLevelLoad;
        showAt = Time.unscaledTime + 0.6f;
        exitAt = showAt + endDelay;
    }

    void Update() {
        if ((!gameOver && !won) || restarting) return;
        bool skip = Time.unscaledTime >= showAt && Input.GetKeyDown(KeyCode.Return);
        if (skip || Time.unscaledTime >= exitAt) Restart();
    }

    void Restart() {
        restarting = true;
        Time.timeScale = 1f;
#if UNITY_EDITOR
        EditorSceneManager.LoadSceneInPlayMode(SceneManager.GetActiveScene().path, new LoadSceneParameters(LoadSceneMode.Single));
#else
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
#endif
    }

    // ---------- rocks ----------
    void ScatterRocks(int count) {
        if (count <= 0) return;
        if (rockPrefab == null) { Debug.LogWarning("PlayerHealth: drag the rock prefab into Rock Prefab"); return; }
        if (ArenaBuilder.Current != null) { ScatterInArena(count); return; }

        Vector3 from = life.RespawnPoint;
        int placed = 0;
        for (int tries = 0; tries < 80 && placed < count; tries++) {
            float x = from.x + Random.Range(minAhead, maxAhead);
            Vector2 spot;
            if (FindSpot(x, from.y, out spot)) {
                Instantiate(rockPrefab, spot, Quaternion.identity);
                placed++;
            }
        }
        if (placed < count) Debug.LogWarning("PlayerHealth: could only place " + placed + " of " + count + " rocks");
    }

    // in the generated arena: pick free spots ahead of you on the same floor
    void ScatterInArena(int count) {
        Vector3 from = life.RespawnPoint;
        int placed = 0;
        for (int i = 0; i < count; i++) {
            Vector2 spot;
            if (ArenaBuilder.Current.TryPickSpotAhead(from.x, from.y, minAhead, maxAhead, rockLift, out spot)) {
                Instantiate(rockPrefab, spot, Quaternion.identity);
                placed++;
            }
        }
        if (placed < count) Debug.LogWarning("PlayerHealth: could only place " + placed + " of " + count + " rocks");
    }

    // look straight down at this x for ground; reject spots on fire or on top of another pickup
    bool FindSpot(float x, float fromY, out Vector2 spot) {
        spot = Vector2.zero;
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(x, fromY + 20f), Vector2.down, 60f);
        foreach (RaycastHit2D hit in hits) {
            if (hit.collider == null || hit.collider.isTrigger) continue;
            if (hit.collider.GetComponentInParent<PlayerLife>() != null) continue;
            if (hit.collider.GetComponent<Egg>() != null) continue;

            Vector2 p = new Vector2(x, hit.point.y + rockLift);
            foreach (Collider2D c in Physics2D.OverlapCircleAll(p, 0.7f)) {
                if (c.GetComponent<Hazard>() != null || c.GetComponent<Collectible>() != null) return false;
            }
            spot = p;
            return true;
        }
        return false;
    }

    // ---------- drawing ----------
    void DrawSprite(Sprite s, Rect r) {
        Texture2D tex = s.texture;
        tex.filterMode = FilterMode.Point;           // sharp pixels
        Rect tr = s.textureRect;
        Rect uv = new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);
        GUI.DrawTextureWithTexCoords(r, tex, uv);
    }

    static string Clock(float seconds) {
        int s = Mathf.FloorToInt(seconds);
        return (s / 60) + ":" + (s % 60).ToString("00");
    }

    void OnGUI() {
        GUI.depth = -500;                                  // the title screen (-1000) stays on top
        float sw = Screen.width, sh = Screen.height;

        if (heartSprite != null) {
            float size = sh * 0.07f, gap = size * 0.25f;
            float total = maxHearts * size + (maxHearts - 1) * gap;
            float x0 = (sw - total) / 2f, y = sh * 0.02f;
            for (int i = 0; i < maxHearts; i++) {
                GUI.color = (i < Hearts) ? Color.white : new Color(0f, 0f, 0f, 0.45f);   // lost hearts turn into dark shadows
                DrawSprite(heartSprite, new Rect(x0 + i * (size + gap), y, size, size));
            }
            GUI.color = Color.white;
        }

        if (hudStyle == null) {
            hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.alignment = TextAnchor.UpperRight;
        }
        hudStyle.fontSize = Mathf.RoundToInt(sh * 0.035f);
        hudStyle.normal.textColor = Color.white;
        float shown = won ? winTime : Time.timeSinceLevelLoad;
        GUI.Label(new Rect(sw - 300f - 10f, 10f, 300f, sh * 0.06f), "Time " + Clock(shown), hudStyle);

        bool ending = (gameOver || won) && Time.unscaledTime >= showAt;
        if (!ending) return;

        if (bigStyle == null) {
            bigStyle = new GUIStyle(GUI.skin.label);
            bigStyle.alignment = TextAnchor.MiddleCenter;
            bigStyle.fontStyle = FontStyle.Bold;
            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.alignment = TextAnchor.MiddleCenter;
        }
        bigStyle.fontSize = Mathf.RoundToInt(sh * 0.12f);
        bigStyle.normal.textColor = won ? new Color(1f, 0.85f, 0.25f, 1f) : new Color(1f, 0.3f, 0.2f, 1f);
        smallStyle.fontSize = Mathf.RoundToInt(sh * 0.035f);
        smallStyle.normal.textColor = Color.white;

        GUI.color = won ? new Color(0.04f, 0.08f, 0.04f, 0.88f) : new Color(0.08f, 0.02f, 0.04f, 0.88f);
        GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
        GUI.color = Color.white;

        int score = (stats != null) ? stats.Score : 0;
        GUI.Label(new Rect(0, sh * 0.26f, sw, sh * 0.16f), won ? "YOU WIN!" : "GAME OVER", bigStyle);
        if (won) {
            GUI.Label(new Rect(0, sh * 0.45f, sw, sh * 0.06f), "You reached the golden egg!", smallStyle);
            GUI.Label(new Rect(0, sh * 0.52f, sw, sh * 0.06f), "Score: " + score + "      Time: " + Clock(winTime) + "      Hearts left: " + Hearts, smallStyle);
        } else {
            GUI.Label(new Rect(0, sh * 0.48f, sw, sh * 0.06f), "Score: " + score, smallStyle);
        }
        GUI.Label(new Rect(0, sh * 0.64f, sw, sh * 0.06f), "Back to the start screen...  (Enter to skip)", smallStyle);
    }
}