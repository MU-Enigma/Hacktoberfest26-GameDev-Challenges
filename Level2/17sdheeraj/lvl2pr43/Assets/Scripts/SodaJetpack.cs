using UnityEngine;

public class SodaJetpack : MonoBehaviour
{
    [HideInInspector] public bool isJetpacking = false;
    public float cooldownTimer = 0f;
    private float dashTimer = 0f;
    private Vector3 dashDir;
    private TrailRenderer trail;

    void Awake()
    {
        EnsureTrail();
    }

    void EnsureTrail()
    {
        if (trail != null) return;
        var obj = new GameObject("SodaJetpackVisual");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        trail = obj.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.35f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 0.95f, 0.8f, 0.9f);
        trail.endColor = new Color(1f, 0.6f, 0.1f, 0f);
        trail.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J) && cooldownTimer <= 0f && !isJetpacking)
        {
            LaunchSoda();
        }

        if (isJetpacking)
        {
            dashTimer -= Time.deltaTime;
            transform.position += dashDir * 14f * Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isJetpacking = false;
                if (trail != null) trail.enabled = false;
            }
        }
    }

    public void LaunchSoda()
    {
        if (cooldownTimer > 0f) return;
        EnsureTrail();
        Vector3 m = Camera.main != null ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : transform.position + Vector3.right;
        m.z = 0f;
        dashDir = (m - transform.position).normalized;
        if (dashDir == Vector3.zero) dashDir = Vector3.right;

        isJetpacking = true;
        dashTimer = 0.35f;
        if (trail != null) trail.enabled = true;
        cooldownTimer = 5f;
    }
}