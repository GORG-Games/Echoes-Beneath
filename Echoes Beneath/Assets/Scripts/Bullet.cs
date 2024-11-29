using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifespan; // ����� ����� ����
    [SerializeField] private LayerMask _enemyLayer;
    private LayerMask _bulletLayer;
    private TrailRenderer trailRenderer;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = _lifespan/10; //����������������� �����
        trailRenderer.startWidth = 0.05f;  // ��������� ������ �����
        trailRenderer.endWidth = 0.00f;    // �������� ������ �����

        _bulletLayer = gameObject.layer;
        // ���������� ���� ����� �������� �����
        Destroy(gameObject, _lifespan);

        Physics2D.IgnoreLayerCollision(_bulletLayer, _bulletLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������� ���� ��� ������������
        if (((1 << collision.gameObject.layer) & _enemyLayer) != 0)
        {
            Destroy(collision.gameObject); //���������� 
            Destroy(gameObject); //���������� ����
        }
        else
            Destroy(gameObject); //���������� ����
            Debug.Log("Layer: " + collision.gameObject.layer);
            Debug.Log("Collided Layer: " + LayerMask.LayerToName(collision.gameObject.layer));

        // ����� ����� �������� �������������� ������, ��������, ���� ������
        // ���� �����, ����� ��������� ��� �������, � ������� ��������� ������������:
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // ������ ����� �����
        // }
    }
}
