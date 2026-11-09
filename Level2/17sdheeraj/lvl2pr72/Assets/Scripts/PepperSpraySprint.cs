using UnityEngine;

public class PepperSpraySprint : MonoBehaviour
{
    [HideInInspector] public bool isSpicy = false;
    public float cooldownTimer = 0f;
    private float spicyTimer = 0f;
    private PlayerMovement pm;

    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha7) && cooldownTimer <= 0f && !isSpicy)
        {
            EatPepper();
        }

        if (isSpicy)
        {
            spicyTimer -= Time.deltaTime;
            if (spicyTimer <= 0f)
            {
                isSpicy = false;
                if (pm != null) pm.moveSpeed = 5f;
            }
        }
    }

    public void EatPepper()
    {
        if (cooldownTimer > 0f) return;
        isSpicy = true;
        spicyTimer = 3.5f;
        if (pm != null) pm.moveSpeed = 11f;
        cooldownTimer = 7f;
    }
}