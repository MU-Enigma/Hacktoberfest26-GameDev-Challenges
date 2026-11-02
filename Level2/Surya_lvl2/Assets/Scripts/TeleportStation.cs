
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TeleportStation : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;
    public float cooldown = 2f;

    private bool playerNearby = false;
    private bool canTeleport = true;

    void Update()
    {
        if (playerNearby &&
            canTeleport &&
            Keyboard.current != null &&
            Keyboard.current.lKey.wasPressedThisFrame)
        {
            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null || destination == null)
        {
            Debug.LogWarning("Player or teleport destination is missing!");
            return;
        }

        player.transform.position = destination.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player teleported successfully!");

        StartCoroutine(TeleportCooldown());
    }

    IEnumerator TeleportCooldown()
    {
        canTeleport = false;

        yield return new WaitForSeconds(cooldown);

        canTeleport = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Press E to teleport!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
