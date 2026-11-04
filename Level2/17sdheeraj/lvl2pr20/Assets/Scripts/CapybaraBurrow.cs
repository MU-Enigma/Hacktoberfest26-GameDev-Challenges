using UnityEngine;

public class CapybaraBurrow : MonoBehaviour
{
    public Sprite dirtMoundSprite;
    public float cooldown = 3f;
    public float burrowDuration = 0.5f;

    [HideInInspector]
    public bool isBurrowing = false;
    private float cooldownTimer = 0f;
    private float burrowTimer = 0f;

    private SpriteRenderer capySr;
    private Collider2D capyCol;
    private PlayerMovement pm;

    void Start()
    {
        capySr = GetComponent<SpriteRenderer>();
        capyCol = GetComponent<Collider2D>();
        pm = GetComponent<PlayerMovement>();

        if (dirtMoundSprite == null)
        {
            dirtMoundSprite = Resources.Load<Sprite>("hole_in_the_ground");
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
               CheckMode("VerityEdit", "isPlayingEdit") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing");
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && !isBurrowing && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            StartBurrow();
        }

        if (isBurrowing)
        {
            burrowTimer += Time.deltaTime;
            if (burrowTimer >= burrowDuration)
            {
                EndBurrow();
            }
        }
    }

    public void StartBurrow()
    {
        isBurrowing = true;
        burrowTimer = 0f;

        SpawnMound(transform.position);

        if (capySr != null) capySr.enabled = false;
        if (capyCol != null) capyCol.enabled = false;
        if (pm != null) pm.enabled = false;
    }

    public void EndBurrow()
    {
        isBurrowing = false;
        cooldownTimer = cooldown;

        // Teleport across meadow away from crocodile
        Vector3 exitPos;
        if (transform.position.x > 0f)
        {
            exitPos = new Vector3(-4.5f, Random.Range(-2.5f, 2.5f), 0f);
        }
        else
        {
            exitPos = new Vector3(4.5f, Random.Range(-2.5f, 0.5f), 0f);
        }

        transform.position = exitPos;
        SpawnMound(exitPos);

        if (capySr != null) capySr.enabled = true;
        if (capyCol != null) capyCol.enabled = true;
        if (pm != null) pm.enabled = true;
    }

    private void SpawnMound(Vector3 pos)
    {
        if (dirtMoundSprite == null) return;

        GameObject mound = new GameObject("BurrowHole");
        mound.transform.position = pos + new Vector3(0f, -0.2f, 0f);
        mound.transform.localScale = new Vector3(0.9f, 0.6f, 1f);
        var sr = mound.AddComponent<SpriteRenderer>();
        sr.sprite = dirtMoundSprite;
        sr.sortingOrder = 1;
        Destroy(mound, 2.5f);
    }

    void OnGUI()
    {
        GUIStyle burrowStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        if (isBurrowing)
        {
            burrowStyle.normal.textColor = new Color(0.9f, 0.6f, 0.2f);
            GUI.Box(new Rect(20, 245, 175, 34), "BURROWING...", burrowStyle);
        }
        else if (cooldownTimer > 0f)
        {
            burrowStyle.normal.textColor = Color.gray;
            GUI.Box(new Rect(20, 245, 175, 34), "[R] Burrow (" + cooldownTimer.ToString("F1") + "s)", burrowStyle);
        }
        else if (!IsOtherModeActive())
        {
            burrowStyle.normal.textColor = new Color(0.85f, 0.55f, 0.2f);
            GUI.Box(new Rect(20, 245, 175, 34), "[R] Burrow: READY", burrowStyle);
        }
    }
}
