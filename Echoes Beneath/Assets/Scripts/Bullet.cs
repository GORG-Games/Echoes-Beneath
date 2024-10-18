using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifespan; // ����� ����� ����
    [SerializeField] private LayerMask bulletLayer;

    void Start()
    {
        // ���������� ���� ����� �������� �����
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������� ���� ��� ������������
        if (collision.gameObject.layer == bulletLayer)
        {
            Destroy(gameObject);

        }

        // ����� ����� �������� �������������� ������, ��������, ���� ������
        // ���� �����, ����� ��������� ��� �������, � ������� ��������� ������������:
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // ������ ����� �����
        // }
    }
}
