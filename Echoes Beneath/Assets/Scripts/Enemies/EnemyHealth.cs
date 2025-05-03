using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{

    [Header("Health")]
    private int _currentHealth; // Current health of the enemy
    [field: SerializeField] public int MaxHealth { get; private set; } // Maximum health of the enemy

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce; // Force applied for knockback
    private Rigidbody2D rb;

    private EnemyAI _enemyAI; // AI Script
    private Animator _animator;

    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.2f; // how long to stay red
    [SerializeField] private float fadeBackTime = 0.2f;   // how long to fade back
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _damageFlashCoroutine;
    void Start()
    {
        _currentHealth = MaxHealth; // Initialize health
        rb = GetComponent<Rigidbody2D>();
        _enemyAI = GetComponent<EnemyAI>();
        _animator = GetComponent<Animator>();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            _originalColor = _spriteRenderer.color;
    }

    // Method to apply damage to the enemy
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        _currentHealth -= damage;
        _enemyAI.IsPlayerInSight = true;
        _animator.SetBool("IsDetected", _enemyAI.IsPlayerInSight);
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
        StartDamageFlash();
    }
    private void StartDamageFlash()
    {
        if (_spriteRenderer == null) return;

        if (_damageFlashCoroutine != null)
            StopCoroutine(_damageFlashCoroutine);

        _damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        _spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;
        while (elapsed < fadeBackTime)
        {
            elapsed += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(Color.red, _originalColor, elapsed / fadeBackTime);
            yield return null;
        }

        _spriteRenderer.color = _originalColor;
        _damageFlashCoroutine = null;
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