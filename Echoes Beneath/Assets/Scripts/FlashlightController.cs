using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D flashlight;        // Light component of flashlight
    [SerializeField] private float _maxBattery;       // Max charge of battery
    [SerializeField] private float _lightDrainSpeed;    // Light drain speed 
    [SerializeField] private float _flickerThreshold;  // How much percent should be to start flickering
    [SerializeField] private float _chargeAmount;       // How much we charge after pressing F
    [SerializeField] private float _minRange;
    [SerializeField] private float _maxRange;


    private float currentBattery;       // Current battery charge
    private bool isFlickering = false;  // Flag to show if flashlight flickers

    [Header("UI Elements")]
    [SerializeField] private Slider batterySlider; 

    void Start()
    {
        currentBattery = _maxBattery;
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
        if (currentBattery > 0)
        {
            currentBattery -= _lightDrainSpeed * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, _maxBattery);
            UpdateBatteryUI();
        }
    }
    void CheckBatteryLevel()
    {
        if (currentBattery <= _flickerThreshold && !isFlickering)
        {
            isFlickering = true;
            StartCoroutine(FlickerLight());
        }
    }

    // Flickering couroutine
    System.Collections.IEnumerator FlickerLight()
    {
        while (currentBattery <= _flickerThreshold)
        {
            flashlight.enabled = !flashlight.enabled;
            yield return new WaitForSeconds(Random.Range(_minRange, _maxRange)); // Random range of flickering
        }

        // After exitting flickering mode
        flashlight.enabled = true;
        isFlickering = false;
    }

    // Charging battery
    void HandleRecharge()
    {
        currentBattery += _chargeAmount;
        currentBattery = Mathf.Clamp(currentBattery, 0, _maxBattery);
        UpdateBatteryUI();
    }
    void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery;
        }
    }
}
