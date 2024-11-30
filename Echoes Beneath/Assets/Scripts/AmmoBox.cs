using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    public int AmmoAmount;
    public TextMeshProUGUI pickupText; // ������ �� ������ ������

    void Start()
    {
        // �������� ������ �� ����� � �������� �������
        pickupText = GetComponentInChildren<TextMeshProUGUI>(true);

        // ������ ����� ��������� ��� ������
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    public void ShowPickupText()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(true); // ���������� �����
        }
    }

    public void HidePickupText()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false); // �������� �����
        }
    }
}
