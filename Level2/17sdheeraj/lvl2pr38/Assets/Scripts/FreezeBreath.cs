using UnityEngine;

public class FreezeBreath : MonoBehaviour
{
    [HideInInspector] public bool isFreezing = false;
    public float cooldownTimer = 0f;
    private LineRenderer frostLine;
    private float frostTimer = 0f;

    void Awake()
    {
        frostLine = gameObject.AddComponent<LineRenderer>();
        frostLine.positionCount = 3;
        frostLine.startWidth = 0.2f;
        frostLine.endWidth = 1.2f;
        frostLine.material = new Material(Shader.Find("Sprites/Default"));
        frostLine.startColor = new Color(0.7f, 0.95f, 1f, 0.9f);
        frostLine.endColor = new Color(0.3f, 0.7f, 1f, 0.1f);
        frostLine.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Z) && cooldownTimer <= 0f && !isFreezing)
        {
            ExhaleFrost();
        }

        if (isFreezing)
        {
            frostTimer -= Time.deltaTime;
            frostLine.SetPosition(0, transform.position);
            frostLine.SetPosition(1, transform.position + Vector3.right * 2.5f + Vector3.up * 0.8f);
            frostLine.SetPosition(2, transform.position + Vector3.right * 4f);

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 4f)
            {
                var rb = croc.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            if (frostTimer <= 0f)
            {
                isFreezing = false;
                frostLine.enabled = false;
            }
        }
    }

    public void ExhaleFrost()
    {
        if (cooldownTimer > 0f) return;
        isFreezing = true;
        frostTimer = 2.5f;
        frostLine.enabled = true;
        cooldownTimer = 7f;
    }
}