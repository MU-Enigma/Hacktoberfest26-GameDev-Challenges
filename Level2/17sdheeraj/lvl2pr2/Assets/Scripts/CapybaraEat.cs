using UnityEngine;

public class CapybaraEat : MonoBehaviour
{
    public Sprite[] eatFrames;
    public float frameRate = 12f;

    [HideInInspector]
    public bool isEating = false;

    private SpriteRenderer sr;
    private Sprite defaultSprite;
    private float timer;
    private int currentFrame;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleEat();
        }

        if (isEating && eatFrames != null && eatFrames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % eatFrames.Length;
                if (sr != null)
                {
                    sr.sprite = eatFrames[currentFrame];
                }
            }
        }
    }

    public void ToggleEat()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        isEating = !isEating;

        if (isEating)
        {
            var sleep = GetComponent<CapybaraSleep>();
            if (sleep != null && sleep.isSleeping)
            {
                sleep.WakeUp();
            }

            defaultSprite = sr.sprite;
            currentFrame = 0;
            timer = 0f;

            if (eatFrames != null && eatFrames.Length > 0 && sr != null)
            {
                sr.sprite = eatFrames[0];
            }
        }
        else
        {
            StopEating();
        }
    }

    public void StopEating()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }

        isEating = false;

        if (sr != null && defaultSprite != null)
        {
            sr.sprite = defaultSprite;
        }
    }
}
