using UnityEngine;

public class CheeseWheel : MonoBehaviour
{
    [HideInInspector] public bool isRollingCheese = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.C) && cooldownTimer <= 0f)
        {
            RollCheese();
        }
    }

    public void RollCheese()
    {
        if (cooldownTimer > 0f) return;
        var cheese = new GameObject("CheeseWheelObj");
        cheese.transform.position = transform.position;
        var b = cheese.AddComponent<CheeseWheelBehavior>();
        b.Init();
        isRollingCheese = true;
        cooldownTimer = 5f;
    }
}

public class CheeseWheelBehavior : MonoBehaviour
{
    private float life = 3f;

    public void Init()
    {
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
                    tex.SetPixel(x, y, new Color(1f, 0.85f, 0.2f, 1f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 4;
        transform.localScale = Vector3.one * 1.5f;
    }

    void Update()
    {
        life -= Time.deltaTime;
        transform.position += Vector3.right * 7f * Time.deltaTime;
        transform.Rotate(0, 0, -360f * Time.deltaTime);

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.5f)
        {
            croc.transform.position += Vector3.right * 3f;
        }

        if (life <= 0f) Destroy(gameObject);
    }
}