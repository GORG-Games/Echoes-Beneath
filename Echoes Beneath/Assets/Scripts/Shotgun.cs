using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab; // ������ ����
    [SerializeField] private Transform _firePoint; // �����, �� ������� ����� ����������� �������
    [SerializeField] private float _bulletSpeed; // �������� ����
    [SerializeField] private int _bulletCount;
    [SerializeField] private float _spread;

    private void Start()
    {

    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // �������� ������� ���
        {
            Shoot(_bulletCount, _spread);
        }
    }

    void Shoot(int bulletCount, float spread)
    {


        float angleStep = spread / (bulletCount - 1); // ��� ���� ����� ������
        float startAngle = -spread / 2; // ��������� ����

        for (int i = 0; i < bulletCount; i++)
        {
            // ������� ����
            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            // ��������� ���� ��� ������� ����
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // ���������� ���� � ������ ��������
            Vector2 direction = bulletRotation * _firePoint.up; // �������� �� firePoint.right, ���� �������� ������������ ������
            rb.velocity = direction * _bulletSpeed;
        }
    }

    void OnDrawGizmos(int bulletCount, float spread)
    {
        Gizmos.color = Color.red; // ���� ������
        float angleStep = spread / (bulletCount - 1);
        float startAngle = -spread / 2;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = bulletRotation * _firePoint.up;
            Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)direction * 5f); // ����� �����
        }

        // ������ ����� �� �����
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, -spread / 2) * _firePoint.up) * 100f);
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, spread / 2) * _firePoint.up) * 100f);
    }

}
