using UnityEngine;
using System.Collections;
using Pathfinding;
using Pathfinding.RVO;

public class EnemyHealth : MonoBehaviour
{

    [Header("Health")]
    public  int _currentHealth; // Current health of the enemy
    public int CurrentHealth => _currentHealth;
    [field: SerializeField] public int MaxHealth { get; private set; } // Maximum health of the enemy

    [Header("Knockback")]
    [SerializeField] private float _knockbackForce; // Force applied for knockback
    private Rigidbody2D rb;

    private EnemyAI _enemyAI; // AI Script
    private Animator _animator;
    private AIPath _aiPath;
    private RVOController _rvoController;
    private Seeker _seeker;

    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.2f; // how long to stay red
    [SerializeField] private float fadeBackTime = 0.2f;   // how long to fade back
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _damageFlashCoroutine;

    [Header("Optional Boss settings")]
    [SerializeField] private bool isBoss = false;
    private BossController bossController;
    void Start()
    {
        _currentHealth = MaxHealth; // Initialize health
        rb = GetComponent<Rigidbody2D>();
        _enemyAI = GetComponent<EnemyAI>();
        _animator = GetComponent<Animator>();
        _aiPath = GetComponent<AIPath>();
        _rvoController = GetComponent<RVOController>();
        _seeker = GetComponent<Seeker>();

        if (isBoss)
        {
            bossController = GetComponent<BossController>();
            Debug.Log("bossController: " + bossController);
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            _originalColor = _spriteRenderer.color;
    }

    // Method to apply damage to the enemy
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (_enemyAI != null)
        {
            _enemyAI.IsPlayerInSight = true;
            _animator.SetBool("IsDetected", _enemyAI.IsPlayerInSight);
        }
#if UNIY_EDITOR
        Debug.Log($"Enemy Health: {_currentHealth}");
#endif
        // --- Реакция босса ---
        if (isBoss && bossController != null)
        {
            if ((bossController.CurrentState == BossState.Phase1 || bossController.CurrentState == BossState.Phase2) &&
                bossController.IsDashing && !bossController.WasHitThisDash)
            {
                _currentHealth -= damage;
                bossController.RegisterHit(); // запускает отступление
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Босс не получил урон — не в уязвимой фазе.");
#endif
                return; // урон не засчитываем, всё остальное не выполняем
            }
        }
        else
        {
            _currentHealth -= damage;
        }

        // --- Наносим отталкивание ---
        if (rb != null)
            rb.AddForce(knockbackDirection * _knockbackForce, ForceMode2D.Impulse);

        // --- Проверка на смерть ---
        if (_currentHealth <= 0)
        {
            Die();
            return;
        }

        // --- Визуальный флэш ---
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
    public void SetBossReference(BossController boss)
    {
        bossController = boss;
    }

    // Method to handle enemy death
    private void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Enemy has died!");
#endif
        if (isBoss && bossController != null)
        {
            bossController.Die(); // Вызов функции смерти босса
            return; // Не продолжаем стандартную логику
        }
        if (bossController != null)
        {
            bossController.NotifyMinionKilled(gameObject);
        }
        // Отключаем логику поведения
        if (_enemyAI != null)
            _enemyAI.enabled = false;

        // Запускаем анимацию "труп"
        if (_animator != null)
            _animator.SetTrigger("DeathMess");

        // Отключаем физику (если надо)
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
        if (_aiPath != null) _aiPath.enabled = false;
        if (_rvoController != null) _rvoController.enabled = false;
        if (_seeker != null) _seeker.enabled = false;

        // Отключаем коллайдер (если нужно, чтобы не мешал)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }
    public void ResetHealthToMax()
    {
        _currentHealth = MaxHealth;
    }
}