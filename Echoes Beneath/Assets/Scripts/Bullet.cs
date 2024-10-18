using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifespan; // Время жизни пули
    [SerializeField] private LayerMask bulletLayer;

    void Start()
    {
        // Уничтожаем пулю через заданное время
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Уничтожаем пулю при столкновении
        if (collision.gameObject.layer == bulletLayer)
        {
            Destroy(gameObject);

        }

        // Здесь можно добавить дополнительную логику, например, урон врагам
        // Если нужно, можно проверить тег объекта, с которым произошло столкновение:
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // Логика урона врагу
        // }
    }
}
