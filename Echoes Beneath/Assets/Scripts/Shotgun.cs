using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
    // ��� ��� ��������:
    [SerializeField] private GameObject _bulletPrefab; // ������ ����
    [SerializeField] private Transform _firePoint; // �����, �� ������� ����� ����������� �������
    [SerializeField] private float _bulletSpeed; // �������� ����
    
    [SerializeField] private int _bulletCount;
    [SerializeField] private float _spread;
    [SerializeField] private float _fireRate; // ����� ����� ���������� (�������)
    private float _nextFireTime; // �����, ����� ����� �������� � ��������� ���

    [SerializeField] private float _shakeStrength;
    [SerializeField] private float _shakeTime;
    [SerializeField] private float _shakeFadeTime;

    // ��� ��� �����������
    [SerializeField] private int _maxAmmo; // ������������ ���������� ��������
    [SerializeField] private int _startTotalAmmo;
    private int _currentAmmoInChamber; // ������� ���������� ��������
    public int _totalAmmo;
    [SerializeField] private float _reloadDelay; // ����� �� �����������
    private bool _isReloading = false; // ����, �����������, ��� ���� �����������
    [SerializeField] private Text _ammoDisplay; //������ �� ��������� ���� ��� ����������� �������� ���-�� ��������
    [SerializeField] private Text _totalAmmoDisplay;
    private Coroutine _reloadCoroutine; // ������ �� ������� �������� �����������


    private void Start()
    {
        _currentAmmoInChamber = _maxAmmo; // �������������� ��������� ���������� �������� � ���������
        _totalAmmo = _startTotalAmmo; // �������������� ��������� ��������� ���������� ��������
        UpdateAmmoUI();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= _nextFireTime) // �������� ������� ���
        {
            if (_isReloading)
            {
                StopCoroutine(_reloadCoroutine); // ������������� �������� �����������
                _isReloading = false; // ������� ���� �����������
                Debug.Log("����������� ��������!");
            }
            if (_currentAmmoInChamber >= 1)
            {
                Shoot(_bulletCount, _spread);
                CameraController.cameraShake(_shakeStrength, _shakeTime, _shakeFadeTime);
                _nextFireTime = Time.time + 1f / _fireRate; // ������������� ����� ����� ��� ���������� ��������
                _currentAmmoInChamber--;
                UpdateAmmoUI();
            }

        }
        if ((Input.GetKeyDown(KeyCode.R) && _currentAmmoInChamber < _maxAmmo && !_isReloading) || (_currentAmmoInChamber == 0 && !_isReloading && _totalAmmo != 0))
        {
            _reloadCoroutine = StartCoroutine(Reload()); // ��������� �������� ��� �����������
        }
    }

    void Shoot(int bulletCount, float spread)
    {


        float angleStep = spread / (bulletCount); // ��� ���� ����� ������
        float startAngle = -spread / 2; // ��������� ����
        if (!_isReloading)
        {

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
    }
    IEnumerator Reload()
    {
        _isReloading = true; // ������������� ����, ��� ���� �����������
        Debug.Log("�����������...");
        // �������� �� ������ ����������� (���� ����� ����� ��� ��������, ������ � �.�.)
        yield return new WaitForSeconds(0.5f); 

        // �������� ������� �� ������ � ���������
        while (_currentAmmoInChamber < _maxAmmo && _totalAmmo >= 1)
        {
            _currentAmmoInChamber++; // ����������� ���������� �������� �� 1
            _totalAmmo--; // ��������� ����� ���������� �������� �� 1
            UpdateAmmoUI(); // ��������� ����������� ��������

            // �������� ����� ������ ��������
            yield return new WaitForSeconds(_reloadDelay); // �������� �� 0.2 ������� ����� ���������
        }
        _isReloading = false; // ������� ���� �����������
        UpdateAmmoUI();
        Debug.Log("����������� ���������!");
    }
    void UpdateAmmoUI()
    {
        _ammoDisplay.text = "Ammo: " + _currentAmmoInChamber + "/" + _maxAmmo; // ��������� ����� UI
        _totalAmmoDisplay.text = $"{_totalAmmo}";
        if (_totalAmmo < 1)
            _totalAmmoDisplay.color = Color.red;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ���� ������
        float angleStep = _spread / (_bulletCount - 1);
        float startAngle = -_spread / 2;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = bulletRotation * _firePoint.up;
            Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)direction * 5f); // ����� �����
        }

        // ������ ����� �� �����
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, -_spread / 2) * _firePoint.up));
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, _spread / 2) * _firePoint.up));
    }

}
