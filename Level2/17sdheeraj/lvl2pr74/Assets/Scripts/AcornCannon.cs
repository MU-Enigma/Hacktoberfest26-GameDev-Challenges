using UnityEngine;

public class AcornCannon : MonoBehaviour
{
    [HideInInspector] public bool isFiringAcorns = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha9) && cooldownTimer <= 0f)
        {
            FireAcorns();
        }
    }

    public void FireAcorns()
    {
        if (cooldownTimer > 0f) return;
        for (int i = 0; i < 3; i++)
        {
            var acorn = new GameObject("AcornBullet_" + i);
            acorn.transform.position = transform.position + new Vector3(0.5f, (i - 1) * 0.3f, 0);
            var b = acorn.AddComponent<AcornBulletBehavior>();
            b.Init();
        }
        cooldownTimer = 4.5f;
    }
}

public class AcornBulletBehavior : MonoBehaviour
{
    private float life = 1.2f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(0.6f, 0.4f, 0.2f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
        transform.localScale = Vector3.one * 0.5f;
    }

    void Update()
    {
        life -= Time.deltaTime;
        transform.position += Vector3.right * 9f * Time.deltaTime;

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.2f)
        {
            croc.transform.position += Vector3.right * 2f;
            Destroy(gameObject);
        }

        if (life <= 0f) Destroy(gameObject);
    }
}