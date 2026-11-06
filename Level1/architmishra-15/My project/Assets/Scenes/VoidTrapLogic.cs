using UnityEngine;

public class VoidTrapLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Instantly delete any physics object that falls into the trigger zone
        Destroy(other.gameObject);
    }
}