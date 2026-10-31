using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float speed = 5f;
    public Rigidbody2D body;

    [Header("Dash (Left Shift or Space)")]
    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.8f;

    [Header("Giant mode (E)")]
    public float giantScale = 2f;

    [Header("Time slow (hold T)")]
    public float slowTimeScale = 0.3f;

    [Header("Chick parade (F)")]
    public int chickCount = 4;
    public float chickSize = 0.35f;   // size of a chick compared to the mother

    [Header("Teleport to mouse (left click)")]
    public float teleportCooldown = 1f;
    public float teleportRange = 8f;

    [HideInInspector] public bool frozen;   // set by LevelBuilder while the finish message shows

    SpriteRenderer sr;
    CircleCollider2D col;
    float dashTimer, cooldownTimer, teleportTimer, trailTimer;
    Vector2 dashDirection;
    Vector2 lastDirection = Vector2.right;
    Vector2 smoothFacing = Vector2.right;
    Vector3 normalScale;
    float unit;
    bool isGiant, isGhost;
    int colorIndex = -1;   // -1 = original mint colour
    Sprite originalSprite;
    Sprite[] variants;     // recoloured chickens loaded from Resources
    readonly List<Transform> chicks = new List<Transform>();
    readonly List<float> chickFollow = new List<float>();

    // rainbow: coral red, sunset orange, golden yellow, fresh green, sky blue, violet, bubblegum pink
    static readonly float[] hues = { 355f, 28f, 48f, 125f, 205f, 265f, 325f };

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
        if (body == null) body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        // the circle collider that came with the object is tiny compared to the chicken picture,
        // so fit it to the sprite
        if (col != null && sr != null && sr.sprite != null)
            col.radius = sr.sprite.bounds.extents.x * 0.8f;

        normalScale = transform.localScale;
        unit = sr != null ? sr.bounds.size.x : 1f;
        originalSprite = sr != null ? sr.sprite : null;
        LoadVariants();
    }

    // the recoloured pictures are stored as Resources/chicken_0.bytes ... chicken_6.bytes (PNG data)
    void LoadVariants() {
        if (originalSprite == null) return;
        Sprite[] loaded = new Sprite[hues.Length];
        Vector2 pivot = new Vector2(originalSprite.pivot.x / originalSprite.rect.width,
                                    originalSprite.pivot.y / originalSprite.rect.height);
        for (int i = 0; i < loaded.Length; i++) {
            TextAsset ta = Resources.Load<TextAsset>("chicken_" + i);
            if (ta == null) return;   // missing: fall back to simple tinting
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.LoadImage(ta.bytes);
            loaded[i] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, originalSprite.pixelsPerUnit);
        }
        variants = loaded;
    }

    void Update() {
        if (frozen) {
            body.linearVelocity = Vector2.zero;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            return;
        }

        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(xInput, yInput).normalized;
        if (direction != Vector2.zero) lastDirection = direction;

        // time slow: hold T
        Time.timeScale = Input.GetKey(KeyCode.T) ? slowTimeScale : 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // dash
        cooldownTimer -= Time.deltaTime;
        teleportTimer -= Time.deltaTime;
        bool dashPressed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space);
        if (dashPressed && cooldownTimer <= 0f && dashTimer <= 0f) {
            dashTimer = dashTime;
            cooldownTimer = dashCooldown;
            dashDirection = lastDirection;
        }

        if (dashTimer > 0f) {
            dashTimer -= Time.deltaTime;
            body.linearVelocity = dashDirection * dashSpeed;
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0f) {
                trailTimer = 0.03f;
                SpawnAfterimage();   // dash trail
            }
        } else {
            body.linearVelocity = direction * speed;
        }

        if (Input.GetKeyDown(KeyCode.E)) ToggleGiant();
        if (Input.GetKeyDown(KeyCode.C)) CycleColor();
        if (Input.GetKeyDown(KeyCode.G)) ToggleGhost();
        if (Input.GetKeyDown(KeyCode.F)) ToggleChicks();
        if (Input.GetMouseButtonDown(0) && teleportTimer <= 0f) TryTeleport();
    }

    // the chicks walk behind the mother, each wandering a little in its own random way
    void LateUpdate() {
        if (chicks.Count == 0) return;
        float mul = isGiant ? giantScale : 1f;
        smoothFacing = Vector2.Lerp(smoothFacing, lastDirection, Mathf.Min(1f, 3f * Time.deltaTime));
        Vector2 behind = smoothFacing.sqrMagnitude > 0.0001f ? -smoothFacing.normalized : Vector2.left;
        Vector2 side = new Vector2(-behind.y, behind.x);

        for (int i = 0; i < chicks.Count; i++) {
            if (chicks[i] == null) continue;
            float dist = unit * mul * (1.0f + 0.65f * i);
            float n1 = Mathf.PerlinNoise(Time.time * (0.5f + 0.12f * i) + i * 13.7f, i * 0.37f) - 0.5f;
            float n2 = Mathf.PerlinNoise(i * 0.77f, Time.time * 0.6f + i * 5.1f) - 0.5f;
            Vector2 wander = side * (n1 * 1.8f * unit * mul) + behind * (n2 * 0.7f * unit * mul);
            float hop = Mathf.Abs(Mathf.Sin(Time.time * 9f + i * 1.7f)) * 0.1f * unit * mul;
            Vector3 target = transform.position + (Vector3)(behind * dist + wander) + new Vector3(0f, hop, 0f);
            chicks[i].position = Vector3.Lerp(chicks[i].position, target, Mathf.Min(1f, chickFollow[i] * Time.deltaTime));
            chicks[i].localScale = normalScale * chickSize * mul;
        }
    }

    void OnDisable() {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    // ---------- giant mode (E) ----------
    void ToggleGiant() {
        isGiant = !isGiant;
        transform.localScale = isGiant ? normalScale * giantScale : normalScale;
    }

    // ---------- rainbow chicken (C) ----------
    void CycleColor() {
        colorIndex++;
        if (colorIndex >= hues.Length) colorIndex = -1;
        ApplyLook();
    }

    void ApplyLook() {
        if (sr == null) return;
        Color c = Color.white;
        if (colorIndex < 0) {
            sr.sprite = originalSprite;
        } else if (variants != null) {
            sr.sprite = variants[colorIndex];
        } else {
            sr.sprite = originalSprite;
            c = Color.HSVToRGB(hues[colorIndex] / 360f, 0.5f, 1f);   // fallback: soft pastel tint
        }
        c.a = isGhost ? 0.4f : 1f;
        sr.color = c;

        foreach (Transform t in chicks) {
            if (t == null) continue;
            SpriteRenderer r = t.GetComponent<SpriteRenderer>();
            if (r == null) continue;
            r.sprite = sr.sprite;
            r.color = c;
        }
    }

    // ---------- ghost mode (G) ----------
    void ToggleGhost() {
        if (isGhost && InsideSolid(transform.position)) {
            LevelBuilder.Say("Can't leave ghost mode while inside a wall!");
            return;
        }
        isGhost = !isGhost;
        if (col != null) col.enabled = !isGhost;
        ApplyLook();
    }

    // ---------- chick parade (F) ----------
    void ToggleChicks() {
        if (chicks.Count > 0) {
            ClearChicks();
            return;
        }
        for (int i = 0; i < chickCount; i++) {
            GameObject go = new GameObject("Chick");
            go.transform.position = transform.position;
            go.transform.localScale = normalScale * chickSize;
            SpriteRenderer r = go.AddComponent<SpriteRenderer>();
            r.sprite = sr.sprite;
            r.sortingOrder = sr.sortingOrder - 1;
            chicks.Add(go.transform);
            chickFollow.Add(Random.Range(3.5f, 6.5f));
        }
        ApplyLook();
    }

    void ClearChicks() {
        foreach (Transform t in chicks) if (t != null) Destroy(t.gameObject);
        chicks.Clear();
        chickFollow.Clear();
    }

    // ---------- teleport to mouse (left click) ----------
    void TryTeleport() {
        Camera cam = Camera.main;
        if (cam == null) return;
        Vector3 w = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = new Vector2(w.x, w.y);
        Vector2 pos = transform.position;
        Vector2 delta = target - pos;
        if (delta.magnitude > teleportRange) target = pos + delta.normalized * teleportRange;

        if (!LevelBuilder.CanTeleportTo(target)) {
            LevelBuilder.Say("Can't teleport outside the stairs!");
            return;
        }
        if (!isGhost && InsideSolid(target)) {
            LevelBuilder.Say("Blocked! You can't teleport into a wall.");
            return;
        }
        teleportTimer = teleportCooldown;
        SpawnAfterimage();
        body.position = target;
        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }

    bool InsideSolid(Vector2 p) {
        if (col == null) return false;
        float r = col.radius * Mathf.Max(transform.localScale.x, transform.localScale.y) * 0.9f;
        foreach (Collider2D h in Physics2D.OverlapCircleAll(p, r)) {
            if (h != col && !h.isTrigger) return true;
        }
        return false;
    }

    // ---------- dash trail ----------
    void SpawnAfterimage() {
        if (sr == null) return;
        GameObject go = new GameObject("Afterimage");
        go.transform.position = transform.position;
        go.transform.localScale = transform.localScale;
        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sprite = sr.sprite;
        Color c = sr.color;
        c.a = 0.5f;
        r.color = c;
        r.sortingLayerID = sr.sortingLayerID;
        r.sortingOrder = sr.sortingOrder - 1;
        StartCoroutine(FadeOut(r, 0.35f));
    }

    IEnumerator FadeOut(SpriteRenderer r, float duration) {
        float t = 0f;
        while (t < duration && r != null) {
            t += Time.deltaTime;
            Color c = r.color;
            c.a = Mathf.Lerp(0.5f, 0f, t / duration);
            r.color = c;
            yield return null;
        }
        if (r != null) Destroy(r.gameObject);
    }

    // ---------- called by LevelBuilder when the chicken respawns ----------
    public void ResetState() {
        if (isGiant) ToggleGiant();
        if (isGhost) {
            isGhost = false;
            if (col != null) col.enabled = true;
        }
        ClearChicks();
        colorIndex = -1;
        dashTimer = 0f;
        ApplyLook();
    }
}