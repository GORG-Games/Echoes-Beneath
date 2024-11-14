using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab; // Префаб пули
    [SerializeField] private Transform _firePoint; // Точка, из которой будет происходить выстрел
    [SerializeField] private float _bulletSpeed; // Скорость пули
    
    [SerializeField] public int _bulletCount;
    [SerializeField] public float _spread;
    [SerializeField] public float fireRate; // Время между выстрелами (кулдаун)
    private float _nextFireTime; // Время, когда можно стрелять в следующий раз

    private void Start()
    {

    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= _nextFireTime) // Проверка нажатия ЛКМ
        {
            Shoot(_bulletCount, _spread);
            _nextFireTime = Time.time + 1f / fireRate; // Устанавливаем новое время для следующего выстрела
        }
    }

    void Shoot(int bulletCount, float spread)
    {


        float angleStep = spread / (bulletCount); // Шаг угла между пулями
        float startAngle = -spread / 2; // Начальный угол

        for (int i = 0; i < bulletCount; i++)
        {
            // Создаем пулю
            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            // Вычисляем угол для текущей пули
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Направляем пулю с учетом разброса
            Vector2 direction = bulletRotation * _firePoint.up; // Изменить на firePoint.right, если дробовик ориентирован вправо
            rb.velocity = direction * _bulletSpeed;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Цвет конуса
        float angleStep = _spread / (_bulletCount - 1);
        float startAngle = -_spread / 2;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = bulletRotation * _firePoint.up;
            Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)direction * 5f); // Длина линии
        }

        // Рисуем линии по краям
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, -_spread / 2) * _firePoint.up));
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, _spread / 2) * _firePoint.up));
    }

}
