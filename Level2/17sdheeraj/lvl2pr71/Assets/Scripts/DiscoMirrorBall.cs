using UnityEngine;

public class DiscoMirrorBall : MonoBehaviour
{
    [HideInInspector] public bool isMirrorBallActive = false;
    public float cooldownTimer = 0f;
    private GameObject ballObj;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha6) && cooldownTimer <= 0f)
        {
            DropMirrorBall();
        }
    }

    public void DropMirrorBall()
    {
        if (cooldownTimer > 0f) return;
        ballObj = new GameObject("MirrorBallVisual");
        ballObj.transform.position = transform.position + Vector3.up * 2f;
        var b = ballObj.AddComponent<MirrorBallBehavior>();
        b.Init();
        isMirrorBallActive = true;
        cooldownTimer = 8f;
    }
}

public class MirrorBallBehavior : MonoBehaviour
{
    private float timer = 5f;

    public void Init()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        int res = 32;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, ((x + y) % 4 == 0) ? Color.white : new Color(0.7f, 0.7f, 0.8f));
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
        sr.sortingOrder = 6;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        transform.Rotate(0, 0, 180f * Time.deltaTime);
        if (timer <= 0f) Destroy(gameObject);
    }
}