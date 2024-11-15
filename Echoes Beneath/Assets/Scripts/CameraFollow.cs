using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Ссылка на объект игрока
    [SerializeField] private float smoothSpeed; // Скорость сглаживания камеры
    private Vector3 offset; // Смещение камеры относительно игрока

    void LateUpdate()
    {
        // Целевая позиция камеры — это позиция игрока с учетом смещения
        Vector3 desiredPosition = player.position + offset;

        // Плавное перемещение камеры с использованием Lerp
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Обновляем позицию камеры
        transform.position = smoothedPosition;

        // Камера всегда будет смотреть на игрока
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
