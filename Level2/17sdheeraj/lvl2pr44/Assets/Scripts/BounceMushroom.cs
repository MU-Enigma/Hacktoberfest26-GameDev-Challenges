using UnityEngine;

public class BounceMushroom : MonoBehaviour
{
    [HideInInspector] public bool isMushroomPlanted = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.M) && cooldownTimer <= 0f)
        {
            PlantMushroom();
        }
    }

    public void PlantMushroom()
    {
        if (cooldownTimer > 0f) return;
        var shroom = new GameObject("BounceMushroomObj");
        shroom.transform.position = transform.position;
        var b = shroom.AddComponent<BounceMushroomBehavior>();
        b.Init();
        isMushroomPlanted = true;
        cooldownTimer = 6f;
    }
}

public class BounceMushroomBehavior : MonoBehaviour
{
    private float life = 12f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.4f);
        float r = res * 0.45f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                if (Vector2.Distance(new Vector2(x, y), c) <= r && y >= res * 0.35f)
                    tex.SetPixel(x, y, new Color(0.9f, 0.2f, 0.2f, 1f));
                else if (Mathf.Abs(x - res * 0.5f) < 4 && y < res * 0.4f)
                    tex.SetPixel(x, y, new Color(0.9f, 0.85f, 0.75f, 1f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 2;
    }

    void Update()
    {
        life -= Time.deltaTime;
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.2f)
        {
            croc.transform.position += Vector3.up * 4f;
            Destroy(gameObject);
        }

        if (life <= 0f) Destroy(gameObject);
    }
}