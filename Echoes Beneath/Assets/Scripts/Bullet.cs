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

    [Header("Hit Effects")]
    [SerializeField] private GameObject bloodSplashPrefab;
    [SerializeField] private GameObject[] bloodDecalPrefabs; // массив префабов
    [SerializeField] private int numberOfDecals = 3;          // сколько штук спавнить
    [SerializeField] private float spawnRadius = 0.2f;        // радиус разброса
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float decalYOffset = 0.01f;
    [SerializeField] private float hitSoundVolume = 0.8f;

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

                if (bloodSplashPrefab != null)
                {
                    Vector2 direction = -_knockbackDirection; // от врага, в сторону вылета крови
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);

                    Instantiate(bloodSplashPrefab, transform.position, rotation);
                }
                for (int i = 0; i < numberOfDecals; i++)
                {

                    GameObject chosenDecal = bloodDecalPrefabs[Random.Range(0, bloodDecalPrefabs.Length)];

                    Vector2 offset = Random.insideUnitCircle * spawnRadius;
                    Vector3 spawnPosition = transform.position + new Vector3(offset.x, offset.y - decalYOffset, 0);
                    Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

                    DecalPool.Instance.Spawn(chosenDecal, spawnPosition, rotation);
                }
                if (hitSound != null)
                {
                    AudioSource.PlayClipAtPoint(hitSound, transform.position, hitSoundVolume);
                }
            }
        }
        Destroy(gameObject);
    }
}
