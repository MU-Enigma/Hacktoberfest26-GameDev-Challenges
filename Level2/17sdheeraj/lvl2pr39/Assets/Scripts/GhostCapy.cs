using UnityEngine;

public class GhostCapy : MonoBehaviour
{
    [HideInInspector] public bool isGhost = false;
    public float cooldownTimer = 0f;
    private float ghostTimer = 0f;
    private SpriteRenderer sr;
    private PlayerMovement pm;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.N) && cooldownTimer <= 0f && !isGhost)
        {
            ToggleGhost();
        }

        if (isGhost)
        {
            ghostTimer -= Time.deltaTime;
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0.35f + Mathf.Sin(Time.time * 8f) * 0.15f;
                sr.color = c;
            }

            if (ghostTimer <= 0f)
            {
                isGhost = false;
                if (sr != null) { Color c = sr.color; c.a = 1f; sr.color = c; }
                if (pm != null) pm.moveSpeed = 5f;
            }
        }
    }

    public void ToggleGhost()
    {
        if (cooldownTimer > 0f) return;
        isGhost = true;
        ghostTimer = 4f;
        if (pm != null) pm.moveSpeed = 7.5f;
        cooldownTimer = 8f;
    }
}