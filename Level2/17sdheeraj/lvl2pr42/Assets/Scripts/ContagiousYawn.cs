using UnityEngine;

public class ContagiousYawn : MonoBehaviour
{
    [HideInInspector] public bool isYawning = false;
    public float cooldownTimer = 0f;
    private float yawnTimer = 0f;
    private LineRenderer yawnAura;

    void Awake()
    {
        EnsureYawnAura();
    }

    void EnsureYawnAura()
    {
        if (yawnAura != null) return;
        var obj = new GameObject("ContagiousYawnVisual");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        yawnAura = obj.AddComponent<LineRenderer>();
        yawnAura.positionCount = 36;
        yawnAura.startWidth = 0.12f;
        yawnAura.endWidth = 0.12f;
        yawnAura.material = new Material(Shader.Find("Sprites/Default"));
        yawnAura.startColor = new Color(0.6f, 0.8f, 1f, 0.7f);
        yawnAura.endColor = new Color(0.8f, 0.9f, 1f, 0.1f);
        yawnAura.loop = true;
        yawnAura.useWorldSpace = true;
        yawnAura.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Y) && cooldownTimer <= 0f && !isYawning)
        {
            DoYawn();
        }

        if (isYawning)
        {
            yawnTimer -= Time.deltaTime;
            float r = (2.5f - yawnTimer) * 2.5f;
            if (yawnAura == null) EnsureYawnAura();
            for (int i = 0; i < 36; i++)
            {
                float rad = i * (Mathf.PI * 2f / 36f);
                yawnAura.SetPosition(i, transform.position + new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f));
            }

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < r)
            {
                var rb = croc.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            if (yawnTimer <= 0f)
            {
                isYawning = false;
                if (yawnAura != null) yawnAura.enabled = false;
            }
        }
    }

    public void DoYawn()
    {
        if (cooldownTimer > 0f) return;
        EnsureYawnAura();
        isYawning = true;
        yawnTimer = 2.5f;
        if (yawnAura != null) yawnAura.enabled = true;
        cooldownTimer = 8f;
    }
}