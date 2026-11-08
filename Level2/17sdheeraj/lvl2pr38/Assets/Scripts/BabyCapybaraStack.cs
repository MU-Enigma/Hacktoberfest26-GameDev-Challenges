using UnityEngine;

public class BabyCapybaraStack : MonoBehaviour
{
    public Sprite babySprite;
    public int babyCount = 0;
    public int maxBabies = 3;
    public float passiveScoreInterval = 10f;

    private float timer = 0f;
    private GameObject[] babyObjects;
    private SpriteRenderer capySr;

    void Start()
    {
        capySr = GetComponent<SpriteRenderer>();
        babyObjects = new GameObject[maxBabies];
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (babyCount > 0)
        {
            timer += Time.deltaTime;
            if (timer >= passiveScoreInterval)
            {
                timer = 0f;
                if (GameManager.instance != null)
                {
                    GameManager.instance.AddScore(babyCount);
                }
            }

            for (int i = 0; i < babyCount; i++)
            {
                if (babyObjects[i] != null)
                {
                    float dir = (capySr != null && capySr.flipX) ? 0.3f : -0.3f;
                    float height = 0.5f + (i * 0.35f);
                    float waddle = Mathf.Sin(Time.time * 6f + i) * 0.03f;
                    babyObjects[i].transform.position = transform.position + new Vector3(dir * (1f - (i * 0.2f)), height + waddle, 0f);

                    var bsr = babyObjects[i].GetComponent<SpriteRenderer>();
                    if (bsr != null && capySr != null)
                    {
                        bsr.flipX = capySr.flipX;
                    }
                }
            }
        }
    }

    public bool AddBaby()
    {
        if (babyCount >= maxBabies) return false;

        if (babyObjects == null || babyObjects.Length == 0)
        {
            babyObjects = new GameObject[maxBabies];
        }

        GameObject b = new GameObject("BabyCapy_" + babyCount);
        b.transform.SetParent(transform);
        var bsr = b.AddComponent<SpriteRenderer>();
        bsr.sprite = babySprite;
        bsr.sortingOrder = 9 + babyCount;

        babyObjects[babyCount] = b;
        babyCount++;
        return true;
    }

    public void ClearBabies()
    {
        if (babyObjects != null)
        {
            for (int i = 0; i < babyObjects.Length; i++)
            {
                if (babyObjects[i] != null)
                {
                    Destroy(babyObjects[i]);
                    babyObjects[i] = null;
                }
            }
        }
        babyCount = 0;
        timer = 0f;
    }
}

