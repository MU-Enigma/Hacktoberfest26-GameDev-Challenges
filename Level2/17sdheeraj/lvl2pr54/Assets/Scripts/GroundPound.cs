using System.Collections;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    public Sprite shockwaveSprite;
    public float cooldown = 4.5f;

    [HideInInspector]
    public bool isPounding = false;

    private float cooldownTimer = 0f;
    private SpriteRenderer sr;
    private Collider2D col;
    private PlayerMovement pm;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        pm = GetComponent<PlayerMovement>();
        LoadAssets();
    }

    void Start()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (col == null) col = GetComponent<Collider2D>();
        if (pm == null) pm = GetComponent<PlayerMovement>();
        LoadAssets();
    }

    private void LoadAssets()
    {
        if (shockwaveSprite == null)
        {
            shockwaveSprite = Resources.Load<Sprite>("shockwave_ring");
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
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing") ||
               CheckMode("CardboardBox", "isInBox");
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.X) && !isPounding && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            StartCoroutine(GroundPoundRoutine());
        }
    }

    public void DoGroundPound()
    {
        if (!isPounding && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            StartCoroutine(GroundPoundRoutine());
        }
    }

    private IEnumerator GroundPoundRoutine()
    {
        isPounding = true;
        cooldownTimer = cooldown;
        LoadAssets();

        if (pm != null) pm.enabled = false;
        Vector3 startPos = transform.position;
        Vector3 apexPos = startPos + new Vector3(0f, 2.5f, 0f);

        // 1. Leap up
        float elapsed = 0f;
        float jumpTime = 0.35f;
        while (elapsed < jumpTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, apexPos, elapsed / jumpTime);
            yield return null;
        }

        // 2. Pause at peak
        yield return new WaitForSeconds(0.12f);

        // 3. Slam down
        elapsed = 0f;
        float slamTime = 0.15f;
        while (elapsed < slamTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(apexPos, startPos, elapsed / slamTime);
            yield return null;
        }
        transform.position = startPos;

        // 4. Impact: Shockwave and knockback
        SpawnShockwave(startPos);
        ApplyShockwaveKnockback(startPos);

        if (pm != null) pm.enabled = true;
        isPounding = false;
    }

    private void SpawnShockwave(Vector3 pos)
    {
        GameObject shock = new GameObject("ShockwaveRing");
        shock.transform.position = pos - new Vector3(0f, 0.3f, 0f);
        shock.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        var shockSr = shock.AddComponent<SpriteRenderer>();
        shockSr.sprite = shockwaveSprite;
        shockSr.sortingOrder = 3;

        StartCoroutine(ExpandShockwave(shock, shockSr));
    }

    private IEnumerator ExpandShockwave(GameObject obj, SpriteRenderer shockSr)
    {
        float timer = 0f;
        float duration = 0.55f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            float scale = Mathf.Lerp(0.5f, 5.5f, progress);
            obj.transform.localScale = new Vector3(scale, scale * 0.65f, 1f);

            if (shockSr != null)
            {
                shockSr.color = new Color(1f, 0.9f, 0.5f, 1f - progress);
            }
            yield return null;
        }

        Destroy(obj);
    }

    private void ApplyShockwaveKnockback(Vector3 epicenter)
    {
        GameObject croc = GameObject.Find("Crocodile");
        if (croc != null)
        {
            float dist = Vector2.Distance(epicenter, croc.transform.position);
            if (dist < 5.0f)
            {
                Vector2 knockDir = ((Vector2)croc.transform.position - (Vector2)epicenter).normalized;
                if (knockDir == Vector2.zero) knockDir = Vector2.right;

                croc.transform.position += (Vector3)(knockDir * 3.5f);
                var ai = croc.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.enabled = false;
                    StartCoroutine(RestoreCroc(ai, 2.0f));
                }
            }
        }
    }

    private IEnumerator RestoreCroc(EnemyAI ai, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ai != null) ai.enabled = true;
    }
}

