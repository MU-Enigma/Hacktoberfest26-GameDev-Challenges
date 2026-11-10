using UnityEngine;

public class ConfettiCannon : MonoBehaviour
{
    [HideInInspector] public bool isFiringConfetti = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K) && cooldownTimer <= 0f)
        {
            FireConfetti();
        }
    }

    public void FireConfetti()
    {
        if (cooldownTimer > 0f) return;
        for (int i = 0; i < 8; i++)
        {
            var p = new GameObject("ConfettiRibbon_" + i);
            p.transform.position = transform.position;
            var b = p.AddComponent<ConfettiRibbonBehavior>();
            b.Init(new Vector3(Random.Range(1f, 4f), Random.Range(-2f, 2f), 0));
        }
        cooldownTimer = 5f;
    }
}

public class ConfettiRibbonBehavior : MonoBehaviour
{
    private Vector3 vel;
    private float timer = 1.2f;

    public void Init(Vector3 v)
    {
        vel = v;
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Color c = new Color(Random.value, Random.value, Random.value, 1f);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, c);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 6;
        transform.localScale = Vector3.one * 0.3f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += vel * Time.deltaTime;
        transform.Rotate(0, 0, 360f * Time.deltaTime);
        if (timer <= 0f) Destroy(gameObject);
    }
}