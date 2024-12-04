using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player; // Ссылка на игрока
    [SerializeField] private float _detectionRange; // Радиус видимости
    [SerializeField] private float _speed; // Скорость движения
    [SerializeField] private float _attackRange; // Дальность атаки
    [SerializeField] private LayerMask _playerLayer; // Слой препятствий
    [SerializeField] private float attackCooldown;
    private float _distanceToPlayer;

    public bool _isPlayerInSight = false; // Проверка, замечен ли игрок
    private bool isAttacking = false; // Флаг атаки для предотвращения повторного вызова корутины

    public AIPath aiPath;
    //private AIPath aiPath;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        if (aiPath == null)
        {
            Debug.LogError("AIPath component missing!");
        }
        aiPath.maxSpeed = _speed;
        aiPath.endReachedDistance = _attackRange;
    }

    void Update()
    {
        // Проверяем, видит ли враг игрока
        CheckPlayerVisibility();

        if (_isPlayerInSight)
        {
            // Устанавливаем игрока как цель
            aiPath.destination = _player.position;

            // Если враг находится в пределах stoppingDistance, атакуем
            if (!aiPath.hasPath || aiPath.reachedDestination)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
    }

    private void CheckPlayerVisibility()
    {
        //Debug.Log("CheckPlayerVisibility called"); // Убедимся, что метод вызывается
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer > _detectionRange)
        {
            return;
        }
        // Направление от врага к игроку
        Vector2 direction = (_player.position - transform.position).normalized;

        // Визуализация Raycast
        Debug.DrawRay(transform.position, direction * _detectionRange, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _detectionRange, _playerLayer);

        if (hit.collider != null)
        {
            // Проверяем, принадлежит ли объект слою игрока
            if ((_playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                _isPlayerInSight = true; // Игрок видим
                Debug.Log("Player detected!");
            }
            else
            {
                Debug.Log($"Raycast hit object on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            }
        }
    }
    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Здесь логика атаки
        Debug.Log("Enemy attacks the player!");

        // Например, наносим урон игроку
        // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

        // Ждём завершения кулдауна
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        if (_player == null)
            return;

        // Радиус обнаружения
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // Луч к игроку
        Vector2 directionToPlayer = (_player.position - transform.position).normalized;

        // Рисуем жёлтый луч до игрока
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _player.position);

        // Если игрок замечен, перекрашиваем луч в красный
        if (_isPlayerInSight)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _player.position);
        }
    }
}