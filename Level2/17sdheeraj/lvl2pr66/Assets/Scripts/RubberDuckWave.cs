using UnityEngine;

public class RubberDuckWave : MonoBehaviour
{
    [HideInInspector] public bool isWaveActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1) && cooldownTimer <= 0f)
        {
            ReleaseWave();
        }
    }

    public void ReleaseWave()
    {
        if (cooldownTimer > 0f) return;
        for (int i = 0; i < 5; i++)
        {
            var duck = new GameObject("RubberDuckieWave_" + i);
            duck.transform.position = transform.position + new Vector3(i * 0.4f, (i - 2) * 0.3f, 0);
            var b = duck.AddComponent<RubberDuckWaveBehavior>();
            b.Init();
        }
        isWaveActive = true;
        cooldownTimer = 6f;
    }
}

public class RubberDuckWaveBehavior : MonoBehaviour
{
    private float timer = 2.5f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 24;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, (x > 4 && x < 20 && y > 4 && y < 18) ? new Color(1f, 0.9f, 0.1f) : Color.clear);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
        transform.localScale = Vector3.one * 0.7f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += Vector3.right * 6f * Time.deltaTime;

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.2f)
        {
            croc.transform.position += Vector3.right * 4f * Time.deltaTime;
        }

        if (timer <= 0f) Destroy(gameObject);
    }
}