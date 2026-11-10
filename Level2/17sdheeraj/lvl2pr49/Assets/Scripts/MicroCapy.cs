using UnityEngine;

public class MicroCapy : MonoBehaviour
{
    [HideInInspector] public bool isMicro = false;
    public float cooldownTimer = 0f;
    private float microTimer = 0f;
    private Vector3 originalScale;
    private PlayerMovement pm;

    void Awake()
    {
        originalScale = transform.localScale;
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.S) && cooldownTimer <= 0f && !isMicro)
        {
            ToggleMicro();
        }

        if (isMicro)
        {
            microTimer -= Time.deltaTime;
            if (microTimer <= 0f)
            {
                isMicro = false;
                transform.localScale = originalScale;
                if (pm != null) pm.moveSpeed = 5f;
            }
        }
    }

    public void ToggleMicro()
    {
        if (cooldownTimer > 0f) return;
        isMicro = true;
        microTimer = 5f;
        transform.localScale = originalScale * 0.45f;
        if (pm != null) pm.moveSpeed = 8f;
        cooldownTimer = 8f;
    }
}