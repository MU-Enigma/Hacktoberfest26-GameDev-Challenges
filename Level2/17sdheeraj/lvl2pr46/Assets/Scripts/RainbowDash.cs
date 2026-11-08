using UnityEngine;

public class RainbowDash : MonoBehaviour
{
    [HideInInspector] public bool isDashing = false;
    public float cooldownTimer = 0f;
    private TrailRenderer rainbowTrail;

    void Awake()
    {
        rainbowTrail = gameObject.AddComponent<TrailRenderer>();
        rainbowTrail.time = 0.5f;
        rainbowTrail.startWidth = 0.4f;
        rainbowTrail.endWidth = 0.1f;
        rainbowTrail.material = new Material(Shader.Find("Sprites/Default"));
        rainbowTrail.startColor = Color.magenta;
        rainbowTrail.endColor = Color.cyan;
        rainbowTrail.enabled = false;
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && cooldownTimer <= 0f && !isDashing)
        {
            DoDash();
        }
    }

    public void DoDash()
    {
        if (cooldownTimer > 0f) return;
        rainbowTrail.enabled = true;
        transform.position += Vector3.right * 3.5f;
        cooldownTimer = 4f;
        Invoke("EndDash", 0.6f);
    }

    void EndDash()
    {
        if (rainbowTrail != null) rainbowTrail.enabled = false;
        isDashing = false;
    }
}