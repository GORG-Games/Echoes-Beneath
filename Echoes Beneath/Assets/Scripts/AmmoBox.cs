using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    public int AmmoAmount;
    public TextMeshProUGUI pickupText; // Ссылка на объект текста

    void Start()
    {
        // Получаем ссылку на текст в дочернем объекте
        pickupText = GetComponentInChildren<TextMeshProUGUI>(true);

        // Делаем текст невидимым при старте
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    public void ShowPickupText()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(true); // Показываем текст
        }
    }

    public void HidePickupText()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false); // Скрываем текст
        }
    }
}
