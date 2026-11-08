using UnityEngine;

public class ZeroGravity : MonoBehaviour
{
    [HideInInspector] public bool isZeroG = false;
    public float cooldownTimer = 0f;
    private float zeroGTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Z) && cooldownTimer <= 0f && !isZeroG)
        {
            ActivateZeroG();
        }

        if (isZeroG)
        {
            zeroGTimer -= Time.deltaTime;
            transform.position += Vector3.up * Mathf.Sin(Time.time * 4f) * 0.02f;

            GameObject croc = GameObject.Find("Crocodile");
            if (croc != null)
            {
                croc.transform.position += Vector3.up * Mathf.Sin(Time.time * 3f) * 0.02f;
            }

            if (zeroGTimer <= 0f)
            {
                isZeroG = false;
            }
        }
    }

    public void ActivateZeroG()
    {
        if (cooldownTimer > 0f) return;
        isZeroG = true;
        zeroGTimer = 4f;
        cooldownTimer = 8f;
    }
}