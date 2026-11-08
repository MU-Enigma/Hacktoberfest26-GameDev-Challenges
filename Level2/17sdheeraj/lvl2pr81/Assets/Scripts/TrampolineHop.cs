using UnityEngine;

public class TrampolineHop : MonoBehaviour
{
    [HideInInspector] public bool isHopping = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.H) && cooldownTimer <= 0f)
        {
            DropTrampoline();
        }
    }

    public void DropTrampoline()
    {
        if (cooldownTimer > 0f) return;
        var tramp = new GameObject("TrampolineObj");
        tramp.transform.position = transform.position;
        var b = tramp.AddComponent<TrampolineBehavior>();
        b.Init();
        isHopping = true;
        cooldownTimer = 6f;
    }
}

public class TrampolineBehavior : MonoBehaviour
{
    private float life = 8f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, (y < 8) ? Color.blue : Color.gray);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 2;
        transform.localScale = new Vector3(2f, 1f, 1f);
    }

    void Update()
    {
        life -= Time.deltaTime;
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.4f)
        {
            croc.transform.position += Vector3.up * 4f;
        }

        if (life <= 0f) Destroy(gameObject);
    }
}