using UnityEngine;

public class BungeeCord : MonoBehaviour
{
    [HideInInspector] public bool isTethered = false;
    public float cooldownTimer = 0f;
    private Vector3 anchorPos;
    private float tetherTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.B) && cooldownTimer <= 0f && !isTethered)
        {
            ToggleBungee();
        }

        if (isTethered)
        {
            tetherTimer -= Time.deltaTime;
            if (tetherTimer <= 0.4f)
            {
                transform.position = Vector3.MoveTowards(transform.position, anchorPos, 18f * Time.deltaTime);
            }

            if (tetherTimer <= 0f)
            {
                isTethered = false;
            }
        }
    }

    public void ToggleBungee()
    {
        if (cooldownTimer > 0f) return;
        anchorPos = transform.position;
        isTethered = true;
        tetherTimer = 2.5f;
        cooldownTimer = 7f;
    }
}