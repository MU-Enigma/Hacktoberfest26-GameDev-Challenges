using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Builds the whole game world when you press Play: a walled arena with several floors, stairs between them,
// fire, patrolling flames, hurdles, floating platforms, hearts, and a golden egg at the top.
// Rocks and worms are placed at random on every run. Put this on an empty GameObject called "Arena".
public class ArenaBuilder : MonoBehaviour {
    public static ArenaBuilder Current { get; private set; }

    [Header("Size")]
    public float arenaWidth = 110f;
    public int floors = 6;

    [Header("Randomness")]
    public int layoutSeed = 11;            // same layout every run. Set 0 for a new layout every run
    public int rockCount = 12;
    public int wormCount = 30;
    public int heartPickups = 3;

    [Header("Art (filled in automatically in the editor)")]
    public Sprite fireSprite;
    public Sprite heartSprite;
    public Sprite eggSprite;
    public GameObject rockPrefab;
    public GameObject wormPrefab;
    public bool clearOldLevel = true;      // removes the hand-made Ground, Steps, fire, rocks and worms

    [Header("Colours")]
    // every floor gets its own set: floor body, bright top strip, and block colour (steps, hurdles, platforms)
    public Color[] floorPalette = {
        new Color(0.40f, 0.20f, 0.60f, 1f),   // plum
        new Color(0.70f, 0.18f, 0.45f, 1f),   // rose
        new Color(0.32f, 0.26f, 0.72f, 1f),   // violet
        new Color(0.62f, 0.14f, 0.62f, 1f),   // magenta
        new Color(0.17f, 0.32f, 0.65f, 1f),   // cobalt
        new Color(0.50f, 0.22f, 0.70f, 1f) }; // grape
    public Color[] topPalette = {
        new Color(1.00f, 0.50f, 0.75f, 1f),   // bubblegum
        new Color(1.00f, 0.92f, 0.40f, 1f),   // lemon
        new Color(0.60f, 1.00f, 0.45f, 1f),   // lime
        new Color(0.35f, 0.95f, 0.90f, 1f),   // aqua
        new Color(1.00f, 0.70f, 0.50f, 1f),   // peach
        new Color(0.50f, 1.00f, 0.75f, 1f) }; // mint
    public Color[] blockPalette = {
        new Color(1.00f, 0.78f, 0.30f, 1f),   // gold
        new Color(0.30f, 0.85f, 0.80f, 1f),   // turquoise
        new Color(0.80f, 0.60f, 1.00f, 1f),   // lilac
        new Color(1.00f, 0.55f, 0.60f, 1f),   // coral pink
        new Color(0.60f, 0.90f, 0.40f, 1f),   // lime
        new Color(1.00f, 0.65f, 0.25f, 1f) }; // orange
    public Color trapdoorColor = new Color(1.00f, 0.94f, 0.78f, 1f);    // cream: "jump through me"
    public Color wallColor = new Color(0.16f, 0.07f, 0.30f, 1f);
    public Color[] skyStops = {                                         // sunset at the bottom, deep night at the top
        new Color(1.00f, 0.78f, 0.62f, 1f),
        new Color(0.97f, 0.52f, 0.72f, 1f),
        new Color(0.55f, 0.32f, 0.82f, 1f),
        new Color(0.16f, 0.09f, 0.38f, 1f) };

    Color curFloor, curTop, curBlock;

    struct Surface { public float left, right, top; }
    readonly List<Surface> surfaces = new List<Surface>();
    readonly List<Surface> floaters = new List<Surface>();
    readonly List<Vector2> hazardSpots = new List<Vector2>();
    readonly List<Vector2> pickupSpots = new List<Vector2>();

    Sprite whiteSprite;
    Transform holder, player;
    Camera cam;
    float jumpH = 1.7f, stepRise = 1.2f, hurdleH = 1f, floaterRise = 2.2f, tierHeight = 7.2f, ceilingY;
    const float StairWidth = 11f, StepW = 2f, StepSpacing = 2.25f;

    // Makes sure the arena exists even if the Arena object was never added to the scene.
    // sceneLoaded also fires when the game reloads after WIN / GAME OVER, so the arena is rebuilt every time.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (FindFirstObjectByType<ArenaBuilder>() != null) return;
        if (FindFirstObjectByType<PlayerLife>() == null) return;      // not the game scene
        new GameObject("Arena").AddComponent<ArenaBuilder>();
    }

    // ---------- setup ----------
