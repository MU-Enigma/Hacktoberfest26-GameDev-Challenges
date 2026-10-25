
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet hit: " + other.name);

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            Debug.Log("Enemy found! Applying " + damage + " damage.");
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
