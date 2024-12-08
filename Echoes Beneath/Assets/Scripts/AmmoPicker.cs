using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPicker : MonoBehaviour
{
    private Shotgun _shotgun;

    private bool canPickUpAmmo = false;
    private AmmoBox _ammoBox;
    [SerializeField] private LayerMask _ammoBoxLayer;

    void Start()
    {
        if (_shotgun == null)
        {
            _shotgun = GetComponentInChildren<Shotgun>();
        }
    }

    
    void Update()
    {
        if (canPickUpAmmo && Input.GetKeyDown(KeyCode.E))
        {
            PickUpAmmo();
        }
    }

    void PickUpAmmo()
    {
        // Add ammo to shotgun
        _shotgun._totalAmmo += _ammoBox.AmmoAmount;
        _shotgun.UpdateAmmoUI();

#if UNITY_EDITOR
        Debug.Log("Патроны подобраны!");
#endif
        Destroy(_ammoBox.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // LayerMask check
        if (Utils.LayerMaskUtil.ContainsLayer(_ammoBoxLayer, collision.gameObject))
        {
            _ammoBox = collision.GetComponent<AmmoBox>();
            if(_ammoBox != null)
            {
                canPickUpAmmo = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_ammoBoxLayer, collision.gameObject))
        {

            canPickUpAmmo = false;
            _ammoBox = null;
        }
    }
}
