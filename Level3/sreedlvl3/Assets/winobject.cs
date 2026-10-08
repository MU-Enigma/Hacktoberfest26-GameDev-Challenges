using UnityEngine;

public class WinObject : MonoBehaviour
{
    public GameObject winText;

    private void Start()
    {
        // Make sure the win text starts hidden
        winText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            winText.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            winText.SetActive(true);
        }
    }
}
