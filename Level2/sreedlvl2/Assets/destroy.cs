using UnityEngine;

public class Destroy : MonoBehaviour
{
    private Vector3 spawnPosition;
    private Rigidbody2D playerRb;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Save the exact position where the Player starts the game
            spawnPosition = player.transform.position;
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RespawnPlayer(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RespawnPlayer(collision.gameObject);
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        player.transform.position = spawnPosition;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}