using UnityEngine;

public class CapybaraCar : MonoBehaviour
{
    public Sprite[] carFrames;
    public AudioClip carAudio;
    public float driveSpeed = 8.5f;
    public float frameRate = 12f;

    [HideInInspector]
    public bool isInCar = false;
    private GameObject carObj;
    private SpriteRenderer carSr;
    private AudioSource audioSource;
    private PlayerMovement pm;
    private SpriteRenderer capySr;
    private float normalSpeed = 5f;
    private float honkTimer = 0f;
    private float frameTimer = 0f;
    private int currentFrame = 0;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        capySr = GetComponent<SpriteRenderer>();
        if (pm != null)
        {
            normalSpeed = pm.moveSpeed;
        }

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Auto load audio if not assigned
        if (carAudio == null)
        {
            carAudio = Resources.Load<AudioClip>("ok-i-pull-up-capybara");
            if (carAudio == null)
            {
                carAudio = Resources.Load<AudioClip>("Audio/ok-i-pull-up-capybara");
            }
        }
        if (carAudio != null)
        {
            audioSource.clip = carAudio;
        }

        // Auto load frames if not assigned
        if (carFrames == null || carFrames.Length == 0)
        {
            carFrames = Resources.LoadAll<Sprite>("OkPullUp");
            if (carFrames == null || carFrames.Length == 0)
            {
                Texture2D[] texs = Resources.LoadAll<Texture2D>("OkPullUp");
                if (texs != null && texs.Length > 0)
                {
                    carFrames = new Sprite[texs.Length];
                    for (int i = 0; i < texs.Length; i++)
                    {
                        carFrames[i] = Sprite.Create(texs[i], new Rect(0, 0, texs[i].width, texs[i].height), new Vector2(0.5f, 0.5f), 100f);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            if (isInCar) ExitCar();
            return;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isInCar)
            {
                ExitCar();
            }
            else
            {
                EnterCar();
            }
        }

        if (isInCar)
        {
            if (pm != null)
            {
                pm.moveSpeed = driveSpeed;
            }

            // Animate car frames
            if (carFrames != null && carFrames.Length > 0 && carSr != null)
            {
                frameTimer += Time.deltaTime;
                if (frameTimer >= 1f / frameRate)
                {
                    frameTimer = 0f;
                    currentFrame = (currentFrame + 1) % carFrames.Length;
                    carSr.sprite = carFrames[currentFrame];
                }
            }

            if (carObj != null)
            {
                bool flip = capySr != null && capySr.flipX;
                carObj.transform.position = transform.position;
                if (carSr != null)
                {
                    carSr.flipX = flip;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Honk();
            }

            if (honkTimer > 0f)
            {
                honkTimer -= Time.deltaTime;
            }
        }
    }

    public void EnterCar()
    {
        isInCar = true;
        if (pm != null)
        {
            pm.moveSpeed = driveSpeed;
        }

        if (capySr != null)
        {
            capySr.enabled = false;
        }

        if (carObj == null)
        {
            carObj = new GameObject("OkPullUpCar");
            carObj.transform.SetParent(transform);
            carObj.transform.localPosition = Vector3.zero;
            carObj.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            carSr = carObj.AddComponent<SpriteRenderer>();
            if (carFrames != null && carFrames.Length > 0)
            {
                carSr.sprite = carFrames[0];
            }
            carSr.sortingOrder = 5;
        }
        else
        {
            carObj.SetActive(true);
        }

        if (audioSource != null && carAudio != null)
        {
            audioSource.Play();
        }
    }

    public void ExitCar()
    {
        isInCar = false;
        if (pm != null)
        {
            pm.moveSpeed = normalSpeed;
        }

        if (capySr != null)
        {
            capySr.enabled = true;
        }

        if (carObj != null)
        {
            carObj.SetActive(false);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void Honk()
    {
        honkTimer = 0.8f;

        var croc = GameObject.Find("Crocodile");
        if (croc != null)
        {
            float dist = Vector2.Distance(transform.position, croc.transform.position);
            if (dist < 4.5f)
            {
                var ai = croc.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.Die();
                }
            }
        }
    }

    void OnGUI()
    {
        if (isInCar)
        {
            GUIStyle carStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            carStyle.normal.textColor = Color.red;
            GUI.Box(new Rect(20, 205, 185, 34), "[V] Car: ON | [Space] Honk", carStyle);

            if (honkTimer > 0f)
            {
                float bx = Screen.width / 2f - 75f;
                float by = Screen.height / 2f - 90f;
                GUIStyle honkStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                honkStyle.normal.textColor = Color.yellow;
                GUI.Box(new Rect(bx, by, 150, 40), "HONK HONK!", honkStyle);
            }
        }
    }
}
