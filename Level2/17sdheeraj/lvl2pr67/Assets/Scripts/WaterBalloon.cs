using UnityEngine;

public class WaterBalloon : MonoBehaviour
{
    [HideInInspector] public bool isBalloonFlying = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha2) && cooldownTimer <= 0f)
        {
            ThrowBalloon();
        }
    }

    public void ThrowBalloon()
    {
        if (cooldownTimer > 0f) return;
        var b = new GameObject("WaterBalloonObj");
        b.transform.position = transform.position;
        var comp = b.AddComponent<WaterBalloonBehavior>();
        comp.Init();
        isBalloonFlying = true;
        cooldownTimer = 5f;
    }
}

public class WaterBalloonBehavior : MonoBehaviour
{
    private float timer = 1.5f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 24;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(0.2f, 0.7f, 1f, 0.85f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
        transform.localScale = Vector3.one * 0.6f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += Vector3.right * 8f * Time.deltaTime;

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.3f)
        {
            croc.transform.position += Vector3.right * 3f;
            Destroy(gameObject);
        }

        if (timer <= 0f) Destroy(gameObject);
    }
}