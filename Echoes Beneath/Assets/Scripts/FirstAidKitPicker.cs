using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKitPicker : MonoBehaviour
{
    private bool _canPickUpKit = false;
    private PlayerHealth _playerHealth;
    private HealthPickup _healthPickup;
    [SerializeField] private LayerMask _FirstAidKitLayer;

    void Start()
    {
        if (_playerHealth == null)
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }
    }


    void Update()
    {
        if (_canPickUpKit && Input.GetKeyDown(KeyCode.E))
        {
            PickUpKit();
        }
    }

    void PickUpKit()
    {
        _playerHealth.AddMedkit();
#if UNITY_EDITOR
        Debug.Log("Аптечка подобрана!");
#endif
        Destroy(_healthPickup.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // LayerMask check
        if (Utils.LayerMaskUtil.ContainsLayer(_FirstAidKitLayer, collision.gameObject))
        {
            _healthPickup = collision.GetComponent<HealthPickup>();
            if (_healthPickup != null)
            {
                _canPickUpKit = true;
#if UNITY_EDITOR
                Debug.Log("можно подобрать аптечку");
#endif
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_FirstAidKitLayer, collision.gameObject))
        {

            _canPickUpKit = false;
            _playerHealth = null;
        }
    }
}
