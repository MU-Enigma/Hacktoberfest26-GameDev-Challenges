using UnityEngine;

public class FortuneCookie : MonoBehaviour
{
    [HideInInspector] public bool hasCookie = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && cooldownTimer <= 0f)
        {
            CrackCookie();
        }
    }

    public void CrackCookie()
    {
        if (cooldownTimer > 0f) return;
        var snack = new GameObject("FortuneSnack");
        snack.transform.position = transform.position + Vector3.up * 1f;
        var sr = snack.AddComponent<SpriteRenderer>();
        int res = 24;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, new Color(1f, 0.8f, 0.4f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 4;
        hasCookie = true;
        cooldownTimer = 6f;
    }
}