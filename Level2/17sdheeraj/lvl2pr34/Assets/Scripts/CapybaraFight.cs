using UnityEngine;

public class CapybaraFight : MonoBehaviour
{
    public float attackRange = 2.4f;
    public GameObject hitPrefab;

    private SpriteRenderer sr;
    private CapybaraSleep sleep;
    private CapybaraEat eat;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sleep = GetComponent<CapybaraSleep>();
        eat = GetComponent<CapybaraEat>();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
        {
            Fight();
        }
    }

    public void Fight()
    {
        if (sleep != null && sleep.isSleeping)
        {
            sleep.WakeUp();
        }

        if (eat != null && eat.isEating)
        {
            eat.StopEating();
        }

        float dir = (sr != null && sr.flipX) ? -1f : 1f;
        Vector3 hitPos = transform.position + new Vector3(dir * 1.2f, 0f, 0f);

        if (hitPrefab != null)
        {
            GameObject fx = Instantiate(hitPrefab, hitPos, Quaternion.identity);
            Destroy(fx, 0.25f);
        }

        GameObject enemy = GameObject.Find("Crocodile");
        if (enemy != null)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            Vector2 toEnemy = enemy.transform.position - transform.position;

            bool inFront = (dir > 0 && toEnemy.x > -0.5f) || (dir < 0 && toEnemy.x < 0.5f);
            if (dist <= attackRange && inFront)
            {
                var ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.Die();
                }
            }
        }
    }
}

