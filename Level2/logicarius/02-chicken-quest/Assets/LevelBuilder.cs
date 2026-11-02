using UnityEngine;

// Builds the staircase level, flags, camera follow and the finish message at runtime,
// so nothing has to be placed by hand in the scene. It only needs a PlayerMovement object.
public class LevelBuilder : MonoBehaviour {
    static LevelBuilder instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot() {
        if (FindFirstObjectByType<LevelBuilder>() != null) return;
        new GameObject("LevelBuilder").AddComponent<LevelBuilder>();
    }

    public static void Say(string text) {
        if (instance != null) instance.ShowHint(text);
    }

    public static bool CanTeleportTo(Vector2 p) {
        return instance == null || instance.InCorridor(p);
    }

    // level layout, measured in "chicken widths"
    const int Steps = 7;       // number of stairs
    const float Run = 4f;      // length of each stair
    const float Rise = 1.2f;   // how much each stair goes up
    const float Gap = 3.8f;    // height of the passage (giant mode still fits)

    PlayerMovement player;
    Camera cam;
    Sprite whiteSprite;
    float u;                   // one chicken width in world units
    Vector3 startPos;
    Vector2 finishCenter;
    bool finished;
    float finishTimer, hintTimer;
    string hint = "";
    GUIStyle bigStyle, midStyle, smallStyle;

    void Awake() {
        instance = this;
    }

    void Start() {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null) {
            Debug.LogWarning("LevelBuilder: no PlayerMovement object found in this scene.");
            enabled = false;
            return;
        }
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        u = sr != null ? sr.bounds.size.x : 1f;
        if (u < 0.05f) u = 1f;

        whiteSprite = MakeWhiteSprite();
        BuildLevel();

