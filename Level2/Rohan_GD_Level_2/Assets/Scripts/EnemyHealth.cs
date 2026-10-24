
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log("Enemy health: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy took damage! Remaining health: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Debug.Log("Enemy defeated!");
            Destroy(gameObject);
        }
    }
}
