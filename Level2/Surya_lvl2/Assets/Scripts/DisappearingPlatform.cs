
using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public float disappearDelay = 1f;
    public float respawnDelay = 3f;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isDisappearing = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDisappearing)
        {
            StartCoroutine(DisappearAndRespawn());
        }
    }

    IEnumerator DisappearAndRespawn()
    {
        isDisappearing = true;

        yield return new WaitForSeconds(disappearDelay);

        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;

        isDisappearing = false;
    }
}
