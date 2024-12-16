using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [Header("Health")]
    private int _currentHealth; // Current health of the enemy
    [field: SerializeField] public int MaxHealth { get; private set; } // Maximum health of the enemy

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce; // Force applied for knockback
    private Rigidbody2D rb;

    private EnemyAI _enemyAI; // AI Script
    void Start()
    {
        _currentHealth = MaxHealth; // Initialize health
        rb = GetComponent<Rigidbody2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    // Method to apply damage to the enemy
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        _currentHealth -= damage;
        _enemyAI.IsPlayerInSight = true;
#if UNIY_EDITOR
        Debug.Log($"Enemy Health: {_currentHealth}");
#endif
        // Apply knockback force
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * _knockbackForce, ForceMode2D.Impulse);
        }
        // Check if health is depleted
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
#if UNIY_EDITOR
        Debug.Log("Enemy has died!");
#endif
        // Add death animation, sound effect, or particle effect here
        Destroy(gameObject); // Destroy the enemy GameObject
    }
}