using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6.5f;
    public float sprintSpeed = 10.5f;
    public float sprintDuration = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movement;
    private CapybaraSleep sleep;
    private CapybaraEat eat;
    private float sprintTimer = 0f;
    private bool isSprinting = false;

    void Awake()
    {
        EnsureComponent("CapybaraCar");
        EnsureComponent("FruitMagnet");
        EnsureComponent("CapybaraBurrow");
        EnsureComponent("IndianCapybara");
        EnsureComponent("DiscoMode");
        EnsureComponent("VerityEdit");
        EnsureComponent("CardboardBox");
        EnsureComponent("BananaPeelTrap");
        EnsureComponent("LaserEyes");
        EnsureComponent("MatrixTime");
        EnsureComponent("GroundPound");
        EnsureComponent("CapyFireworks");
    }

    private void EnsureComponent(string typeName)
    {
        var type = System.Type.GetType(typeName);
        if (type != null && GetComponent(type) == null)
        {
            gameObject.AddComponent(type);
        }
    }

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

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            sprintTimer = sprintDuration;
        }

        bool holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (sprintTimer > 0f)
        {
            sprintTimer -= Time.deltaTime;
            isSprinting = true;
        }
        else if (holdingShift)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
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

        float speed = isSprinting ? sprintSpeed : moveSpeed;
        float dt = (Time.timeScale > 0f && Time.timeScale < 0.9f) ? (Time.fixedDeltaTime / Time.timeScale) : Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement * speed * dt);
    }
}
