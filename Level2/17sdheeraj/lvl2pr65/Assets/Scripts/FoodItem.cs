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
        transform.position = startPos + new Vector3(0f, Mathf.Sin(Time.time * 4f) * 0.08f, 0f);
    }

    public void PullTowards(Vector3 target, float speed)
    {
        startPos = Vector3.MoveTowards(startPos, target, speed * Time.deltaTime);
        transform.position = startPos;

        if (Vector3.Distance(transform.position, target) < 1.0f)
        {
            Collect(GameObject.Find("Capybara"));
        }
    }

    public void Collect(GameObject player)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(points);
        }

        if (player != null)
        {
            var spit = player.GetComponent<CapybaraSeedSpit>();
            if (spit != null)
            {
                spit.OnWatermelonEaten();
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Capybara"))
        {
            Collect(other.gameObject);
        }
    }
}
