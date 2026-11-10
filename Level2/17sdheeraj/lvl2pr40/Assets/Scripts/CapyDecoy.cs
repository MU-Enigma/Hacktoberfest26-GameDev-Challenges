using UnityEngine;

public class CapyDecoy : MonoBehaviour
{
    [HideInInspector] public bool isDecoyActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.D) && cooldownTimer <= 0f)
        {
            SpawnDecoy();
        }
    }

    public void SpawnDecoy()
    {
        if (cooldownTimer > 0f) return;
        var decoy = new GameObject("CapyDecoyDoll");
        decoy.transform.position = transform.position;
        var b = decoy.AddComponent<DecoyDollBehavior>();
        b.Init();
        isDecoyActive = true;
        cooldownTimer = 9f;
    }
}

public class DecoyDollBehavior : MonoBehaviour
{
    private float timer = 5f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 48;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        float r = res * 0.45f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                if (Vector2.Distance(new Vector2(x, y), c) <= r)
                    tex.SetPixel(x, y, new Color(0.55f, 0.35f, 0.2f, 1f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 3;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null)
        {
            croc.transform.position = Vector3.MoveTowards(croc.transform.position, transform.position, 2.5f * Time.deltaTime);
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}