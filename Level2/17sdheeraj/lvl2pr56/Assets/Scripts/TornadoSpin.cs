using UnityEngine;

public class TornadoSpin : MonoBehaviour
{
    [HideInInspector] public bool isSpinning = false;
    public float cooldownTimer = 0f;
    private float spinTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W) && cooldownTimer <= 0f && !isSpinning)
        {
            StartSpin();
        }

        if (isSpinning)
        {
            spinTimer -= Time.deltaTime;
            transform.Rotate(0, 0, 1080f * Time.deltaTime);

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 5f);
            foreach (var col in cols)
            {
                if (col.GetComponent<FoodItem>() != null || col.name.Contains("Food"))
                {
                    col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, 8f * Time.deltaTime);
                }
            }

            if (spinTimer <= 0f)
            {
                isSpinning = false;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    public void StartSpin()
    {
        if (cooldownTimer > 0f) return;
        isSpinning = true;
        spinTimer = 3f;
        cooldownTimer = 6f;
    }
}