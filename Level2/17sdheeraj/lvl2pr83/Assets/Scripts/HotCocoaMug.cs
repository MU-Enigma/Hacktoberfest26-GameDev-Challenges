using UnityEngine;

public class HotCocoaMug : MonoBehaviour
{
    [HideInInspector] public bool isDrinkingCocoa = false;
    public float cooldownTimer = 0f;
    private float cocoaTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.C) && cooldownTimer <= 0f && !isDrinkingCocoa)
        {
            DrinkCocoa();
        }

        if (isDrinkingCocoa)
        {
            cocoaTimer -= Time.deltaTime;
            if (cocoaTimer <= 0f)
            {
                isDrinkingCocoa = false;
            }
        }
    }

    public void DrinkCocoa()
    {
        if (cooldownTimer > 0f) return;
        isDrinkingCocoa = true;
        cocoaTimer = 3f;
        cooldownTimer = 7f;
    }
}