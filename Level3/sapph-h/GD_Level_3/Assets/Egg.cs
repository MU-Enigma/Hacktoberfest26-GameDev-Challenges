using UnityEngine;
using UnityEngine.SceneManagement;

public class Egg : MonoBehaviour
{
    public static int score = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Catcher"))
        {
            score++;

            Debug.Log("Score: " + score);

            Destroy(gameObject);

            if (score >= 10)
            {
                
                score = 0;
                SceneManager.LoadScene("level_1_completed");
                
            }
        }
    }
}