using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D _flashlight;        // Light component of flashlight
    [SerializeField] private float _maxBattery;       // Max charge of battery
    [SerializeField] private float _lightDrainSpeed;    // Light drain speed 
    [SerializeField] private float _flickerThreshold;  // How much percent should be to start flickering
    [SerializeField] private float _chargeAmount;       // How much we charge after pressing F
    [SerializeField] private float _minRange;
    [SerializeField] private float _maxRange;


    private float _currentBattery;       // Current battery charge
    private bool _isFlickering = false;  // Flag to show if flashlight flickers

    [Header("UI Elements")]
    [SerializeField] private Slider batterySlider;

    [Header("Audio Settings")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;      // Ссылка на AudioSource для воспроизведения звуков
    [SerializeField] private AudioClip _drainSound;         // Звук разрядки фонарика
    [SerializeField] private AudioClip _flickerSound;       // Звук мигания фонарика
    [SerializeField] private AudioClip _chargeSound;        // Звук зарядки фонарика

    [Header("Pulse Settings")]
    [SerializeField] private PulseController _pulseController;
    [SerializeField] private float _pulseIncreaseAmount;

    void Start()
    {
        _currentBattery = _maxBattery;
        batterySlider.maxValue = _maxBattery;
        UpdateBatteryUI();
    }

    void Update()
    {
        DrainBattery();
        CheckBatteryLevel();
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleRecharge();
        }
    }


    void DrainBattery()
    {
        if (_currentBattery > 0)
        {
            _currentBattery -= _lightDrainSpeed * Time.deltaTime;
            _currentBattery = Mathf.Clamp(_currentBattery, 0, _maxBattery);
            UpdateBatteryUI();
        }
    }
    void CheckBatteryLevel()
    {
        if (_currentBattery <= _flickerThreshold && !_isFlickering)
        {
            _isFlickering = true;
            _pulseController.IsFlickering = true;
            StartCoroutine(FlickerLight());
        }
    }

    // Flickering couroutine
    System.Collections.IEnumerator FlickerLight()
    {
        while (_currentBattery <= _flickerThreshold)
        {
            _flashlight.enabled = !_flashlight.enabled;
            yield return new WaitForSeconds(Random.Range(_minRange, _maxRange)); // Random range of flickering
            _pulseController.IncreasePulseFromFlashlight(_pulseIncreaseAmount);
        }

        // After exitting flickering mode
        _flashlight.enabled = true;
        _isFlickering = false;
        _pulseController.IsFlickering = false;
    }

    // Charging battery
    void HandleRecharge()
    {
        _currentBattery += _chargeAmount;
        _currentBattery = Mathf.Clamp(_currentBattery, 0, _maxBattery);
        UpdateBatteryUI();
    }
    void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = _currentBattery;
        }
    }
}
