using UnityEngine;

public class JetpackGlider : MonoBehaviour
{
    [HideInInspector] public bool isGliding = false;
    public float cooldownTimer = 0f;
    private float glideTimer = 0f;
    private Rigidbody2D rb;
    private GameObject bubblesEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J) && cooldownTimer <= 0f && !isGliding)
        {
            StartGliding();
        }

        if (isGliding)
        {
            glideTimer -= Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(Time.time * 8f) * 0.02f;
            if (glideTimer <= 0f)
            {
                EndGliding();
            }
        }
    }

    public void StartGliding()
    {
        if (cooldownTimer > 0f || isGliding) return;
        isGliding = true;
        glideTimer = 3.5f;
        cooldownTimer = 7f;

        bubblesEffect = new GameObject("SodaBubbles");
        bubblesEffect.transform.SetParent(transform);
        bubblesEffect.transform.localPosition = new Vector3(0, -0.4f, 0);
        var sr = bubblesEffect.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, (Vector2.Distance(new Vector2(x, y), new Vector2(res/2f, res/2f)) < res/2f - 1f) ? new Color(0.2f, 0.8f, 1f, 0.8f) : Color.clear);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 6;
    }

    public void EndGliding()
    {
        isGliding = false;
        if (bubblesEffect != null) Destroy(bubblesEffect);
    }
}