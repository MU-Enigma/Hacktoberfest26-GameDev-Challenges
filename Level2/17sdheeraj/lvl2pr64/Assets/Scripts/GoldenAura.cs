using UnityEngine;

public class GoldenAura : MonoBehaviour
{
    [HideInInspector] public bool isGolden = false;
    public float cooldownTimer = 0f;
    private float auraTimer = 0f;
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

        if (Input.GetKeyDown(KeyCode.Z) && cooldownTimer <= 0f && !isGolden)
        {
            ActivateGolden();
        }

        if (isGolden)
        {
            auraTimer -= Time.deltaTime;
            if (sr != null)
            {
                sr.color = Color.Lerp(Color.yellow, Color.white, Mathf.PingPong(Time.time * 6f, 1f));
            }

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 2.5f)
            {
                Vector3 blast = (croc.transform.position - transform.position).normalized;
                croc.transform.position += blast * 4f * Time.deltaTime;
            }

            if (auraTimer <= 0f)
            {
                isGolden = false;
                if (sr != null) sr.color = originalColor;
            }
        }
    }

    public void ActivateGolden()
    {
        if (cooldownTimer > 0f) return;
        isGolden = true;
        auraTimer = 4.5f;
        cooldownTimer = 9f;
    }
}