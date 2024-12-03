using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player; // ������ �� ������
    [SerializeField] private float _detectionRange; // ������ ���������
    [SerializeField] private float _speed; // �������� ��������
    [SerializeField] private float _attackRange; // ��������� �����
    [SerializeField] private LayerMask _playerLayer; // ���� �����������
    private Vector2 _directionToPlayer;
    [SerializeField] private float stoppingDistance; // ����������, �� ������� ���� ���������������

    public bool _isPlayerInSight = false; // ��������, ������� �� �����
    private float _distanceToPlayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���������, ����� �� ���� ������
        CheckPlayerVisibility();

        if (_isPlayerInSight)
        {
            _distanceToPlayer = Vector2.Distance(transform.position, _player.position);

            if (_distanceToPlayer > stoppingDistance)
            {
                // �������� � ������
                _directionToPlayer = (_player.position - transform.position).normalized;
            }
            else
            {
                // ���������������, ���� ����� ������
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
        Debug.Log("CheckPlayerVisibility called"); // ��������, ��� ����� ����������
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer > _detectionRange)
        {
            Debug.Log($"Player is out of detection range. Distance: {_distanceToPlayer}");
            return;
        }
        // ����������� �� ����� � ������
        _directionToPlayer = (_player.position - transform.position).normalized;

        // ������������ Raycast
        Debug.DrawRay(transform.position, _directionToPlayer * _detectionRange, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _directionToPlayer, _detectionRange, _playerLayer);

        if (hit.collider != null)
        {
            // ���������, ����������� �� ������ ���� ������
            if ((_playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                _isPlayerInSight = true; // ����� �����
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

        // ������ �����������
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        // ��� � ������
        Vector2 directionToPlayer = (_player.position - transform.position).normalized;

        // ������ ����� ��� �� ������
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _player.position);

        // ���� ����� �������, ������������� ��� � �������
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