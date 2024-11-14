using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifespan; // Время жизни пули
    [SerializeField] private LayerMask _enemyLayer;
    private LayerMask _bulletLayer;

    void Start()
    {
        _bulletLayer = gameObject.layer;
        // Уничтожаем пулю через заданное время
        Destroy(gameObject, _lifespan);

        Physics2D.IgnoreLayerCollision(_bulletLayer, _bulletLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Уничтожаем пулю при столкновении
        if (((1 << collision.gameObject.layer) & _enemyLayer) != 0)
        {
            Destroy(collision.gameObject); //уничтожаем 
            Destroy(gameObject); //уничтожаем пулю
        }
        else
            Destroy(gameObject); //уничтожаем пулю
            Debug.Log("Layer: " + collision.gameObject.layer);
            Debug.Log("Collided Layer: " + LayerMask.LayerToName(collision.gameObject.layer));

        // Здесь можно добавить дополнительную логику, например, урон врагам
        // Если нужно, можно проверить тег объекта, с которым произошло столкновение:
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // Логика урона врагу
        // }
    }
}
