using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator _animator;  // ������ �� ��������� Animator
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

        if (angle > -45f && angle <= 45f)
        {
            _animator.SetInteger("Direction", 0); // Right
        }
        else if (angle > 45f && angle <= 135f)
        {
            _animator.SetInteger("Direction", 1); // Back
        }
        else if (angle > 135f || angle <= -135f)
        {
            _animator.SetInteger("Direction", 2); // Left
        }
        else if (angle > -135f && angle <= -45f)
        {
            _animator.SetInteger("Direction", 3); // Front
        }
    }
}