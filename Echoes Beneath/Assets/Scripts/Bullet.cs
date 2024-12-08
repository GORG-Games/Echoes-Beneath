using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Path")]
    [SerializeField] private float _lifespan;
    private TrailRenderer trailRenderer;
    private LayerMask _bulletLayer;

    [Header("Dealing Damage")]
    [SerializeField] private LayerMask _enemyLayer;
    private EnemyHealth _enemyHealth;
    [SerializeField] private int damage = 10; // Damage dealt by the bullet
    private Vector2 _knockbackDirection;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.time = _lifespan/10;
        trailRenderer.startWidth = 0.05f;
        trailRenderer.endWidth = 0.00f;

        _bulletLayer = gameObject.layer;
        Destroy(gameObject, _lifespan);

        Physics2D.IgnoreLayerCollision(_bulletLayer, _bulletLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy bullet if collided
        if (Utils.LayerMaskUtil.ContainsLayer(_enemyLayer, collision.gameObject))
        {
            _enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (_enemyHealth != null)
            {
                _knockbackDirection = (collision.transform.position - transform.position).normalized;
                _enemyHealth.TakeDamage(damage, _knockbackDirection);
            }
        }
        Destroy(gameObject);
    }
}
