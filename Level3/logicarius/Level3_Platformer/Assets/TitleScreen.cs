using UnityEngine;

// Title screen drawn with plain OnGUI (no Canvas or UI package needed).
// Put this on an empty GameObject. The game is paused until Play is pressed.
public class TitleScreen : MonoBehaviour {
    public string title = "CHICKEN CLIMB";
    public string subtitle = "Climb the stairs. Don't get roasted.";
    public Sprite chickenSprite;                                   // drag the chicken sprite here
    public Color backgroundColor = new Color(0.16f, 0.07f, 0.30f, 1f);
    public Color titleColor = new Color(1f, 0.55f, 0.80f, 1f);

    bool showing;
    GUIStyle titleStyle, subStyle, buttonStyle, smallStyle;

    void Awake() { Show(); }

    void Show() { showing = true; Time.timeScale = 0f; }
    void Begin() { showing = false; Time.timeScale = 1f; }

    void Update() {
        if (showing) {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) Begin();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            Show();
        }
    }

    void MakeStyles(float sh) {
        if (titleStyle == null) {
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontStyle = FontStyle.Bold;
            subStyle = new GUIStyle(GUI.skin.label);
            subStyle.alignment = TextAnchor.MiddleCenter;
            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.alignment = TextAnchor.UpperCenter;
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
        }
        titleStyle.fontSize = Mathf.RoundToInt(sh * 0.09f);
        titleStyle.normal.textColor = titleColor;
        subStyle.fontSize = Mathf.RoundToInt(sh * 0.035f);
        subStyle.normal.textColor = Color.white;
        smallStyle.fontSize = Mathf.RoundToInt(sh * 0.026f);
        smallStyle.normal.textColor = new Color(0.8f, 0.8f, 0.9f, 1f);
        buttonStyle.fontSize = Mathf.RoundToInt(sh * 0.05f);
    }

    void OnGUI() {
        if (!showing) return;
        GUI.depth = -1000;                       // draw on top of everything else
        float sw = Screen.width, sh = Screen.height;
        MakeStyles(sh);

        GUI.color = backgroundColor;
        GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (chickenSprite != null) {
            Texture2D tex = chickenSprite.texture;
            tex.filterMode = FilterMode.Point;       // sharp pixels
            Rect tr = chickenSprite.textureRect;
            Rect uv = new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);
            float h = sh * 0.28f;
            float w = h * tr.width / tr.height;
            float bob = Mathf.Sin(Time.unscaledTime * 3f) * sh * 0.012f;
            GUI.DrawTextureWithTexCoords(new Rect((sw - w) / 2f, sh * 0.10f + bob, w, h), tex, uv);
        }

        GUI.Label(new Rect(0, sh * 0.40f, sw, sh * 0.13f), title, titleStyle);
        GUI.Label(new Rect(0, sh * 0.53f, sw, sh * 0.06f), subtitle, subStyle);

        float bw = sh * 0.42f, bh = sh * 0.10f;
        if (GUI.Button(new Rect((sw - bw) / 2f, sh * 0.63f, bw, bh), "PLAY", buttonStyle)) Begin();

        GUI.Label(new Rect(0, sh * 0.76f, sw, sh * 0.04f), "or press Enter", smallStyle);
        Color[] bands = { new Color(1f,0.5f,0.75f), new Color(1f,0.78f,0.3f), new Color(1f,0.92f,0.4f), new Color(0.6f,1f,0.45f), new Color(0.35f,0.95f,0.9f), new Color(0.55f,0.5f,1f), new Color(0.8f,0.5f,1f) };
        for (int b = 0; b < bands.Length; b++) {
            GUI.color = bands[b];
            GUI.DrawTexture(new Rect(b * sw / bands.Length, sh * 0.975f, sw / bands.Length + 1f, sh * 0.025f), Texture2D.whiteTexture);
        }
        GUI.color = Color.white;

        GUI.Label(new Rect(0, sh * 0.84f, sw, sh * 0.12f),
            "Move: A / D     Jump: W     Super jump: Space     Fast fall: S\nEsc: back to this screen", smallStyle);
    }
}