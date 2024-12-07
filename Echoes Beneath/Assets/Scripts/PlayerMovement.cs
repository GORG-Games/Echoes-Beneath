using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // User Input
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D
        movement.y = Input.GetAxisRaw("Vertical");   // W/S
    }

    void FixedUpdate()
    {
        // Перемещение игрока
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}

