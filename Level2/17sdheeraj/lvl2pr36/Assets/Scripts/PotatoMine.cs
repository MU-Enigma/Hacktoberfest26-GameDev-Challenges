using UnityEngine;

public class PotatoMine : MonoBehaviour
{
    [HideInInspector] public bool isMineActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J) && cooldownTimer <= 0f)
        {
            PlantMine();
        }
    }

    public void PlantMine()
    {
        if (cooldownTimer > 0f) return;
        var mine = new GameObject("PotatoMineObj");
        mine.transform.position = transform.position;
        var b = mine.AddComponent<PotatoMineBehavior>();
        b.Init();
        isMineActive = true;
        cooldownTimer = 6f;
    }
}

public class PotatoMineBehavior : MonoBehaviour
{
    private SpriteRenderer sr;
    private float armTimer = 1f;

    public void Init()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        float r = res * 0.4f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                if (Vector2.Distance(new Vector2(x, y), c) <= r)
                    tex.SetPixel(x, y, new Color(0.7f, 0.5f, 0.25f, 1f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 2;
        transform.localScale = Vector3.one * 0.7f;
    }

    void Update()
    {
        if (armTimer > 0f)
        {
            armTimer -= Time.deltaTime;
            return;
        }

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.4f)
        {
            Vector3 blast = (croc.transform.position - transform.position).normalized;
            croc.transform.position += blast * 4f;
            Destroy(gameObject);
        }
    }
}