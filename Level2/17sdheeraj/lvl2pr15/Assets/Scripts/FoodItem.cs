using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public int points = 2;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0f, Mathf.Sin(Time.time * 4f) * 0.1f, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Capybara"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(points);
            }

            var spit = other.GetComponent<CapybaraSeedSpit>();
            if (spit != null)
            {
                spit.OnWatermelonEaten();
            }

            Destroy(gameObject);
        }
    }
}
