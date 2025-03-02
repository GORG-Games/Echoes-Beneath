using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncOverlay : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer; // Ссылка на SpriteRenderer игрока

    [Header("Darkening Factor")]
    [SerializeField, Range(0.26f, 1.0f)] public float darkeningMultiplier = 0.6f; // Коэффициент затемнения (1 - без затемнения, меньше 1 - темнее)

    private SpriteRenderer overlaySpriteRenderer;

    void Awake()
    {
        overlaySpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player SpriteRenderer не назначен в SyncOverlay!");
        }
    }

    void LateUpdate()
    {
        if (playerSpriteRenderer != null && overlaySpriteRenderer != null)
        {
            // Синхронизируем спрайт
            overlaySpriteRenderer.sprite = playerSpriteRenderer.sprite;
            // Копируем флипы
            overlaySpriteRenderer.flipX = playerSpriteRenderer.flipX;
            overlaySpriteRenderer.flipY = playerSpriteRenderer.flipY;
            // Копируем цвет, но затемняем его
            Color playerColor = playerSpriteRenderer.color;
            overlaySpriteRenderer.color = new Color(playerColor.r * darkeningMultiplier,
                                                    playerColor.g * darkeningMultiplier,
                                                    playerColor.b * darkeningMultiplier,
                                                    playerColor.a);
            // Обновляем локальный порядок сортировки, чтобы оверлей был поверх игрока, но внутри Sorting Group
            overlaySpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
        }
    }
}
