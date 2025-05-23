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
    [SerializeField] private Text _batteryCountText;                // UI элемент с отображением кол-ва батареек
    [SerializeField] private float _pulseWhenOff;              // Сколько прибавлять к пульсу, когда фонарик выключен
    [field: SerializeField] public int _batteryPacks { get; private set; } = 0;                 // Кол-во запасных батареек

    private bool _isFlashlightOn = true;                            // Фонарик включён ли


    void Start()
    {
        _currentBattery = _maxBattery;
        batterySlider.maxValue = _maxBattery;
        UpdateBatteryUI();
        UpdateBatteryPackUI();
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFlashlight();
            }

            if (_isFlashlightOn)
            {
                _pulseController.DisablePulseDecay(false);
#if UNITY_EDITOR
            Debug.Log("Фонарик включен");
#endif
                DrainBattery();

                if (_currentBattery <= 0f)
                {
#if UNITY_EDITOR
                Debug.Log("Батарея на нуле");
#endif
                    _currentBattery = 0f;
                    TryConsumeBatteryPack();
                }
            }
            else
            {
                Debug.Log("Фонарик выключен — повышаем пульс");
                _pulseController.DisablePulseDecay(true);
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
    public void AddBatteryPack()
    {
        _batteryPacks++;
        UpdateBatteryPackUI();
    }

    // Charging battery
    public void UpdateBatteryUI()
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
            _batteryCountText.text = $"{_batteryPacks}";
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
