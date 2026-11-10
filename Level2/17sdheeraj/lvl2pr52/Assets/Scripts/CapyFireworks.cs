using UnityEngine;

public class CapyFireworks : MonoBehaviour
{
    [HideInInspector] public bool isLaunching = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && cooldownTimer <= 0f)
        {
            LaunchFireworks();
        }
    }

    public void LaunchFireworks()
    {
        if (cooldownTimer > 0f) return;
        for (int i = 0; i < 3; i++)
        {
            var fw = new GameObject("FireworkRocket_" + i);
            fw.transform.position = transform.position + new Vector3((i - 1) * 0.8f, 0, 0);
            var b = fw.AddComponent<FireworkRocketBehavior>();
            b.Init(i * 0.2f);
        }
        cooldownTimer = 6f;
    }
}

public class FireworkRocketBehavior : MonoBehaviour
{
    private float delay;
    private float timer = 0f;
    private SpriteRenderer sr;

    public void Init(float d)
    {
        delay = d;
        sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, Color.yellow);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < delay) return;

        transform.position += Vector3.up * 7f * Time.deltaTime;
        if (timer > delay + 0.8f)
        {
            Destroy(gameObject);
        }
    }
}