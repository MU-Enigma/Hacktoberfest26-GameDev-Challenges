using UnityEngine;

public class SpringShoes : MonoBehaviour
{
    [HideInInspector] public bool hasSprings = false;
    public float cooldownTimer = 0f;
    private float springTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J) && cooldownTimer <= 0f && !hasSprings)
        {
            ToggleSprings();
        }

        if (hasSprings)
        {
            springTimer -= Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(Time.time * 10f) * 0.05f;

            if (springTimer <= 0f)
            {
                hasSprings = false;
            }
        }
    }

    public void ToggleSprings()
    {
        if (cooldownTimer > 0f) return;
        hasSprings = true;
        springTimer = 5f;
        cooldownTimer = 7f;
    }
}