using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player; // ������ �� ������
    [SerializeField] private float _detectionRange; // ������ ���������
    [SerializeField] private float _speed; // �������� ��������
    [SerializeField] private float _attackRange; // ��������� �����
    [SerializeField] private LayerMask _playerLayer; // ���� �����������
    [SerializeField] private float attackCooldown;
    private float _distanceToPlayer;

    public bool _isPlayerInSight = false; // ��������, ������� �� �����
    private bool isAttacking = false; // ���� ����� ��� �������������� ���������� ������ ��������

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
        // ���������, ����� �� ���� ������
        CheckPlayerVisibility();

        if (_isPlayerInSight)
        {
            // ������������� ������ ��� ����
            aiPath.destination = _player.position;

            // ���� ���� ��������� � �������� stoppingDistance, �������
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
        //Debug.Log("CheckPlayerVisibility called"); // ��������, ��� ����� ����������
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer > _detectionRange)
        {
            return;
        }
        // ����������� �� ����� � ������
        Vector2 direction = (_player.position - transform.position).normalized;

        // ������������ Raycast
        Debug.DrawRay(transform.position, direction * _detectionRange, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _detectionRange, _playerLayer);

        if (hit.collider != null)
        {
            // ���������, ����������� �� ������ ���� ������
            if ((_playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                _isPlayerInSight = true; // ����� �����
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

        // ����� ������ �����
        Debug.Log("Enemy attacks the player!");

        // ��������, ������� ���� ������
        // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

        // ��� ���������� ��������
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
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
}