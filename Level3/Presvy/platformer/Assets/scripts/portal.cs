
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Next Level")]
    public string nextSceneName;

    private bool isTeleporting = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            isTeleporting = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
