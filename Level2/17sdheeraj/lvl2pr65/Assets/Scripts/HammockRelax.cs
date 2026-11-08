using UnityEngine;

public class HammockRelax : MonoBehaviour
{
    [HideInInspector] public bool isRelaxing = false;
    public float cooldownTimer = 0f;
    private float relaxTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.H) && cooldownTimer <= 0f && !isRelaxing)
        {
            ToggleHammock();
        }

        if (isRelaxing)
        {
            relaxTimer -= Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(Time.time * 3f) * 0.005f;

            if (relaxTimer <= 0f)
            {
                isRelaxing = false;
            }
        }
    }

    public void ToggleHammock()
    {
        if (cooldownTimer > 0f) return;
        isRelaxing = true;
        relaxTimer = 4f;
        cooldownTimer = 8f;
    }
}