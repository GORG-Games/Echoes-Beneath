using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class FlashlightController : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D _flashlight;        // Light component of flashlight
    [SerializeField] private float _maxBattery;       // Max charge of battery
    [SerializeField] private float _lightDrainSpeed;    // Light drain speed 
    [SerializeField] private float _chargeAmount;       // How much we charge after pressing F


    private float _currentBattery;       // Current battery charge

    [Header("UI Elements")]
    [SerializeField] private Slider batterySlider;

    [Header("Audio Settings")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;      // Ссылка на AudioSource для воспроизведения звуков
    [SerializeField] private AudioClip _drainSound;         // Звук разрядки фонарика
    [SerializeField] private AudioClip _chargeSound;        // Звук зарядки фонарика
    [SerializeField] private AudioMixerGroup _environmentGroup;

    [Header("Pulse Settings")]
    [SerializeField] private PulseController _pulseController;
    [SerializeField] private float _pulseIncreaseAmount;

    [Header("Battery Pack System")]
    [SerializeField] private int _batteryPacks = 0;                 // Кол-во запасных батареек
    [SerializeField] private Text _batteryCountText;                // UI элемент с отображением кол-ва батареек
    [SerializeField] private float _pulseWhenOff = 1f;              // Сколько прибавлять к пульсу, когда фонарик выключен

    private bool _isFlashlightOn = true;                            // Фонарик включён ли


    void Start()
    {
        _currentBattery = _maxBattery;
        batterySlider.maxValue = _maxBattery;
        UpdateBatteryUI();
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFlashlight();
            }

            if (_isFlashlightOn)
            {
                Debug.Log("Фонарик включен");
                DrainBattery();

                if (_currentBattery <= 0f)
                {
                    Debug.Log("Батарея на нуле");
                    _currentBattery = 0f;
                    TryConsumeBatteryPack();
                }
            }
            else
            {
                Debug.Log("Фонарик выключен — повышаем пульс");
                _pulseController.IncreasePulseFromFlashlight(_pulseWhenOff * Time.deltaTime);
            }

            UpdateBatteryUI();
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

    // Charging battery
    void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = _currentBattery;
        }
    }
    private void UpdateBatteryPackUI()
    {
        if (_batteryCountText != null)
        {
            _batteryCountText.text = $"x{_batteryPacks}";
        }
    }


    private void ToggleFlashlight()
    {
        _isFlashlightOn = !_isFlashlightOn;
        _flashlight.enabled = _isFlashlightOn;
    }

    private void TryConsumeBatteryPack()
    {
        if (_batteryPacks > 0)
        {
            _batteryPacks--;
            _currentBattery = _maxBattery;
            UpdateBatteryUI();
            UpdateBatteryPackUI();
        }
        else
        {
            if (_isFlashlightOn)
            {
                ToggleFlashlight(); // Выключаем фонарик, если не хватает батареек
            }
        }
    }

    // Добавление батареек через предмет/подбор
    public void AddBatteryPack(int count)
    {
        _batteryPacks += count;
        UpdateBatteryUI();
    }
}
