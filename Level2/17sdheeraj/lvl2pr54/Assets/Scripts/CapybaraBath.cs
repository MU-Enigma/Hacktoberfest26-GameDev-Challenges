using UnityEngine;

public class CapybaraBath : MonoBehaviour
{
    public Sprite awakeSprite;
    public Sprite bathSprite;
    public Transform pond;
    public float bathDistance = 4.5f;

    [HideInInspector]
    public bool isBathing = false;

    private SpriteRenderer sr;
    private bool inPondTrigger = false;

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
        var sleep = GetComponent<CapybaraSleep>();
        if (sleep != null && sleep.isSleeping)
        {
            return;
        }

        var eat = GetComponent<CapybaraEat>();
        if (eat != null && eat.isEating)
        {
            return;
        }

        bool nearPond = inPondTrigger;
        if (!nearPond && pond != null)
        {
            nearPond = Vector2.Distance(transform.position, pond.position) <= bathDistance;
        }
        if (!nearPond)
        {
            nearPond = transform.position.x > 2.0f && transform.position.y > 0.0f;
        }

        if (nearPond)
        {
            if (!isBathing || (sr != null && sr.sprite != bathSprite))
            {
                StartBathing();
            }
        }
        else if (isBathing)
        {
            StopBathing();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("Pond"))
        {
            inPondTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.Contains("Pond"))
        {
            inPondTrigger = false;
        }
    }

    public void StartBathing()
    {
        isBathing = true;
        if (sr != null && bathSprite != null)
        {
            sr.sprite = bathSprite;
        }
    }

    public void StopBathing()
    {
        isBathing = false;
        if (sr != null && awakeSprite != null)
        {
            sr.sprite = awakeSprite;
        }
    }
}
