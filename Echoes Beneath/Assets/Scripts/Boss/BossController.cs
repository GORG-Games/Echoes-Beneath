using UnityEngine;

public enum BossState
{
    Idle,        // ожидание, до начала бо€
    Phase1,      // перва€ активна€ фаза
    Transition,  // между фазами
    Phase2,      // втора€ фаза Ч более агрессивна€
    Dead         // смерть, завершение бо€
}
public class BossController : MonoBehaviour
{
    [Header("ќсновные параметры")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("‘азы")]
    [SerializeField] private int phase2Threshold = 50; // при каком здоровье переходит во 2 фазу

    [Header(" омпоненты")]
    private Animator animator;
    private Rigidbody2D rb;

    private BossState currentState;

    void Start()
    {
        currentHealth = maxHealth;
        currentState = BossState.Idle;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // ≈сли бой начинаетс€ сразу Ч можно вызвать StartPhase1()
    }

    void Update()
    {
        switch (currentState)
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

    public void TakeDamage(int amount)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth <= phase2Threshold && currentState == BossState.Phase1)
        {
            EnterTransition();
        }
    }

    void StartPhase1()
    {
        currentState = BossState.Phase1;
        // запустить анимацию, звуки, спавн и т.д.
    }

    void HandlePhase1()
    {
        // логика первой фазы
    }

    void EnterTransition()
    {
        currentState = BossState.Transition;
        // анимаци€ перехода, отключение атак
    }

    void HandleTransition()
    {
        // дождатьс€ конца анимации, потом вызвать StartPhase2()
    }

    void StartPhase2()
    {
        currentState = BossState.Phase2;
        // друга€ логика, агрессивность
    }

    void HandlePhase2()
    {
        // логика второй фазы
    }

    void Die()
    {
        currentState = BossState.Dead;
        // отключить поведение, проиграть анимацию, вызвать конец бо€
    }
}
