using UnityEngine;

public class StrobeFlash : MonoBehaviour
{
    [HideInInspector] public bool isFlashing = false;
    public float cooldownTimer = 0f;
    private float flashTimer = 0f;
    private GameObject flashObj;

    void Awake()
    {
        flashObj = new GameObject("StrobeFlashOverlay");
        flashObj.transform.SetParent(transform);
        flashObj.transform.localPosition = Vector3.zero;
        var sr = flashObj.AddComponent<SpriteRenderer>();
        int res = 16;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 10;
        flashObj.transform.localScale = Vector3.one * 50f;
        flashObj.SetActive(false);
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L) && cooldownTimer <= 0f && !isFlashing)
        {
            TriggerFlash();
        }

        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                isFlashing = false;
                flashObj.SetActive(false);
            }
        }
    }

    public void TriggerFlash()
    {
        if (cooldownTimer > 0f) return;
        isFlashing = true;
        flashTimer = 0.25f;
        flashObj.SetActive(true);

        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 8f)
        {
            croc.transform.Rotate(0, 0, 180f);
        }

        cooldownTimer = 7f;
    }
}