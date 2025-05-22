using UnityEngine;

public class BatteryPicker : MonoBehaviour
{
    private FlashlightController _flashlightController;

    private bool canPickUpBattery = false;
    private BatteryPack _batteryPack;
    [SerializeField] private LayerMask _batteryLayer;

    void Start()
    {
        if (_flashlightController == null)
        {
            _flashlightController = GetComponentInChildren<FlashlightController>();
        }
    }

    void Update()
    {
        if (canPickUpBattery && Input.GetKeyDown(KeyCode.E))
        {
            PickUpBattery();
        }
    }

    void PickUpBattery()
    {
        _flashlightController.AddBatteryPack();
        _flashlightController.UpdateBatteryUI();
#if UNITY_EDITOR
        Debug.Log("Батарейки подобраны!");
#endif
        Destroy(_batteryPack.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_batteryLayer, collision.gameObject))
        {
            _batteryPack = collision.GetComponent<BatteryPack>();
            if (_batteryPack != null)
            {
                canPickUpBattery = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_batteryLayer, collision.gameObject))
        {
            canPickUpBattery = false;
            _batteryPack = null;
        }
    }
}