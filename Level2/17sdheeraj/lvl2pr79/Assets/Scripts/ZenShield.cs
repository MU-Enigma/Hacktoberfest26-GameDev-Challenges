using UnityEngine;

public class ZenShield : MonoBehaviour
{
    [HideInInspector] public bool isMeditating = false;
    public float cooldownTimer = 0f;
    private float zenTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && cooldownTimer <= 0f && !isMeditating)
        {
            StartZen();
        }

        if (isMeditating)
        {
            zenTimer -= Time.deltaTime;
            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null && Vector3.Distance(transform.position, croc.transform.position) < 3.5f)
            {
                var rb = croc.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            if (zenTimer <= 0f)
            {
                isMeditating = false;
            }
        }
    }

    public void StartZen()
    {
        if (cooldownTimer > 0f) return;
        isMeditating = true;
        zenTimer = 3.5f;
        cooldownTimer = 7f;
    }
}