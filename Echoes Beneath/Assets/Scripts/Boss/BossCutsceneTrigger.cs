using UnityEngine;
using UnityEngine.Playables;
using Utils;

public class BossCutsceneTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2 movePlayerToPosition;
    [SerializeField] private MonoBehaviour[] playerControlScripts; // отключаем управление
    [SerializeField] private Collider2D triggerCollider;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (LayerMaskUtil.ContainsLayer(playerLayer, other.gameObject))
        {
            hasTriggered = true;

            // Отключаем управление
            foreach (var script in playerControlScripts)
            {
                if (script != null) script.enabled = false;
            }

            // Перемещаем игрока в центр
            if (playerTransform != null)
            {
                playerTransform.position = movePlayerToPosition;
            }

            // АХТУНГ - УДАЛИТЬ ПРИ СБОРКЕ - ЭТО МГНОВЕННОЕ НАЧАЛО БОЯ
            //BossController boss = FindObjectOfType<BossController>();
            //if (boss != null)
            //{
            //    boss.StartPhase1(); // Запускаем бой
            //}

            // Запускаем катсцену
            if (timelineDirector != null)
            {
                timelineDirector.Play();
            }

            // Деактивируем коллайдер, чтобы не триггерилось повторно
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    public void ActivatePlayer()
    {
        foreach (var script in playerControlScripts)
        {
            if (script != null) script.enabled = true;
        }
    }
}