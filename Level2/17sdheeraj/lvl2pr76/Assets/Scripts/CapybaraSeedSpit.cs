using UnityEngine;

public class CapybaraSeedSpit : MonoBehaviour
{
    public GameObject seedPrefab;
    public int watermelonsEaten = 0;
    public int seedAmmo = 0;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnWatermelonEaten()
    {
        watermelonsEaten++;
        if (watermelonsEaten >= 3)
        {
            seedAmmo += 3;
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (seedAmmo > 0)
            {
                SpitSeed();
            }
        }
    }

    public void SpitSeed()
    {
        if (seedPrefab == null) return;
        seedAmmo--;

        float dir = (sr != null && sr.flipX) ? -1f : 1f;
        Vector3 spawnPos = transform.position + new Vector3(dir * 0.8f, 0.2f, 0f);

        GameObject seedObj = Instantiate(seedPrefab, spawnPos, Quaternion.identity);
        var seed = seedObj.GetComponent<WatermelonSeed>();
        if (seed != null)
        {
            seed.Launch(new Vector2(dir, 0f));
        }
    }

    void OnGUI()
    {
        if (watermelonsEaten >= 3)
        {
            GUIStyle ammoStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            ammoStyle.normal.textColor = Color.white;
            GUI.Box(new Rect(20, 75, 140, 36), "[C] Seeds: " + seedAmmo, ammoStyle);
        }
    }
}

