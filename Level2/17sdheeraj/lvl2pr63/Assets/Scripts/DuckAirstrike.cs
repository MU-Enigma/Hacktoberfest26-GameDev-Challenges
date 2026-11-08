using UnityEngine;

public class DuckAirstrike : MonoBehaviour
{
    [HideInInspector] public bool isAirstrikeActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.D) && cooldownTimer <= 0f)
        {
            CallAirstrike();
        }
    }

    public void CallAirstrike()
    {
        if (cooldownTimer > 0f) return;
        for (int i = 0; i < 3; i++)
        {
            var duck = new GameObject("AirstrikeDuck_" + i);
            duck.transform.position = transform.position + new Vector3(-6f - i * 1.5f, 3f + i * 0.5f, 0);
            var b = duck.AddComponent<AirstrikeDuckBehavior>();
            b.Init();
        }
        cooldownTimer = 10f;
    }
}

public class AirstrikeDuckBehavior : MonoBehaviour
{
    private float timer = 4f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(1f, 0.9f, 0.2f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 6;
        transform.localScale = Vector3.one * 0.6f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += Vector3.right * 5f * Time.deltaTime;
        if (timer <= 0f) Destroy(gameObject);
    }
}