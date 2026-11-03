using UnityEngine;

public class CapybaraThrow : MonoBehaviour
{
    public GameObject snackPrefab;
    public float throwCooldown = 1.0f;

    private float cooldownTimer = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver) return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G))
        {
            if (cooldownTimer <= 0f)
            {
                ThrowSnack();
            }
        }
    }

    public void ThrowSnack()
    {
        if (snackPrefab == null) return;

        cooldownTimer = throwCooldown;

        float dir = (sr != null && sr.flipX) ? -1f : 1f;
        Vector3 spawnPos = transform.position + new Vector3(dir * 0.8f, 0.1f, 0f);

        GameObject snackObj = Instantiate(snackPrefab, spawnPos, Quaternion.identity);
        var snack = snackObj.GetComponent<ThrownSnack>();
        if (snack != null)
        {
            snack.Launch(new Vector2(dir * 4.5f, 1.2f));
        }
    }
}
