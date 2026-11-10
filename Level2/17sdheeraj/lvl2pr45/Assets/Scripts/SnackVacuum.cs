using UnityEngine;

public class SnackVacuum : MonoBehaviour
{
    [HideInInspector] public bool isVacuumActive = false;
    public float cooldownTimer = 0f;
    private float vacuumTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.V) && cooldownTimer <= 0f && !isVacuumActive)
        {
            StartVacuum();
        }

        if (isVacuumActive)
        {
            vacuumTimer -= Time.deltaTime;
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, 8f);
            foreach (var col in items)
            {
                if (col.GetComponent<FoodItem>() != null || col.name.Contains("Food") || col.name.Contains("Snack"))
                {
                    col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, 9f * Time.deltaTime);
                }
            }

            if (vacuumTimer <= 0f)
            {
                isVacuumActive = false;
            }
        }
    }

    public void StartVacuum()
    {
        if (cooldownTimer > 0f) return;
        isVacuumActive = true;
        vacuumTimer = 3.5f;
        cooldownTimer = 7f;
    }
}