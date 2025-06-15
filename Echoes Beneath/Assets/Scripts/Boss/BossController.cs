using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;

public enum BossState
{
    Idle,        // ожидание, до начала боя
    Phase1,      // первая активная фаза
    Transition,  // между фазами
    Phase2,      // вторая фаза — более агрессивная
    Dead         // смерть, завершение боя
}
public class BossController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int damageToPlayer = 40;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BossCutsceneTrigger trigger;
    [SerializeField] private FlashlightController flashlight;
    [SerializeField] private SceneLoader loader;
    int animVariant = 0;
    [field: SerializeField] public int phase2Threshold { get; private set; } = 50; // при каком здоровье переходит во 2 фазу

    [Header("Spawns")]
    [SerializeField] private GameObject[] lights;
    [SerializeField] private GameObject[] ammos;

    [Header("Components")]
    private Animator animator;
    private Rigidbody2D rb;

    public BossState CurrentState;

    //----------------------------------- PHASE 1
    private enum Phase1State
    {
        Waiting,
        Appearing,
        Dashing,
        Escaping,
        Transition
    }

    private Phase1State phase1State = Phase1State.Waiting;

    [Header("Phase 1 Settings")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private Transform playerTransform;

    private float phase1Timer = 0f;
    //private int successfulHits = 0;
    private bool wasHitThisDash = false;
    private Vector2 dashDirection;
    [SerializeField] float distance = 16f;

    [Header("Escaping")]
    [SerializeField] private float escapeSpeed = 7f;
    [SerializeField] private float escapeDuration = 1.5f;
    private float escapeTimer = 0f;
    private Vector2 escapeDirection;


    [Header("Minion Settings")]
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private GameObject minionPrefab;
    private List<GameObject> activeMinions = new();
    EnemyAI ai;
    private bool minionsSpawned = false;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bossHurtClip;

    [SerializeField] private AudioClip bossDashScreamClip;
    private bool isScreamPlayed = false;

    [SerializeField] private AudioClip stepClip;

    [SerializeField] private AudioClip bossScreamClip;
    private bool phase2screamPlayed = false;

    [SerializeField] private AudioClip bossMusicClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Routing")]
    [SerializeField] private AudioMixerGroup environmentMixerGroup;

    //----------------------------------- 

    [Header("Timeline Control")]
    [SerializeField] private PlayableDirector deathTimeline;

    public bool IsDashing => phase1State == Phase1State.Dashing;
    public bool WasHitThisDash => wasHitThisDash;

    void Start()
    {
        enemyHealth.ResetHealthToMax();
        CurrentState = BossState.Idle;

        animator = GetComponent<Animator>();
        animator.SetInteger("Phase", 0);

        rb = GetComponent<Rigidbody2D>();

        if (environmentMixerGroup != null)
        {
            if (sfxSource != null)
                sfxSource.outputAudioMixerGroup = environmentMixerGroup;

            if (musicSource != null)
                musicSource.outputAudioMixerGroup = environmentMixerGroup;
        }
        // Если бой начинается сразу — можно вызвать StartPhase1()
        // УДАЛИТЬ ПРИ СБОРКЕ
        //StartPhase1();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case BossState.Phase1:
                HandlePhase1();
                break;
            case BossState.Transition:
                HandleTransition();
                break;
            case BossState.Phase2:
                HandlePhase2();
                break;
            case BossState.Dead:
                // ничего
                break;
        }
    }

    public void StartPhase1()
    {
        animator.SetInteger("AnimVariant", 0);
        CurrentState = BossState.Phase1;
        FindObjectOfType<LightFlicker>()?.StartFlicker();
        foreach (GameObject lightObject in lights)
            if(lightObject != null) lightObject.SetActive(true);
        flashlight?.SetDrainInactive();
        // запустить анимацию, звуки, спавн и т.д.
        if (bossMusicClip != null && musicSource != null)
        {
            musicSource.clip = bossMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void HandlePhase1()
    {
        switch (phase1State)
        {
            case Phase1State.Waiting:
                animator.SetBool("IsMoving", false);
                phase1Timer += Time.deltaTime;
                if (phase1Timer >= dashCooldown)
                {
                    phase1Timer = 0f;
                    phase1State = Phase1State.Appearing;
                    transform.position = GetRandomSpawnPosition();
                    dashDirection = (playerTransform.position - transform.position).normalized;
                    wasHitThisDash = false;
                    // TODO: анимация подготовки
                }
                break;

            case Phase1State.Appearing:
                // Можем сделать небольшую паузу или анимацию
                if (!isScreamPlayed)
                {
                    if (bossDashScreamClip != null)
                    {
                        sfxSource.PlayOneShot(bossDashScreamClip);
                        isScreamPlayed = true;
                    }
                }
                animator.SetBool("IsMoving", false);
                phase1Timer += Time.deltaTime;
                if (phase1Timer >= 0.75f) // полсекунды подготовки
                {
                    phase1Timer = 0f;
                    phase1State = Phase1State.Dashing;
                }
                break;

            case Phase1State.Dashing:
                if (phase1State != Phase1State.Dashing)
                {

                    isScreamPlayed = false;
                    break;
                }
                Vector2 moveDir = dashDirection.normalized; // или escapeDirection.normalized
                UpdateAnimVariant(moveDir);
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", moveDir.x);
                animator.SetFloat("MoveY", moveDir.y);

                // flip спрайта — если используешь:
                spriteRenderer.flipX = moveDir.x > 0;
                transform.position += (Vector3)dashDirection * dashSpeed * Time.deltaTime;
                isScreamPlayed = false;
                break;

            case Phase1State.Escaping:


                Vector2 escMoveDir = escapeDirection.normalized; // или escapeDirection.normalized
                UpdateAnimVariant(escMoveDir);
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", escMoveDir.x);
                animator.SetFloat("MoveY", escMoveDir.y);
                // flip спрайта — если используешь:
                spriteRenderer.flipX = escMoveDir.x > 0;
                transform.position += (Vector3)escapeDirection * escapeSpeed * Time.deltaTime;

                escapeTimer += Time.deltaTime;

                if (escapeTimer >= escapeDuration)
                {
                    if (!minionsSpawned && enemyHealth.CurrentHealth <= phase2Threshold)
                    {
                        SpawnMinions();
                        minionsSpawned = true;
                        EnterTransition();
                        phase1Timer = 0f;
                    }
                    else
                    {
                        phase1State = Phase1State.Waiting;
                    }
                }
                break;

            case Phase1State.Transition:
                // ждём пока игрок убьёт всех миньонов
                if (AreMinionsDefeated())
                {
                    phase1Timer += Time.deltaTime;
                    if (phase1Timer >= 5f)
                    {
                        StartPhase2();
                    }
                }
                break;
        }
    }

    public void EnterTransition()
    {
        CurrentState = BossState.Transition;
        phase1Timer = 0f;
        phase2screamPlayed = false;
    }

    void HandleTransition()
    {
        phase1Timer += Time.deltaTime;

        // Крик только один раз
        if (!phase2screamPlayed && bossScreamClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(bossScreamClip);
            phase2screamPlayed = true;
        }

        if (phase1Timer >= 5f)
        {
            StartPhase2();
        }
    }

    void StartPhase2()
    {
        CurrentState = BossState.Phase2;

        // Можно сбросить счётчики
        phase1Timer = 0f;
        //successfulHits = 0;
        phase1State = Phase1State.Waiting;
        animator.SetInteger("Phase", 1);
        // Усиливаем босса:
        dashCooldown = 2.0f; // или меньше
        dashSpeed *= 1.25f;
        escapeSpeed *= 1.25f;

        activeMinions.Clear();
    }

    void HandlePhase2()
    {
        switch (phase1State)
        {
            case Phase1State.Waiting:
                animator.SetBool("IsMoving", false);
                phase1Timer += Time.deltaTime;
                if (phase1Timer >= dashCooldown)
                {
                    phase1Timer = 0f;
                    phase1State = Phase1State.Appearing;
                    transform.position = GetRandomSpawnPosition();
                    dashDirection = (playerTransform.position - transform.position).normalized;
                    wasHitThisDash = false;

                    if (bossDashScreamClip != null)
                        sfxSource.PlayOneShot(bossDashScreamClip);
                }
                break;

            case Phase1State.Appearing:
                animator.SetBool("IsMoving", false);
                phase1Timer += Time.deltaTime;
                if (phase1Timer >= 0.3f) // подготовка быстрее
                {
                    phase1Timer = 0f;
                    phase1State = Phase1State.Dashing;
                }
                break;

            case Phase1State.Dashing:
                if (phase1State != Phase1State.Dashing)
                    break;
                Vector2 moveDir = dashDirection.normalized;
                UpdateAnimVariant(moveDir);
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", moveDir.x);
                animator.SetFloat("MoveY", moveDir.y);
                spriteRenderer.flipX = moveDir.x > 0;
                transform.position += (Vector3)dashDirection * dashSpeed * Time.deltaTime;
                break;

            case Phase1State.Escaping:
                escapeDirection = -dashDirection;

                Vector2 escMoveDir = escapeDirection.normalized;
                UpdateAnimVariant(escMoveDir);
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", escMoveDir.x);
                animator.SetFloat("MoveY", escMoveDir.y);
                spriteRenderer.flipX = escMoveDir.x > 0;
                transform.position += (Vector3)escapeDirection * escapeSpeed * Time.deltaTime;
                escapeTimer += Time.deltaTime;

                if (escapeTimer >= escapeDuration)
                {
                    phase1Timer = 0f;
                    phase1State = Phase1State.Waiting;
                }
                break;
        }
    }
    public void RegisterHit()
    {
        if (bossHurtClip != null)
            sfxSource.PlayOneShot(bossHurtClip);
        wasHitThisDash = true;

        // Уходим сразу
        phase1State = Phase1State.Escaping;
        escapeTimer = 0f;
        escapeDirection = -dashDirection;

#if UNITY_EDITOR
        Debug.Log("Босс был ранен во время рывка. Уход за экран.");
#endif
    }
    void SpawnMinions()
    {
        foreach(GameObject ammoObject in ammos)
            if(ammoObject != null) ammoObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, minionSpawnPoints.Length);
            GameObject minion = Instantiate(minionPrefab, minionSpawnPoints[index].position, Quaternion.identity);
            ai = minion.GetComponent<EnemyAI>();
            if (ai != null && playerTransform != null)
            {
                ai.SetPlayerTarget(playerTransform);
                ai.IsPlayerInSight = true;
            }
            activeMinions.Add(minion);

            EnemyHealth enemyHealth = minion.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.SetBossReference(this);
            }
        }
    }
    public void NotifyMinionKilled(GameObject minion)
    {
        if (activeMinions.Contains(minion))
        {
            activeMinions.Remove(minion);
        }
    }
    bool AreMinionsDefeated()
    {
        return activeMinions.Count == 0;
    }
    private Vector3 GetRandomSpawnPosition()
    {
        // Пример: 4 фиксированные точки за экраном вокруг игрока
        Vector3 playerPos = playerTransform.position;

        switch (Random.Range(0, 4))
        {
            case 0: return playerPos + new Vector3(distance, 0, 0);   // справа
            case 1: return playerPos + new Vector3(-distance, 0, 0);  // слева
            case 2: return playerPos + new Vector3(0, distance, 0);   // сверху
            case 3: return playerPos + new Vector3(0, -distance, 0);  // снизу
            default: return playerPos;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (phase1State == Phase1State.Dashing &&
            Utils.LayerMaskUtil.ContainsLayer(playerLayer, collision.gameObject))
        {
            // нанести урон игроку
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageToPlayer);
            }

            // прекратить дэш
            phase1State = Phase1State.Escaping;
            escapeDirection = -dashDirection;
            escapeTimer = 0f;
        }
    }
    public void Die()
    {
        // отключить поведение, проиграть анимацию, вызвать конец боя
        if (deathTimeline != null)
        {
            deathTimeline.Play();
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        CurrentState = BossState.Dead;

        animator.SetBool("IsMoving", false);
        animator.SetInteger("AnimVariant", -1);
        // отключение скрипта логики, если нужно:
        //enabled = false;
    }
    public void OnDeathCutsceneEnd()
    { 
        if (loader != null)
        {
            loader.LoadSceneByIndex(0); // или "Level_Complete", что у тебя там
        }
        else
        {
            Debug.LogError("SceneLoader not found!");
        }
    }
    public void PlayStepSound(int eventVariant)
    {
        int currentVariant = animator.GetInteger("AnimVariant");

        if (currentVariant == eventVariant)
        {
            if (stepClip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(stepClip);
            }
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log($"Step ignored. eventVariant={eventVariant} currentVariant={currentVariant}");
        }
#endif
    }
    private void UpdateAnimVariant(Vector2 moveDir)
    {
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            animVariant = 0; // боковая
        }
        else
        {
            animVariant = moveDir.y >= 0 ? 1 : 2; // вверх или вниз
        }
        animator.SetInteger("AnimVariant", animVariant);
    }
    // Вызывается сигналом в конце Timeline
    public void EnablePlayerControlAndStartBoss()
    {
        trigger.ActivatePlayer();

        // Найдём и запустим бой (можно и через ссылку если нужно)
        BossController boss = FindObjectOfType<BossController>();
        if (boss != null)
        {
            boss.StartPhase1(); // Запускаем бой
        }
    }
}
