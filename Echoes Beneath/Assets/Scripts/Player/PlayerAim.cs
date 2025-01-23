using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator _animator;  // Ссылка на компонент Animator
    [SerializeField] private SpriteRenderer _spriteRenderer;


    private Vector2 direction;
    public float angle;
    void Update()
    {
        // Получаем позицию мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Вычисляем направление от игрока к мыши
        direction = (mousePosition - transform.position).normalized;

        // Вычисляем угол в радианах
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Поворачиваем объект
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (direction.x < 0)
        {
            _spriteRenderer.flipX = true; // Отразить спрайт
        }
        else if (direction.x > 0)
        {
            _spriteRenderer.flipX = false; // Сбросить отражение
        }

        UpdateAnimator(direction);
    }
    private void UpdateAnimator(Vector2 aimDirection)
    {
        // Передаем направление взгляда
        _animator.SetFloat("AimX", aimDirection.x);
        _animator.SetFloat("AimY", aimDirection.y);
    }
}