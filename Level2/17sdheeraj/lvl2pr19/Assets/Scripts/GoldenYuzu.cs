using UnityEngine;

public class GoldenYuzu : MonoBehaviour
{
    public int points = 10;
    public float duration = 6f;

    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * 6f) * 0.15f;
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Capybara"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(points);
            }

            var power = other.GetComponent<GoldenPower>();
            if (power != null)
            {
                power.ActivateGolden(duration);
            }

            Destroy(gameObject);
        }
    }
}
