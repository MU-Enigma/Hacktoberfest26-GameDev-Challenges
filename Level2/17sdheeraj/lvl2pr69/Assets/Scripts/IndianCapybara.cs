using UnityEngine;

public class IndianCapybara : MonoBehaviour
{
    public Sprite indianSprite;
    public Texture2D indianTexture;
    public AudioClip indianAudio;

    [HideInInspector]
    public bool isIndianMode = false;

    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Sprite originalSprite;
    private float lastToggleTime = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalSprite = sr.sprite;
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
        }

        EnsureAudioSource();
        LoadAssets();
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            Transform child = transform.Find("IndianAudio");
            if (child == null)
            {
                GameObject go = new GameObject("IndianAudio");
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
        if (indianTexture == null)
        {
            indianTexture = Resources.Load<Texture2D>("indiancapybara");
        }

        if (indianSprite == null)
        {
            if (indianTexture != null)
            {
                // PPU 220 scales 390x512 image cleanly to match normal capybara size
                indianSprite = Sprite.Create(indianTexture, new Rect(0, 0, indianTexture.width, indianTexture.height), new Vector2(0.5f, 0.5f), 220f);
                indianSprite.name = "IndianCapybaraSprite";
            }
            else
            {
                indianSprite = Resources.Load<Sprite>("indiancapybara");
            }
        }

        if (indianAudio == null || indianAudio.name != "indian-song")
        {
            indianAudio = Resources.Load<AudioClip>("indian-song");
            if (indianAudio == null)
            {
                indianAudio = Resources.Load<AudioClip>("Audio/indian-song");
            }
            if (indianAudio == null)
            {
                indianAudio = Resources.Load<AudioClip>("capybara-indian");
            }
        }

        if (audioSource != null && indianAudio != null)
        {
            audioSource.clip = indianAudio;
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
               CheckMode("DiscoMode", "isDisco") ||
               CheckMode("VerityEdit", "isPlayingEdit") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TryToggle();
        }

        if (Input.GetMouseButtonDown(0) && Camera.main != null)
        {
            // Ignore click if over HUD buttons in bottom-left
            if (Input.mousePosition.x < 240 && Input.mousePosition.y < 420)
            {
                // handled by OnGUI
            }
            else
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                Vector2 mouse2D = new Vector2(mouseWorld.x, mouseWorld.y);

                Collider2D col = GetComponent<Collider2D>();
                if ((col != null && col.OverlapPoint(mouse2D)) || Vector2.Distance(mouse2D, transform.position) < 1.4f)
                {
                    TryToggle();
                }
            }
        }

        // Maintain sprite appearance while in Indian mode
        if (isIndianMode && sr != null && indianSprite != null && sr.sprite != indianSprite)
        {
            sr.sprite = indianSprite;
        }
    }

    void OnMouseDown()
    {
        TryToggle();
    }

    public void TryToggle()
    {
        if (Time.unscaledTime - lastToggleTime < 0.35f) return;
        lastToggleTime = Time.unscaledTime;

        if (isIndianMode)
        {
            ToggleMode();
        }
        else if (!IsOtherModeActive())
        {
            ToggleMode();
        }
    }

    public void ToggleMode()
    {
        isIndianMode = !isIndianMode;
        EnsureAudioSource();
        LoadAssets();

        if (isIndianMode)
        {
            if (sr != null)
            {
                if (originalSprite == null) originalSprite = sr.sprite;
                if (indianSprite != null) sr.sprite = indianSprite;
            }

            if (audioSource != null && indianAudio != null)
            {
                audioSource.clip = indianAudio;
                audioSource.time = 0f;
                audioSource.Play();
            }
        }
        else
        {
            if (sr != null && originalSprite != null)
            {
                sr.sprite = originalSprite;
            }

            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }
}
