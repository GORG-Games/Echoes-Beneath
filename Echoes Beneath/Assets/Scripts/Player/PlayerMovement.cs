using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;

    [Header("Audio Settings")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;          // Step sound file
    [SerializeField] private float footstepDelay;      // Задержка между шагами
    [SerializeField] private AudioMixerGroup _environmentGroup;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isMoving;
    private float footstepTimer;

    private Animator _animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footstepTimer = footstepDelay;

        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        Move();
        UpdateAnimator();
    }
    void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        isMoving = movement != Vector2.zero;
    }
    void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    public void PlayFootstepSound(int eventVariant)
    {
        int currentVariant = _animator.GetInteger("AnimVariant");

        if (currentVariant == eventVariant)
        {
/*#if UNITY_EDITOR
            Debug.Log("Playing footstep sound");
#endif*/
            audioManager.PlaySound(audioSource, footstepClip, _environmentGroup);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"Footstep event ignored. eventVariant={eventVariant} currentVariant={currentVariant}");
#endif
        }
    }
    void UpdateAnimator()
    {
        _animator.SetBool("IsMoving", isMoving);
    }
}

