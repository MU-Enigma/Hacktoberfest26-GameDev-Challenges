using UnityEngine;

public class MagnetShoes : MonoBehaviour
{
    [HideInInspector] public bool hasMagnetShoes = false;
    public float cooldownTimer = 0f;
    private float shoeTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha4) && cooldownTimer <= 0f && !hasMagnetShoes)
        {
            ToggleMagnetShoes();
        }

        if (hasMagnetShoes)
        {
            shoeTimer -= Time.deltaTime;
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.mass = 10f;

            if (shoeTimer <= 0f)
            {
                hasMagnetShoes = false;
                if (rb != null) rb.mass = 1f;
            }
        }
    }

    public void ToggleMagnetShoes()
    {
        if (cooldownTimer > 0f) return;
        hasMagnetShoes = true;
        shoeTimer = 5f;
        cooldownTimer = 8f;
    }
}