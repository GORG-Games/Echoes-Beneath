using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player; // Ссылка на игрока
    [SerializeField] private float _detectionRange; // Радиус видимости
    [SerializeField] private float _speed; // Скорость движения
    [SerializeField] private float _attackRange; // Дальность атаки
    [SerializeField] private LayerMask _playerLayer; // Слой препятствий
    private Vector2 _directionToPlayer;
    [SerializeField] private float stoppingDistance; // Расстояние, на котором враг останавливается

    public bool _isPlayerInSight = false; // Проверка, замечен ли игрок
    private float _distanceToPlayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверяем, видит ли враг игрока
        CheckPlayerVisibility();

        if (_isPlayerInSight)
        {
            _distanceToPlayer = Vector2.Distance(transform.position, _player.position);

            if (_distanceToPlayer > stoppingDistance)
            {
                // Движение к игроку
                _directionToPlayer = (_player.position - transform.position).normalized;
            }
            else
            {
                // Останавливаемся, если игрок близко
                _directionToPlayer = Vector2.zero;
            }
        }
    }
    void FixedUpdate()
    {
        if (_directionToPlayer != Vector2.zero)
        {
            rb.MovePosition(rb.position + _directionToPlayer * _speed * Time.fixedDeltaTime);
        }
    }

    private void CheckPlayerVisibility()
    {
        Debug.Log("CheckPlayerVisibility called"); // Убедимся, что метод вызывается
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer > _detectionRange)
        {
            Debug.Log($"Player is out of detection range. Distance: {_distanceToPlayer}");
            return;
        }
        // Направление от врага к игроку
        _directionToPlayer = (_player.position - transform.position).normalized;

        // Визуализация Raycast
        Debug.DrawRay(transform.position, _directionToPlayer * _detectionRange, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _directionToPlayer, _detectionRange, _playerLayer);

        if (hit.collider != null)
        {
            // Проверяем, принадлежит ли объект слою игрока
            if ((_playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                _isPlayerInSight = true; // Игрок видим
                Debug.Log("Player detected in 2D!");
            }
            else
            {
                Debug.Log($"Raycast hit object on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            }
        }
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

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
    }
}