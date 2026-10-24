using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private CapybaraSleep sleep;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sleep = GetComponent<CapybaraSleep>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (sleep != null && sleep.isSleeping)
        {
            if (movement != Vector2.zero)
            {
                sleep.WakeUp();
            }
            else
            {
                movement = Vector2.zero;
            }
        }
    }

    void FixedUpdate()
    {
        if (sleep != null && sleep.isSleeping)
        {
            return;
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
