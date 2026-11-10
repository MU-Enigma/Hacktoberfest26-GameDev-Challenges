using UnityEngine;

public class GrapplingTongue : MonoBehaviour
{
    [HideInInspector] public bool isGrappling = false;
    public float cooldownTimer = 0f;
    private float maxDistance = 7f;
    private LineRenderer line;
    private Vector3 targetPos;
    private Transform pulledObject = null;
    private float grappleSpeed = 12f;

    void Awake()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.12f;
        line.endWidth = 0.08f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(1f, 0.4f, 0.6f);
        line.endColor = new Color(0.9f, 0.2f, 0.4f);
        line.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer <= 0f && !isGrappling)
        {
            FireGrapple();
        }

        if (isGrappling)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, targetPos);

            if (pulledObject != null)
            {
                pulledObject.position = Vector3.MoveTowards(pulledObject.position, transform.position, grappleSpeed * Time.deltaTime);
                targetPos = pulledObject.position;
                if (Vector3.Distance(pulledObject.position, transform.position) < 0.6f)
                {
                    StopGrapple();
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, grappleSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPos) < 0.6f)
                {
                    StopGrapple();
                }
            }
        }
    }

    public void FireGrapple()
    {
        if (cooldownTimer > 0f) return;
        Vector3 mousePos = Camera.main != null ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : transform.position + Vector3.right * 3f;
        mousePos.z = 0f;
        Vector3 dir = (mousePos - transform.position).normalized;
        if (dir == Vector3.zero) dir = Vector3.right;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxDistance);
        Collider2D bestSnack = null;
        float bestDist = float.MaxValue;
        foreach (var h in hits)
        {
            if (h.GetComponent<FoodItem>() != null || h.name.Contains("Food") || h.name.Contains("Snack"))
            {
                float d = Vector3.Distance(transform.position, h.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestSnack = h;
                }
            }
        }

        if (bestSnack != null)
        {
            pulledObject = bestSnack.transform;
            targetPos = bestSnack.transform.position;
        }
        else
        {
            pulledObject = null;
            targetPos = transform.position + dir * maxDistance;
        }

        isGrappling = true;
        line.enabled = true;
        cooldownTimer = 4.5f;
    }

    public void StopGrapple()
    {
        isGrappling = false;
        if (line != null) line.enabled = false;
        pulledObject = null;
    }
}