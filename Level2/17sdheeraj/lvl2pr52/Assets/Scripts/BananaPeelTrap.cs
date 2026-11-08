using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPeelTrap : MonoBehaviour
{
    public Sprite peelSprite;
    public float cooldown = 1.5f;
    public int maxPeels = 4;

    private float cooldownTimer = 0f;
    private List<GameObject> activePeels = new List<GameObject>();

    void Awake()
    {
        LoadAssets();
    }

    void Start()
    {
        LoadAssets();
    }

    private void LoadAssets()
    {
        if (peelSprite == null)
        {
            peelSprite = Resources.Load<Sprite>("bananapeel");
            if (peelSprite == null)
            {
                peelSprite = Resources.Load<Sprite>("banana_peel");
            }
            if (peelSprite == null)
            {
                Texture2D tex = Resources.Load<Texture2D>("bananapeel");
                if (tex == null) tex = Resources.Load<Texture2D>("banana_peel");
                if (tex != null)
                {
                    peelSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 250f);
                }
            }
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
        return CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("VerityEdit", "isPlayingEdit");
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.P) && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            DropPeel();
        }

        // Clean up nulls
        activePeels.RemoveAll(item => item == null);
    }

    public void DropPeel()
    {
        cooldownTimer = cooldown;
        LoadAssets();

        if (activePeels.Count >= maxPeels && activePeels[0] != null)
        {
            Destroy(activePeels[0]);
            activePeels.RemoveAt(0);
        }

        GameObject peel = new GameObject("BananaPeel");
        peel.transform.position = transform.position - new Vector3(0f, 0.2f, 0f);
        peel.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        var sr = peel.AddComponent<SpriteRenderer>();
        sr.sprite = peelSprite;
        sr.sortingOrder = 2;

        var trapLogic = peel.AddComponent<BananaPeelLogic>();
        activePeels.Add(peel);
    }
}

public class BananaPeelLogic : MonoBehaviour
{
    private GameObject croc;
    private bool hasSlipped = false;

    void Start()
    {
        croc = GameObject.Find("Crocodile");
    }

    void Update()
    {
        if (hasSlipped) return;
        if (croc == null) croc = GameObject.Find("Crocodile");
        if (croc == null) return;

        float dist = Vector2.Distance(transform.position, croc.transform.position);
        if (dist < 1.3f)
        {
            hasSlipped = true;
            StartCoroutine(CrocSlipRoutine());
        }
    }

    private IEnumerator CrocSlipRoutine()
    {
        var ai = croc.GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        // Visual slip text
        float elapsed = 0f;
        float duration = 2.2f;
        Vector3 slideDir = (croc.transform.position - transform.position).normalized;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Spin and slide
            croc.transform.Rotate(0, 0, 720f * Time.deltaTime);
            croc.transform.position += slideDir * (3.5f * Time.deltaTime);
            yield return null;
        }

        croc.transform.rotation = Quaternion.identity;
        if (ai != null) ai.enabled = true;

        Destroy(gameObject);
    }

    void OnGUI()
    {
        if (hasSlipped && croc != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(croc.transform.position + new Vector3(0, 1.2f, 0));
            if (screenPos.z > 0)
            {
                GUIStyle slipStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                slipStyle.normal.textColor = Color.yellow;
                GUI.Box(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 20, 100, 30), "SLIPPED!", slipStyle);
            }
        }
    }
}

