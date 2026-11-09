using UnityEngine;

public class PinataSmash : MonoBehaviour
{
    [HideInInspector] public bool isPinataActive = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P) && cooldownTimer <= 0f)
        {
            HangPinata();
        }
    }

    public void HangPinata()
    {
        if (cooldownTimer > 0f) return;
        var p = new GameObject("PinataObj");
        p.transform.position = transform.position + Vector3.up * 1.5f;
        var b = p.AddComponent<PinataBehavior>();
        b.Init();
        isPinataActive = true;
        cooldownTimer = 7f;
    }
}

public class PinataBehavior : MonoBehaviour
{
    private float timer = 5f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(Random.value, Random.value, Random.value));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 4;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.Rotate(0, 0, 90f * Time.deltaTime);
        if (timer <= 0f) Destroy(gameObject);
    }
}