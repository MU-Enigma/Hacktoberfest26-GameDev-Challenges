using UnityEngine;

public class rigid : MonoBehaviour
{
    public Rigidbody2D blue_chicken;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x, y);

        blue_chicken.linearVelocity = movement * 5f;
    }
}