using UnityEngine;

public class YuzuBoomerang : MonoBehaviour
{
    [HideInInspector] public bool isBoomerangFlying = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.O) && cooldownTimer <= 0f && !isBoomerangFlying)
        {
            ThrowBoomerang();
        }
    }

    public void ThrowBoomerang()
    {
        if (cooldownTimer > 0f) return;
        var yuzu = new GameObject("YuzuBoomerangObj");
        yuzu.transform.position = transform.position;
        var b = yuzu.AddComponent<YuzuBoomerangBehavior>();
        b.Init(this, transform);
        isBoomerangFlying = true;
        cooldownTimer = 4.5f;
    }
}

public class YuzuBoomerangBehavior : MonoBehaviour
{
    private YuzuBoomerang owner;
    private Transform player;
    private float life = 0f;
    private Vector3 origin;
    private Vector3 flyDir;

    public void Init(YuzuBoomerang own, Transform p)
    {
        owner = own;
        player = p;
        origin = p.position;
        flyDir = Vector3.right;
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        float r = res * 0.45f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                if (Vector2.Distance(new Vector2(x, y), c) <= r)
                    tex.SetPixel(x, y, new Color(1f, 0.85f, 0.1f, 1f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
    }

    void Update()
    {
        life += Time.deltaTime;
        transform.Rotate(0, 0, 720f * Time.deltaTime);

        float forward = Mathf.Sin(life * Mathf.PI * 0.8f) * 6f;
        float side = Mathf.Cos(life * Mathf.PI * 0.8f) * 2f;
        transform.position = origin + flyDir * forward + Vector3.up * side;

        if (life > 2.5f)
        {
            if (owner != null) owner.isBoomerangFlying = false;
            Destroy(gameObject);
        }
    }
}