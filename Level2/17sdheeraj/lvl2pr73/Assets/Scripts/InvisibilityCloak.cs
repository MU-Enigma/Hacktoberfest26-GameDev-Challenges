using UnityEngine;

public class InvisibilityCloak : MonoBehaviour
{
    [HideInInspector] public bool isCloaked = false;
    public float cooldownTimer = 0f;
    private float cloakTimer = 0f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha8) && cooldownTimer <= 0f && !isCloaked)
        {
            ToggleCloak();
        }

        if (isCloaked)
        {
            cloakTimer -= Time.deltaTime;
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0.2f;
                sr.color = c;
            }

            if (cloakTimer <= 0f)
            {
                isCloaked = false;
                if (sr != null) { Color c = sr.color; c.a = 1f; sr.color = c; }
            }
        }
    }

    public void ToggleCloak()
    {
        if (cooldownTimer > 0f) return;
        isCloaked = true;
        cloakTimer = 4.5f;
        cooldownTimer = 8f;
    }
}