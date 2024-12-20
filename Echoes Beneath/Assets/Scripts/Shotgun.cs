using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Shotgun : MonoBehaviour
{
    [Header("Shooting: Shooting")]
    [SerializeField] private GameObject _bulletPrefab; // Bullet prefab
    [SerializeField] private Transform _firePoint; // point where bullets will be shot from
    [SerializeField] private float _bulletSpeed; // Bullet speed
    [SerializeField] private int _bulletCount; // How many bullets will be shot after shooting once
    [SerializeField] private float _spread; // Bullet spread
    [SerializeField] private float _fireRate; // Higher the value, lower the cooldown
    [field: SerializeField] public int Damage { get; private set; }
    private float _nextFireTime;


    [Header("Shooting: Camera Shake")]
    [SerializeField] private float _shakeStrength;
    [SerializeField] private float _shakeTime;
    [SerializeField] private float _shakeFadeTime;

    [Header("Shooting: Audio Settings")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;      // ������ �� AudioSource ��� ��������������� ������
    [SerializeField] private AudioClip _shootingSound;
    [SerializeField] private AudioClip _reloadSlightSound;
    [SerializeField] private AudioMixerGroup _environmentGroup;

    [Header("Reload: reloading")]
    [SerializeField] private int _maxAmmo; // Max ammo that can be IN shotgun
    [SerializeField] private int _startTotalAmmo; // Starting amount of ammo in shotgun
    private int _currentAmmoInChamber; // Current amount of ammo in shotgun
    public int _totalAmmo; // Total amount of ammo
    [SerializeField] private float _reloadDelay; // Delay between each bullet loaded
    private bool _isReloading = false;
    private Coroutine _reloadCoroutine;

    [Header("Reload: UI")]
    //[SerializeField] private Text _ammoDisplay;
    [SerializeField] private Text _totalAmmoDisplay;
    [SerializeField] private AmmoUI ammoUI; // Reference to the AmmoUI script


    private void Start()
    {
        _currentAmmoInChamber = _maxAmmo;
        _totalAmmo = _startTotalAmmo;
        ammoUI.UpdateAmmoDisplay(_maxAmmo, _currentAmmoInChamber); // Update the display to show full ammo
        UpdateAmmoUI();
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= _nextFireTime)
        {
            if (_isReloading)
            {
                StopCoroutine(_reloadCoroutine);
                _isReloading = false;
#if UNITY_EDITOR
                Debug.Log("����������� ��������!");
#endif
            }
            if (_currentAmmoInChamber >= 1)
            {
                Shoot(_bulletCount, _spread);
                _nextFireTime = Time.time + 1f / _fireRate;
                _currentAmmoInChamber--;
                _audioManager.PlaySound(_audioSource, _reloadSlightSound, _environmentGroup);
                ammoUI.UpdateAmmoDisplay(_maxAmmo, _currentAmmoInChamber); // Update the ammo display after shooting
                UpdateAmmoUI();
            }
        }
        if ((Input.GetKeyDown(KeyCode.R) && _currentAmmoInChamber < _maxAmmo && !_isReloading) || (_currentAmmoInChamber == 0 && !_isReloading && _totalAmmo != 0))
        {
            _reloadCoroutine = StartCoroutine(Reload());
        }
    }
    void Shoot(int bulletCount, float spread)
    {
        // Calculate the step between each bullet's angle
        float angleStep = spread / (bulletCount - 1); // Angle step between bullets
        float startAngle = -spread / 2; // Starting angle for the first bullet

        if (!_isReloading)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                // Calculate angle for the current bullet relative to the fire point's rotation
                float angle = startAngle + i * angleStep;
                Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(0, 0, angle);

                // Instantiate the bullet at the fire point position with the calculated rotation
                GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, bulletRotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                _audioManager.PlaySound(_audioSource, _shootingSound, _environmentGroup);

                // Apply velocity in the forward direction of the fire point
                Vector2 direction = bulletRotation * Vector2.up;
                rb.velocity = direction * _bulletSpeed;

                // Shake the camera (if applicable)
                CameraController.cameraShake(_shakeStrength, _shakeTime, _shakeFadeTime);
            }
        }
    }
    IEnumerator Reload()
    {
        _isReloading = true;
#if UNITY_EDITOR
        Debug.Log("�����������...");
#endif
        yield return new WaitForSeconds(0.5f); 

        // Loading bullets one by one with delay
        while (_currentAmmoInChamber < _maxAmmo && _totalAmmo >= 1)
        {
            _currentAmmoInChamber++; 
            _totalAmmo--;
            ammoUI.UpdateAmmoDisplay(_maxAmmo, _currentAmmoInChamber); // Update the ammo display after reloading
            UpdateAmmoUI(); 

            // Delay
            yield return new WaitForSeconds(_reloadDelay);
        }
        _isReloading = false;
        UpdateAmmoUI();
#if UNITY_EDITOR
        Debug.Log("����������� ���������!");
#endif
    }
    public void UpdateAmmoUI()
    {
        //_ammoDisplay.text = "Ammo: " + _currentAmmoInChamber + "/" + _maxAmmo;
        _totalAmmoDisplay.text = $"{_totalAmmo}";
        if (_totalAmmo == 0)
            _totalAmmoDisplay.color = Color.red;
        else
            _totalAmmoDisplay.color = Color.white;
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float angleStep = _spread / (_bulletCount - 1);
        float startAngle = -_spread / 2;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Vector2 direction = bulletRotation * _firePoint.up;
            Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)direction * 5f);
        }

        // Drawing spread cone
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, -_spread / 2) * _firePoint.up));
        Gizmos.DrawLine(_firePoint.position, _firePoint.position + (Vector3)(Quaternion.Euler(0, 0, _spread / 2) * _firePoint.up));
    }
#endif
}
