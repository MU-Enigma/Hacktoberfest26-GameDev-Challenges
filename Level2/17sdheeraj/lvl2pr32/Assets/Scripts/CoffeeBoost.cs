using UnityEngine;

public class CoffeeBoost : MonoBehaviour
{
    [HideInInspector] public bool isBoosted = false;
    public float cooldownTimer = 0f;
    private float boostTimer = 0f;
    private PlayerMovement pm;
    private TrailRenderer trail;

    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.35f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(0.6f, 0.35f, 0.15f, 0.8f);
        trail.endColor = new Color(0.9f, 0.7f, 0.3f, 0f);
        trail.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.C) && cooldownTimer <= 0f && !isBoosted)
        {
            DrinkCoffee();
        }

        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            transform.position += (Vector3)(Random.insideUnitCircle * 0.02f);
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                if (pm != null) pm.moveSpeed = 5f;
                trail.enabled = false;
            }
        }
    }

    public void DrinkCoffee()
    {
        if (cooldownTimer > 0f) return;
        isBoosted = true;
        boostTimer = 3.5f;
        if (pm != null) pm.moveSpeed = 12f;
        trail.enabled = true;
        cooldownTimer = 6f;
    }
}