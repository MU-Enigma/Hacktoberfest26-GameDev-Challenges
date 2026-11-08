
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D body;

    private Vector2 direction;

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        direction = new Vector2(xInput, yInput).normalized;
    }

    void FixedUpdate()
    {
        body.linearVelocity = direction * speed;
    }
}
