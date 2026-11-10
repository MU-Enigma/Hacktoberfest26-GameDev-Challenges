using UnityEngine;

public class MetalRoar : MonoBehaviour
{
    [HideInInspector] public bool isRoaring = false;
    public float cooldownTimer = 0f;
    private LineRenderer ring;
    private float ringTimer = 0f;
    private float currentRadius = 0f;

    void Awake()
    {
        EnsureRing();
    }

    void EnsureRing()
    {
        if (ring != null) return;
        var obj = new GameObject("MetalRoarVisual");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        ring = obj.AddComponent<LineRenderer>();
        ring.positionCount = 36;
        ring.startWidth = 0.15f;
        ring.endWidth = 0.15f;
        ring.material = new Material(Shader.Find("Sprites/Default"));
        ring.startColor = new Color(1f, 0.85f, 0.2f, 0.9f);
        ring.endColor = new Color(1f, 0.3f, 0.1f, 0.2f);
        ring.loop = true;
        ring.useWorldSpace = true;
        ring.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && cooldownTimer <= 0f && !isRoaring)
        {
            DoRoar();
        }

        if (isRoaring)
        {
            ringTimer -= Time.deltaTime;
            currentRadius += 10f * Time.deltaTime;
            DrawRing(transform.position, currentRadius);

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < currentRadius + 0.5f)
            {
                Vector3 knock = (croc.transform.position - transform.position).normalized;
                croc.transform.position += knock * 6f * Time.deltaTime;
            }

            if (ringTimer <= 0f)
            {
                isRoaring = false;
                if (ring != null) ring.enabled = false;
            }
        }
    }

    void DrawRing(Vector3 center, float radius)
    {
        if (ring == null) EnsureRing();
        for (int i = 0; i < 36; i++)
        {
            float rad = i * (Mathf.PI * 2f / 36f);
            Vector3 pos = center + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
            ring.SetPosition(i, pos);
        }
    }

    public void DoRoar()
    {
        if (cooldownTimer > 0f) return;
        EnsureRing();
        isRoaring = true;
        ringTimer = 0.6f;
        currentRadius = 0.2f;
        if (ring != null) ring.enabled = true;
        cooldownTimer = 5f;
    }
}
