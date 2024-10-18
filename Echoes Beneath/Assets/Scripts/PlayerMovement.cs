using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Скорость перемещения

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Получаем ввод от пользователя
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D или стрелки влево/вправо
        movement.y = Input.GetAxisRaw("Vertical");   // W/S или стрелки вверх/вниз
    }

    void FixedUpdate()
    {
        // Перемещение игрока
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}

