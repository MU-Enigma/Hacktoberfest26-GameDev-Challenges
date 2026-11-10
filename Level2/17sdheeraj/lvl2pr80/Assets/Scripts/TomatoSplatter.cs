using UnityEngine;

public class TomatoSplatter : MonoBehaviour
{
    [HideInInspector] public bool isSplattering = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.T) && cooldownTimer <= 0f)
        {
            ThrowTomato();
        }
    }

    public void ThrowTomato()
    {
        if (cooldownTimer > 0f) return;
        var t = new GameObject("TomatoObj");
        t.transform.position = transform.position;
        var b = t.AddComponent<TomatoBehavior>();
        b.Init();
        isSplattering = true;
        cooldownTimer = 4.5f;
    }
}

public class TomatoBehavior : MonoBehaviour
{
    private float timer = 1.2f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 24;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(0.9f, 0.2f, 0.15f));
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
            croc.transform.position += Vector3.right * 2.5f;
            Destroy(gameObject);
        }

        if (timer <= 0f) Destroy(gameObject);
    }
}