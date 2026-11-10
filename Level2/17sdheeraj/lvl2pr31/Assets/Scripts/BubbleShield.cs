using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    [HideInInspector] public bool isShieldActive = false;
    public float cooldownTimer = 0f;
    private GameObject shieldObj;
    private SpriteRenderer shieldSr;
    private float shieldDuration = 5f;
    private float activeTimer = 0f;

    void Awake()
    {
        shieldObj = new GameObject("BubbleShieldVisual");
        shieldObj.transform.SetParent(transform);
        shieldObj.transform.localPosition = Vector3.zero;
        shieldObj.transform.localScale = new Vector3(2.4f, 2.4f, 1f);
        shieldSr = shieldObj.AddComponent<SpriteRenderer>();
        shieldSr.sprite = CreateCircleSprite(new Color(0.3f, 0.8f, 1f, 0.45f));
        shieldSr.sortingOrder = 5;
        shieldObj.SetActive(false);
    }

    Sprite CreateCircleSprite(Color col)
    {
        int res = 64;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        float r = res * 0.48f;
        Vector2 c = new Vector2(res * 0.5f, res * 0.5f);
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                if (d <= r)
                {
                    float edge = Mathf.Abs(d - r);
                    float a = edge < 3f ? 0.8f : 0.35f;
                    tex.SetPixel(x, y, new Color(col.r, col.g, col.b, a));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K) && cooldownTimer <= 0f && !isShieldActive)
        {
            ToggleShield();
        }

        if (isShieldActive)
        {
            activeTimer -= Time.deltaTime;
            float pulse = 2.4f + Mathf.Sin(Time.time * 6f) * 0.15f;
            shieldObj.transform.localScale = new Vector3(pulse, pulse, 1f);

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 2.5f)
            {
                Vector3 pushDir = (croc.transform.position - transform.position).normalized;
                croc.transform.position += pushDir * 5f * Time.deltaTime;
            }

            if (activeTimer <= 0f)
            {
                isShieldActive = false;
                shieldObj.SetActive(false);
            }
        }
    }

    public void ToggleShield()
    {
        if (cooldownTimer > 0f) return;
        isShieldActive = true;
        activeTimer = shieldDuration;
        shieldObj.SetActive(true);
        cooldownTimer = 7f;
    }
}