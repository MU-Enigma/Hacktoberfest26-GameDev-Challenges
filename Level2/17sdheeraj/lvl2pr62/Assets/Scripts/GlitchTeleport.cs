using UnityEngine;

public class GlitchTeleport : MonoBehaviour
{
    [HideInInspector] public bool isGlitching = false;
    public float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && cooldownTimer <= 0f)
        {
            DoGlitch();
        }
    }

    public void DoGlitch()
    {
        if (cooldownTimer > 0f) return;
        transform.position += Vector3.right * 3.5f;
        cooldownTimer = 4f;
    }
}