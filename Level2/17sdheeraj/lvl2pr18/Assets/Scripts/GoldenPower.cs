using UnityEngine;

public class GoldenPower : MonoBehaviour
{
    [HideInInspector]
    public bool isGolden = false;
    [HideInInspector]
    public float goldenTimer = 0f;

    private SpriteRenderer sr;
    private Color originalColor = Color.white;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    void Update()
    {
        if (isGolden)
        {
            goldenTimer -= Time.deltaTime;

            if (sr != null)
            {
                float pulse = (Mathf.Sin(Time.time * 10f) + 1f) * 0.5f;
                sr.color = Color.Lerp(new Color(1f, 0.85f, 0.1f), new Color(1f, 1f, 0.6f), pulse);
            }

            if (goldenTimer <= 0f)
            {
                isGolden = false;
                if (sr != null) sr.color = originalColor;
            }
        }
    }

    public void ActivateGolden(float duration)
    {
        isGolden = true;
        goldenTimer = duration;
    }

    void OnGUI()
    {
        if (isGolden)
        {
            GUIStyle goldStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            goldStyle.normal.textColor = Color.yellow;
            GUI.Box(new Rect(20, 120, 190, 36), "GOLDEN: " + goldenTimer.ToString("F1") + "s", goldStyle);
        }
    }
}
