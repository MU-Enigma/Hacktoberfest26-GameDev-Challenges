
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int points = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collected coin! +" + points + " points");
            Destroy(gameObject);
        }
    }
}
