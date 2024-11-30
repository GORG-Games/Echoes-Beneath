using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPicker : MonoBehaviour
{
    private Shotgun _shotgun;

    private bool canPickUpAmmo = false;
    private AmmoBox _ammoBox;
    [SerializeField] private LayerMask ammoBoxLayer;

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
        // Добавляем патроны к общей переменной из скрипта дробовика
        _shotgun._totalAmmo += _ammoBox.AmmoAmount;
        _shotgun.UpdateAmmoUI();
        Debug.Log("Патроны подобраны!");

        // Уничтожаем объект с патронами
        Destroy(_ammoBox.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем слой объекта
        if (((1 << collision.gameObject.layer) & ammoBoxLayer) != 0)
        {
            _ammoBox = collision.GetComponent<AmmoBox>();
            if(_ammoBox != null)
            {
                _ammoBox.ShowPickupText();
                canPickUpAmmo = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ammoBoxLayer) != 0)
        {
            if (_ammoBox != null)
            {
                // Скрываем текст "E" на коробке
                _ammoBox.HidePickupText();
            }

            canPickUpAmmo = false;
            _ammoBox = null;
        }
    }
}
