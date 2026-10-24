using UnityEngine;

public class CapybaraSleep : MonoBehaviour
{
    public Sprite awakeSprite;
    public Sprite sleepSprite;
    public GameObject zzzPrefab;
    public Transform sleepPoint;

    [HideInInspector]
    public bool isSleeping = false;

    private SpriteRenderer sr;
    private GameObject currentZzz;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (awakeSprite == null && sr != null)
        {
            awakeSprite = sr.sprite;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSleep();
        }
    }

    public void ToggleSleep()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        isSleeping = !isSleeping;

        if (isSleeping)
        {
            var eat = GetComponent<CapybaraEat>();
            if (eat != null && eat.isEating)
            {
                eat.StopEating();
            }

            if (sr != null && sleepSprite != null)
            {
                sr.sprite = sleepSprite;
            }

            if (zzzPrefab != null && currentZzz == null)
            {
                Vector3 pos = sleepPoint != null ? sleepPoint.position : transform.position + new Vector3(0.5f, 1.1f, 0);
                currentZzz = Instantiate(zzzPrefab, pos, Quaternion.identity, transform);
            }
        }
        else
        {
            WakeUp();
        }
    }

    public void WakeUp()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        isSleeping = false;

        if (sr != null && awakeSprite != null)
        {
            sr.sprite = awakeSprite;
        }

        if (currentZzz != null)
        {
            Destroy(currentZzz);
            currentZzz = null;
        }
    }
}
