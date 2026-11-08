using System.Collections;
using UnityEngine;

public class LaserEyes : MonoBehaviour
{
    public float laserDuration = 1.2f;
    public float cooldown = 3.5f;

    [HideInInspector]
    public bool isFiring = false;

    private float cooldownTimer = 0f;
    private LineRenderer lineLeft;
    private LineRenderer lineRight;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetupLineRenderers();
    }

    void Start()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        SetupLineRenderers();
    }

    private void SetupLineRenderers()
    {
        if (lineLeft == null)
        {
            GameObject lObj = new GameObject("LaserLeft");
            lObj.transform.SetParent(transform);
            lineLeft = lObj.AddComponent<LineRenderer>();
            ConfigureLine(lineLeft);
        }

        if (lineRight == null)
        {
            GameObject rObj = new GameObject("LaserRight");
            rObj.transform.SetParent(transform);
            lineRight = rObj.AddComponent<LineRenderer>();
            ConfigureLine(lineRight);
        }
    }

    private void ConfigureLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.startWidth = 0.08f;
        line.endWidth = 0.04f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.yellow;
        line.sortingOrder = 8;
        line.enabled = false;
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
               CheckMode("CardboardBox", "isInBox") ||
               CheckMode("CapybaraSleep", "isSleeping") ||
               CheckMode("CapybaraBath", "isBathing") ||
               CheckMode("CapybaraBurrow", "isBurrowing") ||
               CheckMode("VerityEdit", "isPlayingEdit");
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L) && !isFiring && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            StartCoroutine(FireLaserRoutine());
        }

        if (isFiring)
        {
            UpdateLaserAim();
        }
    }

    public void FireLasers()
    {
        if (!isFiring && cooldownTimer <= 0f && !IsOtherModeActive())
        {
            StartCoroutine(FireLaserRoutine());
        }
    }

    private IEnumerator FireLaserRoutine()
    {
        isFiring = true;
        cooldownTimer = cooldown;

        if (lineLeft != null) lineLeft.enabled = true;
        if (lineRight != null) lineRight.enabled = true;

        float timer = 0f;
        while (timer < laserDuration)
        {
            timer += Time.deltaTime;
            UpdateLaserAim();

            // Hit detection on Crocodile
            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                Vector2 beamDir = (mouseWorld - transform.position).normalized;
                Vector2 toCroc = (croc.transform.position - transform.position).normalized;

                // If aiming generally at crocodile
                if (Vector2.Dot(beamDir, toCroc) > 0.85f && Vector2.Distance(transform.position, croc.transform.position) < 8f)
                {
                    // Knockback crocodile away
                    croc.transform.position += (Vector3)(toCroc * (4f * Time.deltaTime));
                    var crocSr = croc.GetComponent<SpriteRenderer>();
                    if (crocSr != null)
                    {
                        crocSr.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 8f, 1f));
                    }
                }
            }

            yield return null;
        }

        // Reset crocodile color if needed
        GameObject c = GameObject.Find("Crocodile");
        if (c != null)
        {
            var csr = c.GetComponent<SpriteRenderer>();
            if (csr != null) csr.color = Color.white;
        }

        if (lineLeft != null) lineLeft.enabled = false;
        if (lineRight != null) lineRight.enabled = false;
        isFiring = false;
    }

    private void UpdateLaserAim()
    {
        if (Camera.main == null) return;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        mouseWorld.z = 0f;

        bool flip = sr != null && sr.flipX;
        float xOffset = flip ? -0.3f : 0.3f;
        Vector3 eye1 = transform.position + new Vector3(xOffset - 0.08f, 0.45f, 0f);
        Vector3 eye2 = transform.position + new Vector3(xOffset + 0.08f, 0.45f, 0f);

        if (lineLeft != null)
        {
            lineLeft.SetPosition(0, eye1);
            lineLeft.SetPosition(1, mouseWorld);
        }
        if (lineRight != null)
        {
            lineRight.SetPosition(0, eye2);
            lineRight.SetPosition(1, mouseWorld);
        }
    }
}

