using UnityEngine;

public class SoapSlipTrail : MonoBehaviour
{
    [HideInInspector] public bool isSliding = false;
    public float cooldownTimer = 0f;
    private float slideTimer = 0f;
    private PlayerMovement pm;

    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha0) && cooldownTimer <= 0f && !isSliding)
        {
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            transform.position += Vector3.right * 8f * Time.deltaTime;

            if (slideTimer <= 0f)
            {
                isSliding = false;
                if (pm != null) pm.moveSpeed = 5f;
            }
        }
    }

    public void StartSlide()
    {
        if (cooldownTimer > 0f) return;
        isSliding = true;
        slideTimer = 1.2f;
        if (pm != null) pm.moveSpeed = 8f;
        cooldownTimer = 5f;
    }
}