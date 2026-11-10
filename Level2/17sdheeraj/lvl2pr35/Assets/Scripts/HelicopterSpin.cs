using UnityEngine;

public class HelicopterSpin : MonoBehaviour
{
    [HideInInspector] public bool isFloating = false;
    public float cooldownTimer = 0f;
    private float floatTimer = 0f;
    private float spinAngle = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.H) && cooldownTimer <= 0f && !isFloating)
        {
            ToggleGlide();
        }

        if (isFloating)
        {
            floatTimer -= Time.deltaTime;
            spinAngle += 720f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(spinAngle * Mathf.Deg2Rad) * 15f);
            transform.position += Vector3.up * Mathf.Sin(Time.time * 8f) * 0.01f;

            if (floatTimer <= 0f)
            {
                isFloating = false;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public void ToggleGlide()
    {
        if (cooldownTimer > 0f) return;
        isFloating = true;
        floatTimer = 3.5f;
        cooldownTimer = 6f;
    }
}