#if UNITY_EDITOR
    void Reset() { AutoFill(); }

    void AutoFill() {
        if (fireSprite == null) fireSprite = LoadSprite("Assets/Sprites/fire.png");
        if (heartSprite == null) heartSprite = LoadSprite("Assets/Sprites/heart.png");
        if (eggSprite == null) eggSprite = LoadSprite("Assets/Sprites/egg.png");
        if (rockPrefab == null) rockPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/rock_0.prefab");
        if (wormPrefab == null) wormPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/worm_0.prefab");
    }

    static Sprite LoadSprite(string path) {
        foreach (Object o in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path)) {
            Sprite s = o as Sprite;
            if (s != null) return s;
        }
        return null;
    }
#endif

    void Awake() {
        Current = this;
#if UNITY_EDITOR
        AutoFill();
#endif
        if (clearOldLevel) ClearOld();
        ReadMetrics();

        holder = new GameObject("ArenaGenerated").transform;
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, Color.white);
        t.Apply();
        t.filterMode = FilterMode.Point;
        whiteSprite = Sprite.Create(t, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        Random.InitState(layoutSeed != 0 ? layoutSeed : System.Environment.TickCount);
        BuildShell();
        BuildFloors();
        BuildGoal();

        Random.InitState(System.Environment.TickCount);    // pickups are different every run
        Scatter(rockPrefab, rockCount, 0.35f);
        Scatter(wormPrefab, wormCount, 0.3f);
        PlaceHearts();

        PlacePlayer();
        SetupCamera();
        SharpenPixelArt();
    }

    void OnDestroy() { if (Current == this) Current = null; }

    void ClearOld() {
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects()) {
            string n = go.name;
            if (n == "Ground" || n.StartsWith("Step") || n.StartsWith("fire") || n.StartsWith("rock") || n.StartsWith("worm"))
                Destroy(go);
        }
    }

    void ReadMetrics() {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) {
            Rigidbody2D rb = pm.GetComponent<Rigidbody2D>();
            float g = Mathf.Abs(Physics2D.gravity.y) * (rb != null ? rb.gravityScale : 1f);
            if (g > 0.01f) jumpH = pm.jumpSpeed * pm.jumpSpeed / (2f * g);
        }
        stepRise = Mathf.Min(1.3f, jumpH * 0.72f);          // every climb step is comfortably inside a normal jump
        hurdleH = Mathf.Min(1.1f, jumpH * 0.6f);
        floaterRise = hurdleH + stepRise * 0.95f;
        tierHeight = stepRise * 6f;                        // 5 steps + the final hop onto the next floor
        ceilingY = (floors - 1) * tierHeight + 9f;
    }

    // ---------- building blocks ----------
    GameObject MakeBlock(string name, float left, float right, float top, float thickness, Color color, bool grass) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(holder);
        go.transform.position = new Vector3((left + right) / 2f, top - thickness / 2f, 0f);
        go.transform.localScale = new Vector3(right - left, thickness, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = whiteSprite;
        sr.color = color;
        sr.sortingOrder = -10;
        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.size = Vector2.one;

        if (grass) {                                        // thin coloured strip on top: makes the path easy to read
            GameObject g = new GameObject(name + " Top");
            g.transform.SetParent(holder);
            g.transform.position = new Vector3((left + right) / 2f, top - 0.1f, 0f);
            g.transform.localScale = new Vector3(right - left, 0.2f, 1f);
            SpriteRenderer gs = g.AddComponent<SpriteRenderer>();
            gs.sprite = whiteSprite;
            gs.color = curTop;
            gs.sortingOrder = -9;
        }
        return go;
    }

    // a floor you can jump up through from below but can never fall through
    void MakeTrapdoor(string name, float left, float right, float top) {
        GameObject go = MakeBlock(name, left, right, top, 0.5f, trapdoorColor, false);
        go.GetComponent<BoxCollider2D>().usedByEffector = true;
        PlatformEffector2D eff = go.AddComponent<PlatformEffector2D>();
        eff.useOneWay = true;
        eff.surfaceArc = 170f;
        eff.useSideFriction = false;
        eff.useSideBounce = false;
    }

    GameObject MakeFireObject(float x, float topY) {
        GameObject go = new GameObject("Fire");
        go.transform.SetParent(holder);
        go.transform.position = new Vector3(x, topY + 0.45f, 0f);
        go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = fireSprite;
        sr.sortingOrder = -1;
        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(0.5f, 0.4f);        // small box low in the flame: only a real touch burns
        bc.offset = new Vector2(0f, -0.3f);
        go.AddComponent<Hazard>();
        return go;
    }

    void MakeFire(float x, float topY) {
        if (fireSprite == null) { Debug.LogWarning("ArenaBuilder: Fire Sprite is empty"); return; }
        MakeFireObject(x, topY);
        hazardSpots.Add(new Vector2(x, topY + 0.4f));
    }

    // a flame that slides back and forth along the floor
    void MakePatrolFire(float cx, float topY, float range, float speed) {
        if (fireSprite == null) return;
        GameObject go = MakeFireObject(cx, topY);
        go.name = "Patrol Fire";
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        Patrol p = go.AddComponent<Patrol>();
        p.range = range;
        p.speed = speed;
        hazardSpots.Add(new Vector2(cx, topY + 0.4f));
        hazardSpots.Add(new Vector2(cx - range, topY + 0.4f));
        hazardSpots.Add(new Vector2(cx + range, topY + 0.4f));
    }

    // ---------- the world ----------
    void BuildShell() {
        MakeBlock("Wall Left", -1f, 0f, ceilingY + 1f, ceilingY + 2f, wallColor, false);
        MakeBlock("Wall Right", arenaWidth, arenaWidth + 1f, ceilingY + 1f, ceilingY + 2f, wallColor, false);
        MakeBlock("Ceiling", -1f, arenaWidth + 1f, ceilingY + 1f, 1f, wallColor, false);
    }

    void BuildFloors() {
        for (int k = 0; k < floors; k++) {
            float y = k * tierHeight;
            curFloor = floorPalette[k % floorPalette.Length];
            curTop = topPalette[k % topPalette.Length];
            curBlock = blockPalette[k % blockPalette.Length];
            if (k == 0) {
                MakeBlock("Floor 0", 0f, arenaWidth, y, 1f, curFloor, true);
            } else {
                // stairs from below come up on the right (even k-1) or left. The floor above them is a one-way trapdoor,
                // so the floors are full width and there is never a drop-off to fall down.
                bool incomingRight = ((k - 1) % 2 == 0);
                if (incomingRight) {
                    MakeBlock("Floor " + k, 0f, arenaWidth - StairWidth, y, 1f, curFloor, true);
                    MakeTrapdoor("Trapdoor " + k, arenaWidth - StairWidth, arenaWidth, y);
                } else {
                    MakeBlock("Floor " + k, StairWidth, arenaWidth, y, 1f, curFloor, true);
                    MakeTrapdoor("Trapdoor " + k, 0f, StairWidth, y);
                }
            }

            float ua = 1f, ub = arenaWidth - 1f;                // usable range for pickups
            if (k == 0) ua = 7f;                                // keep the start area clear
            if (k < floors - 1) {
                bool right = (k % 2 == 0);
                BuildStairs(y, right);
                if (right) ub = arenaWidth - StairWidth - 1f; else ua = StairWidth + 1f;
            }
            surfaces.Add(new Surface { left = ua, right = ub, top = y });
            BuildObstacles(k, y);
        }
    }

    // five steps rising up to the trapdoor of the next floor
    void BuildStairs(float y, bool right) {
        float hl = right ? arenaWidth - StairWidth : 0f;
        for (int i = 1; i <= 5; i++) {
            float left = right ? hl + (5 - i) * StepSpacing : hl + (i - 1) * StepSpacing;
            float top = y + i * stepRise;
            MakeBlock("Step", left, left + StepW, top, 0.5f, curBlock, false);
            surfaces.Add(new Surface { left = left, right = left + StepW, top = top });
        }
    }

    void BuildObstacles(int k, float y) {
        float d = k / (float)Mathf.Max(1, floors - 1);          // 0 on the first floor, 1 on the last: harder every floor
        float cursor = (k == 0 ? 9f : 13f) + Random.Range(0f, 2f);
        float hi = arenaWidth - 13f;
        float gapMin = 3.5f - 1.3f * d, gapMax = 5.5f - 2.0f * d;

        float pFire = 0.30f + 0.15f * d;
        float pHurdle = pFire + 0.15f;
        float pFloater = pHurdle + 0.15f;
        float pBridge = pFloater + (k >= 1 ? 0.12f : 0f);
        float pPatrol = pBridge + (k >= 2 ? 0.18f : 0f);

        while (cursor < hi - 6f) {
            float r = Random.value;
            if (r < pFire) {                                    // 1 to 3 fires in a row (up to 4 on the upper floors)
                int n = Random.Range(1, d > 0.5f ? 5 : 4);
                for (int j = 0; j < n; j++) MakeFire(cursor + j * 0.9f, y);
                cursor += n * 0.9f + Random.Range(gapMin, gapMax);
            } else if (r < pHurdle) {                           // a low block to hop over
                MakeBlock("Hurdle", cursor, cursor + 1.6f, y + hurdleH, hurdleH, curBlock, true);
                surfaces.Add(new Surface { left = cursor, right = cursor + 1.6f, top = y + hurdleH });
                cursor += 1.6f + Random.Range(gapMin, gapMax);
            } else if (r < pFloater) {                          // a block, then a platform floating above it
                MakeBlock("Hurdle", cursor, cursor + 1.6f, y + hurdleH, hurdleH, curBlock, true);
                float fl = cursor + 1.6f + 0.6f, fr = fl + 3.2f;
                MakeBlock("Floater", fl, fr, y + floaterRise, 0.5f, curBlock, true);
                Surface s = new Surface { left = fl, right = fr, top = y + floaterRise };
                surfaces.Add(s); floaters.Add(s);
                cursor = fr + Random.Range(gapMin, gapMax);
            } else if (r < pBridge) {                           // fire under a long platform: you must go over it
                MakeBlock("Hurdle", cursor, cursor + 1.6f, y + hurdleH, hurdleH, curBlock, true);
                float fl = cursor + 2.2f, fr = fl + 5.4f;
                MakeBlock("Bridge", fl, fr, y + floaterRise, 0.5f, curBlock, true);
                Surface s = new Surface { left = fl, right = fr, top = y + floaterRise };
                surfaces.Add(s); floaters.Add(s);
                for (int j = 0; j < 3; j++) MakeFire(fl + 1.5f + j * 0.9f, y);
                cursor = fr + Random.Range(gapMin, gapMax);
            } else if (r < pPatrol) {                           // a flame that slides back and forth
                MakePatrolFire(cursor + 2.4f, y, 2.4f, 1.8f + 0.8f * d);
                cursor += 4.8f + Random.Range(gapMin, gapMax);
            } else {
                cursor += Random.Range(2f, 4f);                 // a plain stretch to catch your breath
            }
        }
    }

    // golden egg at the far end of the top floor
    void BuildGoal() {
        if (eggSprite == null) { Debug.LogWarning("ArenaBuilder: Egg Sprite is empty, no goal built"); return; }
        int t = floors - 1;
        float y = t * tierHeight;
        float gx = (t % 2 == 0) ? arenaWidth - 6f : 6f;

        MakeBlock("Pedestal", gx - 1.6f, gx + 1.6f, y + 0.4f, 0.4f, curBlock, true);

        float size = 1.5f;
        float s = size / eggSprite.bounds.size.x;
        GameObject go = new GameObject("Golden Egg");
        go.transform.SetParent(holder);
        go.transform.localScale = new Vector3(s, s, 1f);
        go.transform.position = new Vector3(gx, y + 0.4f + size * eggSprite.bounds.size.y / eggSprite.bounds.size.x / 2f + 0.1f, 0f);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = eggSprite;
        sr.color = new Color(1f, 0.85f, 0.25f, 1f);
        sr.sortingOrder = 1;
        CircleCollider2D cc = go.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = eggSprite.bounds.extents.x * 1.1f;
        go.AddComponent<Goal>();
        hazardSpots.Add(new Vector2(gx, y + 0.5f));               // keep pickups away from it
    }

    // ---------- pickups ----------
    bool BlockedList(Vector2 p) {
        if (p.x < 7f && p.y < 3f) return true;                  // not on the start spot
        foreach (Vector2 h in hazardSpots) if ((h - p).sqrMagnitude < 2.6f) return true;
        foreach (Vector2 q in pickupSpots) if ((q - p).sqrMagnitude < 2.6f) return true;
        return false;
    }

    bool BlockedLive(Vector2 p) {
        foreach (Collider2D c in Physics2D.OverlapCircleAll(p, 0.9f)) {
            if (c.GetComponent<Hazard>() != null || c.GetComponent<Collectible>() != null) return true;
        }
        return false;
    }

    bool Pick(List<Surface> list, float lo, float hi, float ya, float yb, float lift, bool live, out Vector2 spot) {
        spot = Vector2.zero;
        if (list.Count == 0) return false;
        for (int t = 0; t < 80; t++) {
            Surface s = list[Random.Range(0, list.Count)];
            if (s.top < ya || s.top > yb) continue;
            float l = Mathf.Max(s.left + 0.6f, lo), r = Mathf.Min(s.right - 0.6f, hi);
            if (r <= l) continue;
            Vector2 p = new Vector2(Random.Range(l, r), s.top + lift);
            if (live ? BlockedLive(p) : BlockedList(p)) continue;
            spot = p;
            return true;
        }
        return false;
    }

    void Scatter(GameObject prefab, int count, float lift) {
        if (prefab == null) return;
        int placed = 0;
        for (int tries = 0; tries < count * 40 && placed < count; tries++) {
            Vector2 p;
            if (!Pick(surfaces, 0f, arenaWidth, -100f, 1000f, lift, false, out p)) continue;
            Instantiate(prefab, p, Quaternion.identity, holder);
            pickupSpots.Add(p);
            placed++;
        }
    }

    // extra hearts sit on the floating platforms
    void PlaceHearts() {
        if (heartSprite == null || heartPickups <= 0) return;
        int placed = 0;
        for (int tries = 0; tries < 200 && placed < heartPickups; tries++) {
            Vector2 p;
            if (!Pick(floaters, 0f, arenaWidth, -100f, 1000f, 0.45f, false, out p)) continue;

            GameObject go = new GameObject("Heart Pickup");
            go.transform.SetParent(holder);
            go.transform.position = p;
            go.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = heartSprite;
            sr.sortingOrder = 1;
            CircleCollider2D cc = go.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            cc.radius = heartSprite.bounds.extents.x * 0.8f;
            Collectible c = go.AddComponent<Collectible>();
            c.kind = CollectKind.Heart;
            pickupSpots.Add(p);
            placed++;
        }
    }

    // which floor are we on, and which way does the route go (right on even floors, left on odd)
    int TierOf(float y) { return Mathf.Clamp(Mathf.FloorToInt((y + 1f) / tierHeight), 0, floors - 1); }
    public int ForwardDir(float y) { return TierOf(y) % 2 == 0 ? 1 : -1; }

    // used by PlayerHealth: a free spot ahead of the player on the same floor
    public bool TryPickSpotAhead(float fromX, float fromY, float minAhead, float maxAhead, float lift, out Vector2 spot) {
        int tier = TierOf(fromY);
        float ya = tier * tierHeight - 0.2f, yb = tier * tierHeight + floaterRise + 0.2f;
        int dir = ForwardDir(fromY);
        float x1 = fromX + dir * minAhead, x2 = fromX + dir * maxAhead;
        if (Pick(surfaces, Mathf.Min(x1, x2), Mathf.Max(x1, x2), ya, yb, lift, true, out spot)) return true;
        if (Pick(surfaces, 0f, arenaWidth, ya, yb, lift, true, out spot)) return true;      // end of the floor: anywhere on this floor
        return Pick(surfaces, 0f, arenaWidth, -100f, 1000f, lift, true, out spot);          // last resort: anywhere
    }

    // ---------- player, camera, look ----------
    void PlacePlayer() {
        PlayerLife pl = FindFirstObjectByType<PlayerLife>();
        if (pl == null) return;
        player = pl.transform;
        pl.transform.position = new Vector3(3f, 0.8f, pl.transform.position.z);
        Rigidbody2D rb = pl.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        Physics2D.SyncTransforms();
    }

    void SetupCamera() {
        cam = Camera.main;
        CameraFollow cf = FindFirstObjectByType<CameraFollow>();
        if (cf != null) cf.SetBounds(new Rect(0f, -1f, arenaWidth, ceilingY + 2f));
        if (cam != null) cam.clearFlags = CameraClearFlags.SolidColor;
    }

    // the sky goes from day blue to night as you climb
    void Update() {
        if (cam == null || player == null) return;
        float t = Mathf.Clamp01(player.position.y / Mathf.Max(1f, ceilingY - 9f)) * (skyStops.Length - 1);
        int i = Mathf.Min((int)t, skyStops.Length - 2);
        cam.backgroundColor = Color.Lerp(skyStops[i], skyStops[i + 1], t - i);
    }

    // pixel art must never be smoothed (blurry): force Point filtering on every pixel-art texture
    void SharpenPixelArt() {
        string[] names = { "fire", "rock", "worm", "egg", "heart", "roasted" };
        foreach (Texture2D tex in Resources.FindObjectsOfTypeAll<Texture2D>()) {
            foreach (string n in names) if (tex.name == n) tex.filterMode = FilterMode.Point;
        }
    }
}