using UnityEngine;

public class LeafParachute : MonoBehaviour
{
    [HideInInspector] public bool isFloating = false;
    public float cooldownTimer = 0f;
    private float floatTimer = 0f;
    private GameObject leafObj;

    void Awake()
    {
        leafObj = new GameObject("LeafParachuteVisual");
        leafObj.transform.SetParent(transform);
        leafObj.transform.localPosition = new Vector3(0, 0.8f, 0);
        var sr = leafObj.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, (y > 10 && x > 4 && x < 28) ? new Color(0.2f, 0.8f, 0.3f) : Color.clear);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 5;
        leafObj.SetActive(false);
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L) && cooldownTimer <= 0f && !isFloating)
        {
            OpenParachute();
        }

        if (isFloating)
        {
            floatTimer -= Time.deltaTime;
            transform.position += new Vector3(Mathf.Sin(Time.time * 4f) * 0.02f, 0.01f, 0);

            if (floatTimer <= 0f)
            {
                isFloating = false;
                leafObj.SetActive(false);
            }
        }
    }

    public void OpenParachute()
    {
        if (cooldownTimer > 0f) return;
        isFloating = true;
        floatTimer = 4f;
        leafObj.SetActive(true);
        cooldownTimer = 6f;
    }
}