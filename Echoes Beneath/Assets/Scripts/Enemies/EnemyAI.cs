using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using System.Collections;


public enum DefaultDirection
{
    left,
    right
}
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement and Finding player")]
    [SerializeField] private Transform _player; // Player Transform component
    [SerializeField] private float _detectionRange; // Detection radius
    [SerializeField] private float _speed; // Enemy speed
    [SerializeField] private LayerMask _playerLayer; // Layer of Player object
    public bool IsPlayerInSight = false; // Check if player is detected
    private float _distanceToPlayer; // Distance from enemy to player
    private AIPath aiPath;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector2 _animationWeight;
    [SerializeField] private DefaultDirection _animDirection;
    int animVariant = 0;

    [Header("Attacking Player")]
    [SerializeField] private float _attackCooldown; // Attack cooldown
    [SerializeField] private float _attackRange; // Attack radius
    [SerializeField] private int _attackDamage; // Attack damage dealt to player
    public bool _isAttacking = false;
    private PlayerHealth _playerHealth;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stepClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip attackClip;

    private bool hasPlayedHurtSound = false;

    void Start()
    {
        // AI initialization
        aiPath = GetComponent<AIPath>();
        if (aiPath == null)
        {
#if UNITY_EDITOR
            Debug.LogError("AIPath component missing!");
#endif
        }
        aiPath.maxSpeed = _speed;
        aiPath.endReachedDistance = _attackRange;

        _animator = GetComponent<Animator>();
        // Player health initialization
        if(_player != null)
        {
            _playerHealth = _player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // Проверяем, видит ли враг игрока
        _animationWeight = CheckPlayerVisibility();

        if (IsPlayerInSight)
        {
            // Устанавливаем игрока как цель
            aiPath.destination = _player.position;
            ChangeAnimationWeight(_animationWeight, _animDirection);
            if (_distanceToPlayer <= _attackRange)
            {
                if (!_isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
    }

    private Vector2 CheckPlayerVisibility()
    {
        //Debug.Log("CheckPlayerVisibility called"); // Убедимся, что метод вызывается
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer > _detectionRange)
        {
            return Vector2.zero;
        }
        // Направление от врага к игроку
        Vector2 direction = (_player.position - transform.position).normalized;

#if UNITY_EDITOR
        Debug.Log($"angle: {direction}");
        // Визуализация Raycast
        Debug.DrawRay(transform.position, direction * _detectionRange, Color.red);
#endif

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _detectionRange, _playerLayer);

        if (hit.collider != null)
        {
            // Проверяем, принадлежит ли объект слою игрока
            if ((_playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                IsPlayerInSight = true; // Игрок видим
                _animator.SetBool("IsDetected", IsPlayerInSight);
#if UNITY_EDITOR
                Debug.Log("Player detected!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"Raycast hit object on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
#endif
            }
        }
        return direction;
    }
    IEnumerator AttackPlayer()
    {
        if (_playerHealth.IsDead)
            yield break;
        _animator.SetBool("IsAttacking", true);

        _isAttacking = true;

#if UNITY_EDITOR
        Debug.Log("Enemy attacks the player!");
#endif
        // Dealing damage to player
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(_attackDamage);
        }

        // Wait for attack cooldown
        yield return new WaitForSeconds(_attackCooldown);
        _isAttacking = false;
        _animator.SetBool("IsAttacking", false);
    }
    private void ChangeAnimationWeight(Vector2 direction, DefaultDirection _animDirection)
    {
        if ((direction.x < 0 && _animDirection == DefaultDirection.left) || (direction.x > 0 && _animDirection == DefaultDirection.right))
        {
            _spriteRenderer.flipX = false;
        }
        else if ((direction.x > 0 && _animDirection == DefaultDirection.left) || (direction.x < 0 && _animDirection == DefaultDirection.right))
        {
            _spriteRenderer.flipX = true;
        }
        _animator.SetFloat("MoveX", direction.x);
        _animator.SetFloat("MoveY", direction.y);

        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animVariant = 0; // боковая
        }
        else
        {
            animVariant = direction.y >= 0 ? 1 : 2; // вверх или вниз
        }
        _animator.SetInteger("AnimVariant", animVariant);
    }
    public void SetPlayerTarget(Transform player)
    {
        _player = player;
    }
    // Вызывается через Animation Event
    public void PlayStepSound(int eventVariant)
    {
        int currentVariant = _animator.GetInteger("AnimVariant");

        if (currentVariant == eventVariant)
        {
            if (stepClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(stepClip);
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"Step ignored. eventVariant={eventVariant} currentVariant={currentVariant}");
        }
#endif
    }

    // Вызывается через Animation Event
    public void PlayAttackSound()
    {
        if (attackClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackClip);
        }
    }

    // Вызывается в EnemyHealth при получении урона
    public void PlayHurtSound()
    {
        if (hasPlayedHurtSound || hurtClip == null || audioSource == null)
            return;

        hasPlayedHurtSound = true;
        audioSource.PlayOneShot(hurtClip);
        StartCoroutine(ResetHurtSoundFlag());
    }

    private IEnumerator ResetHurtSoundFlag()
    {
        yield return new WaitForSeconds(0.1f); // таймаут между ударами
        hasPlayedHurtSound = false;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_player == null)
            return;

        // Detection range radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // direction to player
        Vector2 directionToPlayer = (_player.position - transform.position).normalized;

        // yellow ray to player to track him
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _player.position);

        // If player is spotted, red ray
        if (IsPlayerInSight)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _player.position);
        }
    }
#endif
}