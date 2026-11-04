using UnityEngine;

public class VerityEdit : MonoBehaviour
{
    public AudioClip editAudio;
    public float cutInterval = 0.22f;

    [HideInInspector]
    public bool isPlayingEdit = false;

    private Texture2D[] editFrames;
    private AudioSource audioSource;
    private float timer = 0f;
    private int frameIndex = 0;
    private float zoom = 1f;
    private float previousTimeScale = 1f;

    void Awake()
    {
        isPlayingEdit = false;
        EnsureAudioSource();
        LoadAssets();
    }

    void Start()
    {
        isPlayingEdit = false;
        EnsureAudioSource();
        LoadAssets();
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            Transform child = transform.Find("EditAudio");
            if (child == null)
            {
                GameObject go = new GameObject("EditAudio");
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                audioSource = go.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = child.GetComponent<AudioSource>();
            }

            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
            audioSource.spatialBlend = 0f;
        }
    }

    private void LoadAssets()
    {
        if (editAudio == null)
        {
            editAudio = Resources.Load<AudioClip>("verity-edit");
            if (editAudio == null)
            {
                editAudio = Resources.Load<AudioClip>("Audio/verity-edit");
            }
        }
        if (editAudio != null && audioSource != null)
        {
            audioSource.clip = editAudio;
        }

        if (editFrames == null || editFrames.Length == 0)
        {
            editFrames = Resources.LoadAll<Texture2D>("Verity");
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
               CheckMode("DiscoMode", "isDisco") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (isPlayingEdit)
            {
                StopEdit();
            }
            else if (!IsOtherModeActive())
            {
                StartEdit();
            }
        }

        if (isPlayingEdit)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopEdit();
                return;
            }

            if (audioSource != null && !audioSource.isPlaying)
            {
                StopEdit();
                return;
            }

            // Unscaled time so animation runs while game is paused
            timer += Time.unscaledDeltaTime;
            zoom = 1f + Mathf.PingPong(Time.unscaledTime * 2.5f, 0.12f);

            if (timer >= cutInterval)
            {
                timer = 0f;
                if (editFrames != null && editFrames.Length > 0)
                {
                    frameIndex = (frameIndex + 1) % editFrames.Length;
                }
            }
        }
    }

    public void StartEdit()
    {
        isPlayingEdit = true;
        timer = 0f;
        frameIndex = 0;
        EnsureAudioSource();
        LoadAssets();

        // Pause the game during edit
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (audioSource != null && editAudio != null)
        {
            audioSource.clip = editAudio;
            audioSource.time = 0f;
            audioSource.Play();
        }
    }

    public void StopEdit()
    {
        isPlayingEdit = false;

        // Resume game
        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    void OnDisable()
    {
        if (isPlayingEdit)
        {
            Time.timeScale = 1f;
        }
    }

    void OnDestroy()
    {
        if (isPlayingEdit)
        {
            Time.timeScale = 1f;
        }
    }

    void OnGUI()
    {
        if (isPlayingEdit && editFrames != null && editFrames.Length > 0)
        {
            Texture2D tex = editFrames[frameIndex];
            if (tex != null)
            {
                // Fullscreen background letterbox
                GUI.color = new Color(0f, 0f, 0f, 0.92f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = Color.white;

                // Center picture with beat zoom
                float baseH = Screen.height * 0.75f;
                float baseW = baseH * ((float)tex.width / tex.height);
                float curW = baseW * zoom;
                float curH = baseH * zoom;
                float x = (Screen.width - curW) / 2f;
                float y = (Screen.height - curH) / 2f;

                GUI.DrawTexture(new Rect(x, y, curW, curH), tex, ScaleMode.ScaleToFit);

                GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 24,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                titleStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(0, 20, Screen.width, 35), "CAPYBARA EDIT", titleStyle);

                GUIStyle subStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold
                };
                if (GUI.Button(new Rect(Screen.width / 2f - 90f, Screen.height - 45f, 180, 32), "[U / Esc] Close Edit", subStyle))
                {
                    StopEdit();
                }
            }
        }
        else if (!isPlayingEdit && !IsOtherModeActive())
        {
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            btnStyle.normal.textColor = Color.white;
            if (GUI.Button(new Rect(20, 365, 205, 34), "[U] Capybara Edit", btnStyle))
            {
                StartEdit();
            }
        }
    }
}
