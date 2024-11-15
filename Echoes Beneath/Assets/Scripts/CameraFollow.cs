using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // ������ �� ������ ������
    [SerializeField] private float smoothSpeed; // �������� ����������� ������
    private Vector3 offset; // �������� ������ ������������ ������

    void LateUpdate()
    {
        // ������� ������� ������ � ��� ������� ������ � ������ ��������
        Vector3 desiredPosition = player.position + offset;

        // ������� ����������� ������ � �������������� Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // ��������� ������� ������
        transform.position = smoothedPosition;

        // ������ ������ ����� �������� �� ������
        transform.LookAt(player);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
