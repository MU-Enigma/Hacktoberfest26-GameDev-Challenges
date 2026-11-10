
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Health restored! +" + healthAmount);
            Destroy(gameObject);
        }
    }
}
