using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    public GameObject completionText;

    private bool completed = false;

    private void Start()
    {
        if (completionText != null)
            completionText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed || !other.CompareTag("Player"))
            return;

        completed = true;

        if (completionText != null)
            completionText.SetActive(true);
    }
}

