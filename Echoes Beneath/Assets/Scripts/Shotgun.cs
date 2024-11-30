using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
    // ВСЕ ДЛЯ СТРЕЛЬБЫ:
    [SerializeField] private GameObject _bulletPrefab; // Префаб пули
    [SerializeField] private Transform _firePoint; // Точка, из которой будет происходить выстрел
    [SerializeField] private float _bulletSpeed; // Скорость пули
    
    [SerializeField] private int _bulletCount;
    [SerializeField] private float _spread;
    [SerializeField] private float _fireRate; // Время между выстрелами (кулдаун)
    private float _nextFireTime; // Время, когда можно стрелять в следующий раз

    [SerializeField] private float _shakeStrength;
    [SerializeField] private float _shakeTime;
    [SerializeField] private float _shakeFadeTime;

    // ВСЕ ДЛЯ ПЕРЕЗАРЯДКИ
    [SerializeField] private int _maxAmmo; // Максимальное количество патронов
    [SerializeField] private int _startTotalAmmo;
    private int _currentAmmoInChamber; // Текущее количество патронов
    public int _totalAmmo;
    [SerializeField] private float _reloadDelay; // Время на перезарядку
    private bool _isReloading = false; // Флаг, указывающий, что идет перезарядка
    [SerializeField] private Text _ammoDisplay; //Ссылка на текстовое поле для отображения текущего кол-ва патронов
    [SerializeField] private Text _totalAmmoDisplay;
    private Coroutine _reloadCoroutine; // Ссылка на текущую корутину перезарядки


    private void Start()
    {
        _currentAmmoInChamber = _maxAmmo; // Инициализируем начальное количество патронов в дробовике
        _totalAmmo = _startTotalAmmo; // Инициализируем начальное суммарное количество патронов
        UpdateAmmoUI();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= _nextFireTime) // Проверка нажатия ЛКМ
        {
            if (_isReloading)
            {
                StopCoroutine(_reloadCoroutine); // Останавливаем корутину перезарядки
                _isReloading = false; // Снимаем флаг перезарядки
                Debug.Log("Перезарядка прервана!");
            }
            if (_currentAmmoInChamber >= 1)
            {
                Shoot(_bulletCount, _spread);
                CameraController.cameraShake(_shakeStrength, _shakeTime, _shakeFadeTime);
                _nextFireTime = Time.time + 1f / _fireRate; // Устанавливаем новое время для следующего выстрела
                _currentAmmoInChamber--;
                UpdateAmmoUI();
            }

        }
        if ((Input.GetKeyDown(KeyCode.R) && _currentAmmoInChamber < _maxAmmo && !_isReloading) || (_currentAmmoInChamber == 0 && !_isReloading && _totalAmmo != 0))
        {
            _reloadCoroutine = StartCoroutine(Reload()); // Запускаем корутину для перезарядки
        }
    }

    void Shoot(int bulletCount, float spread)
    {


        float angleStep = spread / (bulletCount); // Шаг угла между пулями
        float startAngle = -spread / 2; // Начальный угол
        if (!_isReloading)
        {

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
    }
    IEnumerator Reload()
    {
        _isReloading = true; // Устанавливаем флаг, что идет перезарядка
        Debug.Log("Перезарядка...");
        // Задержка на начало перезарядки (если нужно время для анимации, звуков и т.д.)
        yield return new WaitForSeconds(0.5f); 

        // Заряжаем патроны по одному с задержкой
        while (_currentAmmoInChamber < _maxAmmo && _totalAmmo >= 1)
        {
            _currentAmmoInChamber++; // Увеличиваем количество патронов на 1
            _totalAmmo--; // Уменьшаем общее количество патронов на 1
            UpdateAmmoUI(); // Обновляем отображение патронов

            // Задержка между каждым патроном
            yield return new WaitForSeconds(_reloadDelay); // Задержка на 0.2 секунды между патронами
        }
        _isReloading = false; // Снимаем флаг перезарядки
        UpdateAmmoUI();
        Debug.Log("Перезарядка завершена!");
    }
    void UpdateAmmoUI()
    {
        _ammoDisplay.text = "Ammo: " + _currentAmmoInChamber + "/" + _maxAmmo; // Обновляем текст UI
        _totalAmmoDisplay.text = $"{_totalAmmo}";
        if (_totalAmmo < 1)
            _totalAmmoDisplay.color = Color.red;
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
