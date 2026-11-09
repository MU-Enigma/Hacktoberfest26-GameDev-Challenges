using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [HideInInspector] public bool isLuring = false;
    public float cooldownTimer = 0f;
    private GameObject dotObj;
    private float dotTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L) && cooldownTimer <= 0f && !isLuring)
        {
            ShineLaser();
        }

        if (isLuring)
        {
            dotTimer -= Time.deltaTime;
            Vector3 mousePos = Camera.main != null ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : transform.position + Vector3.right * 3f;
            mousePos.z = 0f;
            if (dotObj != null) dotObj.transform.position = mousePos;

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null)
            {
                croc.transform.position = Vector3.MoveTowards(croc.transform.position, mousePos, 3f * Time.deltaTime);
            }

            if (dotTimer <= 0f)
            {
                isLuring = false;
                if (dotObj != null) Destroy(dotObj);
            }
        }
    }

    public void ShineLaser()
    {
        if (cooldownTimer > 0f) return;
        dotObj = new GameObject("LaserDotVisual");
        var sr = dotObj.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, Color.red);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 7;
        dotObj.transform.localScale = Vector3.one * 0.4f;

        isLuring = true;
        dotTimer = 4f;
        cooldownTimer = 7f;
    }
}