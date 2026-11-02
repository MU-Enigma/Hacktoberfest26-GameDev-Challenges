using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movement;
    private CapybaraSleep sleep;
    private CapybaraEat eat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sleep = GetComponent<CapybaraSleep>();
        eat = GetComponent<CapybaraEat>();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (movement.x > 0 && sr != null)
        {
            sr.flipX = false;
        }
        else if (movement.x < 0 && sr != null)
        {
            sr.flipX = true;
        }

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
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            return;
        }

        if (sleep != null && sleep.isSleeping)
        {
            return;
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
