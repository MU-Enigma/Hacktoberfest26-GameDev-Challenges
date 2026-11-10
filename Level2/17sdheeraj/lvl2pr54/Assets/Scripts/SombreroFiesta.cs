using UnityEngine;

public class SombreroFiesta : MonoBehaviour
{
    [HideInInspector] public bool isFiesta = false;
    public float cooldownTimer = 0f;
    private float fiestaTimer = 0f;
    private GameObject sombreroObj;

    void Awake()
    {
        sombreroObj = new GameObject("SombreroVisual");
        sombreroObj.transform.SetParent(transform);
        sombreroObj.transform.localPosition = new Vector3(0, 0.6f, 0);
        var sr = sombreroObj.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                if (y < 8 && x > 4 && x < 28) tex.SetPixel(x, y, new Color(0.9f, 0.75f, 0.2f));
                else if (y >= 8 && y < 20 && x > 10 && x < 22) tex.SetPixel(x, y, new Color(0.9f, 0.3f, 0.2f));
                else tex.SetPixel(x, y, Color.clear);
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 6;
        sombreroObj.SetActive(false);
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.M) && cooldownTimer <= 0f && !isFiesta)
        {
            StartFiesta();
        }

        if (isFiesta)
        {
            fiestaTimer -= Time.deltaTime;
            sombreroObj.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 12f) * 15f);

            if (fiestaTimer <= 0f)
            {
                isFiesta = false;
                sombreroObj.SetActive(false);
            }
        }
    }

    public void StartFiesta()
    {
        if (cooldownTimer > 0f) return;
        isFiesta = true;
        fiestaTimer = 6f;
        sombreroObj.SetActive(true);
        cooldownTimer = 8f;
    }
}