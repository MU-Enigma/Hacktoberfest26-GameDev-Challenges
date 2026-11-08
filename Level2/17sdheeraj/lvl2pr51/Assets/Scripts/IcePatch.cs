using UnityEngine;

public class IcePatch : MonoBehaviour
{
    [HideInInspector] public bool isIceActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.I) && cooldownTimer <= 0f)
        {
            PlaceIce();
        }
    }

    public void PlaceIce()
    {
        if (cooldownTimer > 0f) return;
        var ice = new GameObject("IcePatchObj");
        ice.transform.position = transform.position;
        var b = ice.AddComponent<IcePatchBehavior>();
        b.Init();
        isIceActive = true;
        cooldownTimer = 6f;
    }
}

public class IcePatchBehavior : MonoBehaviour
{
    private float life = 8f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 48;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                tex.SetPixel(x, y, new Color(0.6f, 0.9f, 1f, 0.5f));
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 1;
        transform.localScale = new Vector3(2.5f, 1.5f, 1f);
    }

    void Update()
    {
        life -= Time.deltaTime;
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.8f)
        {
            croc.transform.position += Vector3.right * 3f * Time.deltaTime;
        }

        if (life <= 0f) Destroy(gameObject);
    }
}