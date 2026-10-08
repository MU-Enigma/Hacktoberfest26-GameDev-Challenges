using UnityEngine; 
public class PlayerMovement : MonoBehaviour { 
public float speed; 
public Rigidbody2D body; 
void Update() { 
float xInput = Input.GetAxis("Horizontal"); 
float yInput = Input.GetAxis("Vertical"); 
Vector2 direction = new Vector2(xInput, yInput).normalized; 
body.linearVelocity = direction * speed; 
} 
}