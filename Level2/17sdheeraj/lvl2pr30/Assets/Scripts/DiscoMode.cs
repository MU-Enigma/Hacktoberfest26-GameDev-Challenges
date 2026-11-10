using UnityEngine;

public class DiscoMode : MonoBehaviour
{
    public AudioClip discoAudio;
    public float frameRate = 12f;

    [HideInInspector]
    public bool isDisco = false;

    private Sprite[] capyDanceFrames;
    private Sprite[] spinningCatFrames;
    private Sprite[] dancingCatFrames;

    private SpriteRenderer sr;
    private Sprite originalSprite;
    private Vector3 originalScale;
    private AudioSource audioSource;

    private GameObject spinningCatObj;
    private SpriteRenderer spinningCatSr;
    private GameObject dancingCatObj;
    private SpriteRenderer dancingCatSr;

    private GameObject crocodile;
    private Vector3 savedCrocPos;

    private float frameTimer = 0f;
    private int currentCapyFrame = 0;
    private int currentSpinFrame = 0;
    private int currentDanceCatFrame = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalSprite = sr.sprite;
            originalScale = transform.localScale;
        }

        EnsureAudioSource();
        LoadAssets();
    }

    void Start()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null && originalSprite == null)
        {
            originalSprite = sr.sprite;
            originalScale = transform.localScale;
        }

        EnsureAudioSource();
        LoadAssets();
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            Transform child = transform.Find("DiscoAudio");
            if (child == null)
            {
                GameObject go = new GameObject("DiscoAudio");
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                audioSource = go.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = child.GetComponent<AudioSource>();
            }

            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
            audioSource.spatialBlend = 0f;
            audioSource.pitch = 1f;
        }
    }

    private void LoadAssets()
    {
        if (discoAudio == null)
        {
            discoAudio = Resources.Load<AudioClip>("capybara-sound");
            if (discoAudio == null)
            {
                discoAudio = Resources.Load<AudioClip>("Audio/capybara-sound");
            }
        }
        if (discoAudio != null && audioSource != null)
        {
            audioSource.clip = discoAudio;
        }

        if (capyDanceFrames == null || capyDanceFrames.Length == 0)
        {
            capyDanceFrames = Resources.LoadAll<Sprite>("CapyDance");
        }
        if (spinningCatFrames == null || spinningCatFrames.Length == 0)
        {
            spinningCatFrames = Resources.LoadAll<Sprite>("SpinningCat");
        }
        if (dancingCatFrames == null || dancingCatFrames.Length == 0)
        {
            dancingCatFrames = Resources.LoadAll<Sprite>("DancingCat");
        }
    }

    private bool CheckMode(string typeName, string fieldName)
    {
        Component comp = GetComponent(typeName);
        if (comp != null && comp != this)
        {
            var field = comp.GetType().GetField(fieldName);
            if (field != null)
            {
                object val = field.GetValue(comp);
                if (val is bool b && b) return true;
            }
        }
        return false;
    }

    public bool IsOtherModeActive()
    {
        return CheckMode("CapybaraCar", "isInCar") ||
               CheckMode("IndianCapybara", "isIndianMode") ||
               CheckMode("VerityEdit", "isPlayingEdit") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (isDisco)
            {
                ToggleDisco();
            }
            else if (!IsOtherModeActive())
            {
                ToggleDisco();
            }
        }

        if (isDisco)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;

                // Animate Capybara
                if (capyDanceFrames != null && capyDanceFrames.Length > 0 && sr != null)
                {
                    currentCapyFrame = (currentCapyFrame + 1) % capyDanceFrames.Length;
                    sr.sprite = capyDanceFrames[currentCapyFrame];
                }

                // Animate Spinning Cat
                if (spinningCatFrames != null && spinningCatFrames.Length > 0 && spinningCatSr != null)
                {
                    currentSpinFrame = (currentSpinFrame + 1) % spinningCatFrames.Length;
                    spinningCatSr.sprite = spinningCatFrames[currentSpinFrame];
                }

                // Animate Dancing Cat
                if (dancingCatFrames != null && dancingCatFrames.Length > 0 && dancingCatSr != null)
                {
                    currentDanceCatFrame = (currentDanceCatFrame + 1) % dancingCatFrames.Length;
                    dancingCatSr.sprite = dancingCatFrames[currentDanceCatFrame];
                }
            }

            // Position companions beside Capybara
            if (spinningCatObj != null)
            {
                spinningCatObj.transform.position = transform.position + new Vector3(-1.8f, 0f, 0f);
            }
            if (dancingCatObj != null)
            {
                dancingCatObj.transform.position = transform.position + new Vector3(1.8f, 0f, 0f);
            }
        }
    }

    public void ToggleDisco()
    {
        isDisco = !isDisco;
        EnsureAudioSource();
        LoadAssets();

        if (isDisco)
        {
            // Croc goes away
            crocodile = GameObject.Find("Crocodile");
            if (crocodile != null)
            {
                savedCrocPos = crocodile.transform.position;
                crocodile.transform.position = new Vector3(500f, 500f, 0f);
                var ai = crocodile.GetComponent<EnemyAI>();
                if (ai != null) ai.enabled = false;
            }

            // Setup spinning cat on left
            if (spinningCatObj == null)
            {
                spinningCatObj = new GameObject("SpinningCat");
                spinningCatSr = spinningCatObj.AddComponent<SpriteRenderer>();
                spinningCatObj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                spinningCatSr.sortingOrder = 5;
            }
            spinningCatObj.SetActive(true);

            // Setup dancing cat on right
            if (dancingCatObj == null)
            {
                dancingCatObj = new GameObject("DancingCat");
                dancingCatSr = dancingCatObj.AddComponent<SpriteRenderer>();
                dancingCatObj.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                dancingCatSr.sortingOrder = 5;
            }
            dancingCatObj.SetActive(true);

            if (audioSource != null && discoAudio != null)
            {
                audioSource.clip = discoAudio;
                audioSource.time = 0f;
                audioSource.Play();
            }

            if (sr != null)
            {
                if (originalSprite == null) originalSprite = sr.sprite;
                transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
        }
        else
        {
            // Restore Crocodile
            if (crocodile != null)
            {
                crocodile.transform.position = savedCrocPos;
                var ai = crocodile.GetComponent<EnemyAI>();
                if (ai != null) ai.enabled = true;
            }

            if (spinningCatObj != null) spinningCatObj.SetActive(false);
            if (dancingCatObj != null) dancingCatObj.SetActive(false);

            if (sr != null && originalSprite != null)
            {
                sr.sprite = originalSprite;
                transform.localScale = originalScale;
            }

            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }
}