        cam = Camera.main;
        if (cam != null) {
            cam.orthographic = true;
            cam.orthographicSize = 4.5f * u;
        }
        Respawn();
    }

    void BuildLevel() {
        Transform root = new GameObject("Staircase").transform;
        float run = Run * u, rise = Rise * u, gap = Gap * u, t = 2f * u;

        for (int i = 0; i < Steps; i++) {
            float x0 = i * run;
            float floorTop = i * rise;
            float shade = (i % 2 == 0) ? 1f : 0.82f;   // alternate shades so the steps are easy to see
            MakeBlock(root, "Floor" + i, x0 + run / 2f, floorTop - t / 2f, run, t,
                      new Color(0.55f * shade, 0.38f * shade, 0.22f * shade));
            MakeBlock(root, "Ceiling" + i, x0 + run / 2f, floorTop + gap + t / 2f, run, t,
                      new Color(0.30f * shade, 0.32f * shade, 0.45f * shade));
        }

        float lastTop = (Steps - 1) * rise;
        float capH = gap + 2f * t;
        Color wall = new Color(0.25f, 0.25f, 0.3f);
        MakeBlock(root, "WallStart", -t / 2f, gap / 2f, t, capH, wall);
        MakeBlock(root, "WallEnd", Steps * run + t / 2f, lastTop + gap / 2f, t, capH, wall);

        float signSize = 2.8f * u;
        MakeDecor(root, "StartSign", LoadSprite("sign_start"), new Vector3(1.6f * u, 0f, 0f), signSize,
                  new Color(0.8f, 0.55f, 0.3f));
        float flagX = Steps * run - 1.6f * u;
        MakeDecor(root, "FinishFlag", LoadSprite("flag_finish"), new Vector3(flagX, lastTop, 0f), signSize,
                  Color.white);

        startPos = new Vector3(3.2f * u, 0.7f * u, 0f);
        finishCenter = new Vector2(flagX, lastTop + 1.2f * u);
    }

    void MakeBlock(Transform parent, string name, float cx, float cy, float w, float h, Color c) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(cx, cy, 0f);
        go.transform.localScale = new Vector3(w, h, 1f);
        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sprite = whiteSprite;
        r.color = c;
        r.sortingOrder = -5;
        BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
        bc.size = Vector2.one;
    }

    void MakeDecor(Transform parent, string name, Sprite sprite, Vector3 bottomCenter, float size, Color fallback) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = bottomCenter;
        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sortingOrder = -1;
        if (sprite != null) {
            r.sprite = sprite;
            go.transform.localScale = new Vector3(size, size, 1f);
        } else {
            // image missing: show a plain coloured post instead of nothing
            r.sprite = whiteSprite;
            r.color = fallback;
            go.transform.position = bottomCenter + new Vector3(0f, size / 2f, 0f);
            go.transform.localScale = new Vector3(size * 0.2f, size, 1f);
        }
    }

    Sprite MakeWhiteSprite() {
        Texture2D tex = new Texture2D(4, 4);
        Color[] px = new Color[16];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    // loads Assets/Resources/<name>.png as a sprite that is 1 world unit wide, pivot at the bottom
    Sprite LoadSprite(string resourceName) {
        Texture2D tex = Resources.Load<Texture2D>(resourceName);
        if (tex == null) {
            Debug.LogWarning("LevelBuilder: could not find Resources/" + resourceName + ".png");
            return null;
        }
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f), tex.width);
    }

    bool InCorridor(Vector2 p) {
        float run = Run * u, rise = Rise * u, gap = Gap * u;
        if (p.x < 0f || p.x > Steps * run) return false;
        int i = Mathf.Clamp(Mathf.FloorToInt(p.x / run), 0, Steps - 1);
        float floorTop = i * rise;
        return p.y > floorTop && p.y < floorTop + gap;
    }

    void ShowHint(string text) {
        hint = text;
        hintTimer = 2.5f;
    }

    void Update() {
        if (hintTimer > 0f) hintTimer -= Time.unscaledDeltaTime;

        if (finished) {
            finishTimer -= Time.unscaledDeltaTime;
            if (finishTimer <= 0f) Respawn();
            return;
        }
        if (Vector2.Distance(player.transform.position, finishCenter) < 1.4f * u) {
            finished = true;
            finishTimer = 3f;
            player.frozen = true;
        }
    }

    void Respawn() {
        player.ResetState();
        player.transform.position = startPos;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.position = startPos;
            rb.linearVelocity = Vector2.zero;
        }
        player.frozen = false;
        finished = false;
        if (cam != null) {
            cam.transform.position = new Vector3(startPos.x, startPos.y, cam.transform.position.z);
        }
    }

    void LateUpdate() {
        if (cam == null || player == null) return;
        Vector3 target = player.transform.position;
        target.z = cam.transform.position.z;
        float k = Mathf.Min(1f, 6f * Time.unscaledDeltaTime);
        cam.transform.position = Vector3.Lerp(cam.transform.position, target, k);
    }

    void OnGUI() {
        float h = Screen.height;
        float w = Screen.width;
        if (bigStyle == null) {
            bigStyle = new GUIStyle(GUI.skin.label);
            bigStyle.fontStyle = FontStyle.Bold;
            bigStyle.alignment = TextAnchor.MiddleCenter;
            bigStyle.wordWrap = false;
            midStyle = new GUIStyle(GUI.skin.label);
            midStyle.alignment = TextAnchor.MiddleCenter;
            midStyle.wordWrap = false;
            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.alignment = TextAnchor.UpperLeft;
            smallStyle.wordWrap = false;
        }
        // text sizes follow the window height so nothing overflows in small Game views
        bigStyle.fontSize = Mathf.Max(18, Mathf.RoundToInt(h / 15f));
        midStyle.fontSize = Mathf.Max(12, Mathf.RoundToInt(h / 30f));
        smallStyle.fontSize = Mathf.Max(11, Mathf.RoundToInt(h / 45f));

        if (finished) {
            // dim the whole screen, then show the message on a clean banner
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(0, 0, w, h), Texture2D.whiteTexture);
            float bannerH = bigStyle.fontSize * 3.2f;
            float bannerY = (h - bannerH) / 2f;
            GUI.color = new Color(0.05f, 0.08f, 0.15f, 0.9f);
            GUI.DrawTexture(new Rect(0, bannerY, w, bannerH), Texture2D.whiteTexture);
            GUI.color = Color.white;
            DrawShadowed(new Rect(0, bannerY + bigStyle.fontSize * 0.3f, w, bigStyle.fontSize * 1.5f),
                         "You have finished the game!", bigStyle);
            DrawShadowed(new Rect(0, bannerY + bigStyle.fontSize * 1.9f, w, midStyle.fontSize * 1.6f),
                         "Heading back to the start...", midStyle);
            return;
        }

        // controls panel, always visible while playing
        string controls =
            "CONTROLS\n" +
            "WASD / Arrows  -  move\n" +
            "Shift / Space  -  dash\n" +
            "E  -  giant size\n" +
            "C  -  rainbow colours\n" +
            "G  -  ghost mode\n" +
            "T (hold)  -  slow time\n" +
            "F  -  chick parade\n" +
            "Left click  -  teleport";
        float lineH = smallStyle.fontSize * 1.45f;
        float panelW = smallStyle.fontSize * 15f;
        float panelH = lineH * 9f + 14f;
        GUI.color = new Color(0f, 0f, 0f, 0.55f);
        GUI.DrawTexture(new Rect(10, 10, panelW, panelH), Texture2D.whiteTexture);
        GUI.color = Color.white;
        smallStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(18, 14, panelW - 10, panelH), controls, smallStyle);

        if (hintTimer > 0f) {
            DrawShadowed(new Rect(0, 12, w, midStyle.fontSize * 1.8f), hint, midStyle);
        }
    }

    void DrawShadowed(Rect r, string text, GUIStyle style) {
        style.normal.textColor = new Color(0f, 0f, 0f, 0.8f);
        GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), text, style);
        style.normal.textColor = Color.white;
        GUI.Label(r, text, style);
    }
}