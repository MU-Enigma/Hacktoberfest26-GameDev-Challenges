using UnityEngine;

public class SteamCloud : MonoBehaviour
{
    [HideInInspector] public bool isCloudActive = false;
    public float cooldownTimer = 0f;
    private GameObject cloudObj;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && cooldownTimer <= 0f)
        {
            DeploySteam();
        }
    }

    public void DeploySteam()
    {
        if (cooldownTimer > 0f) return;
        cloudObj = new GameObject("SteamCloudObject");
        cloudObj.transform.position = transform.position;
        var sc = cloudObj.AddComponent<SteamCloudBehavior>();
        sc.Init();
        isCloudActive = true;
        cooldownTimer = 6f;
    }
}

public class SteamCloudBehavior : MonoBehaviour
{
    private SpriteRenderer sr;
    private float timer = 4f;

    public void Init()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 64;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        float r = res * 0.45f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                if (d <= r)
                {
                    float a = Mathf.Clamp01(1f - (d / r)) * 0.6f;
                    tex.SetPixel(x, y, new Color(0.9f, 0.95f, 1f, a));
                }
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
        timer -= Time.deltaTime;
        transform.localScale += Vector3.one * 0.8f * Time.deltaTime;
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Clamp01(timer / 4f) * 0.6f;
            sr.color = c;
        }

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < transform.localScale.x * 1.2f)
        {
            croc.transform.position += (Vector3)(Random.insideUnitCircle * 0.03f);
        }

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}