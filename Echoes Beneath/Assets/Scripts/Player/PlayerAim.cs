using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator _animator;  // Ссылка на компонент Animator
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("References")]
    [SerializeField] private Transform firePoint;  // FirePoint – точка выстрела
    [SerializeField] private Transform player;    // Центр игрока
    [SerializeField] private Transform flashlight;

    [Header("Settings")]
    [SerializeField] private float minCursorDistance = 1.0f;  // Минимальная дистанция курсора

    [Header("Firepoint quadrant settings")]
    [SerializeField] private float up_x = 0.0f;
    [SerializeField] private float up_y = 1.0f;

    [SerializeField] private float down_x = 0.0f;
    [SerializeField] private float down_y = -1.0f;

    [SerializeField] private float left_x = -1.0f;
    [SerializeField] private float left_y = 0.0f;

    [SerializeField] private float right_x = 1.0f;
    [SerializeField] private float right_y = 0.0f;

    private Vector3 lastValidPosition; // Последняя корректная позиция FirePoint

    private Vector2 direction;
    public float angle;
    void Update()
    {
        // Получаем позицию мыши в мировых координатах
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distanceToCursor = Vector2.Distance(player.position, mousePosition);

        // Проверяем, достаточно ли курсор далеко
        if (distanceToCursor >= minCursorDistance)
        {
            // Если да – обновляем позицию FirePoint
            lastValidPosition = GetClosestFirePointPosition(mousePosition);
        }

        Vector2 direction = mousePosition - (Vector2)player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (direction.x < 0)
        {
            _spriteRenderer.flipX = true; // Отразить спрайт
        }
        else if (direction.x > 0)
        {
            _spriteRenderer.flipX = false; // Сбросить отражение
        }

        // FirePoint всегда на последней допустимой позиции
        firePoint.position = lastValidPosition;

        // **Поворачиваем firePoint в сторону курсора**
        RotateFirePoint(mousePosition);


        UpdateAnimator(direction);
    }
    private Vector2 GetClosestFirePointPosition(Vector2 cursorPosition)
    {
        Vector2 direction = (cursorPosition - (Vector2)player.position).normalized;

        // Определяем ближайшую фиксированную точку из 4 возможных
        Vector2[] firePointPositions = new Vector2[]
        {
            (Vector2)player.position + new Vector2(right_x, right_y), // Вправо
            (Vector2)player.position + new Vector2(left_x, left_y), // Влево
            (Vector2)player.position + new Vector2(up_x, up_y), // Вверх
            (Vector2)player.position + new Vector2(down_x, down_y) // Вниз
        };

        // Выбираем ближайшую точку к направлению прицела
        Vector2 bestPosition = firePointPositions[0];
        float maxDot = Vector2.Dot(direction, (bestPosition - (Vector2)player.position).normalized);

        for (int i = 1; i < firePointPositions.Length; i++)
        {
            float dot = Vector2.Dot(direction, (firePointPositions[i] - (Vector2)player.position).normalized);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestPosition = firePointPositions[i];
            }
        }

        return bestPosition;
    }
    private void RotateFirePoint(Vector2 cursorPosition)
    {
        Vector2 direction = cursorPosition - (Vector2)firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private void UpdateAnimator(Vector2 aimDirection)
    {
        // Передаем направление взгляда
        _animator.SetFloat("AimX", aimDirection.x);
        _animator.SetFloat("AimY", aimDirection.y);
    }
}