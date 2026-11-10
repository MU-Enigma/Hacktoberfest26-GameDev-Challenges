using UnityEngine;

public class MudArmor : MonoBehaviour
{
    [HideInInspector] public bool hasMudArmor = false;
    public float cooldownTimer = 0f;
    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && cooldownTimer <= 0f && !hasMudArmor)
        {
            ApplyMud();
        }

        if (hasMudArmor)
        {
            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 1.2f)
            {
                // Absorb hit
                hasMudArmor = false;
                if (sr != null) sr.color = originalColor;
                Vector3 blast = (croc.transform.position - transform.position).normalized;
                croc.transform.position += blast * 3f;
                cooldownTimer = 7f;
            }
        }
    }

    public void ApplyMud()
    {
        if (cooldownTimer > 0f) return;
        hasMudArmor = true;
        if (sr != null) sr.color = new Color(0.45f, 0.3f, 0.18f);
    }
}