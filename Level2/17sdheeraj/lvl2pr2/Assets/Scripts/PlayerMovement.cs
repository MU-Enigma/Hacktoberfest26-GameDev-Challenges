using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private CapybaraSleep sleep;
    private CapybaraEat eat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sleep = GetComponent<CapybaraSleep>();
        eat = GetComponent<CapybaraEat>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (movement != Vector2.zero)
        {
            if (sleep != null && sleep.isSleeping)
            {
                sleep.WakeUp();
            }

            if (eat != null && eat.isEating)
            {
                eat.StopEating();
            }
        }
        else
        {
            if (sleep != null && sleep.isSleeping)
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
