using UnityEngine;

public class MarshmallowRoast : MonoBehaviour
{
    [HideInInspector] public bool isRoasting = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha3) && cooldownTimer <= 0f)
        {
            StartRoast();
        }
    }

    public void StartRoast()
    {
        if (cooldownTimer > 0f) return;
        var fire = new GameObject("CampfireObj");
        fire.transform.position = transform.position;
        var b = fire.AddComponent<CampfireBehavior>();
        b.Init();
        isRoasting = true;
        cooldownTimer = 7f;
    }
}

public class CampfireBehavior : MonoBehaviour
{
    private float life = 6f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, (y < 12) ? new Color(0.5f, 0.3f, 0.1f) : new Color(1f, 0.5f, 0.1f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 2;
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }
}