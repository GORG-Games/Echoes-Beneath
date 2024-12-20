using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    void Update()
    {
        // �������� ������� ���� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ��������� ����������� �� ������ � ����
        Vector2 direction = (mousePosition - transform.position).normalized;

        // ��������� ���� � ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ������������ ������
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}