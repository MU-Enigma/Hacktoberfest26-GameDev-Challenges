using UnityEngine;

public class BubbleGumPop : MonoBehaviour
{
    [HideInInspector] public bool isGumActive = false;
    public float cooldownTimer = 0f;
    private GameObject gumObj;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha5) && cooldownTimer <= 0f)
        {
            BlowGum();
        }
    }

    public void BlowGum()
    {
        if (cooldownTimer > 0f) return;
        gumObj = new GameObject("BubbleGumVisual");
        gumObj.transform.position = transform.position;
        var b = gumObj.AddComponent<BubbleGumBehavior>();
        b.Init();
        isGumActive = true;
        cooldownTimer = 6f;
    }
}

public class BubbleGumBehavior : MonoBehaviour
{
    private float timer = 4f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(1f, 0.4f, 0.7f, 0.8f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 3;
        transform.localScale = Vector3.one * 1.4f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.5f)
        {
            var rb = croc.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (timer <= 0f) Destroy(gameObject);
    }
}