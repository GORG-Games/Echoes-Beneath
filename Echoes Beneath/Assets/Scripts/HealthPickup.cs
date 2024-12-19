using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HealthPickup : MonoBehaviour
{
    public TextMeshProUGUI pickupText { get; private set; }// ������ �� ������ ������
    [SerializeField] private LayerMask _playerLayer;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, collision.gameObject))
        {
            ShowPickupText();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Utils.LayerMaskUtil.ContainsLayer(_playerLayer, collision.gameObject))
        {
            HidePickupText();
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