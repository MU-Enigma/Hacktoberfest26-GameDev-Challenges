
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.8f;

    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No GameObject tagged Player was found!");
        }
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        Vector2 direction = (Vector2)player.position - rb.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector2 nextPosition = Vector2.MoveTowards(
                rb.position,
                player.position,
                moveSpeed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPosition);
        }
    }
}
