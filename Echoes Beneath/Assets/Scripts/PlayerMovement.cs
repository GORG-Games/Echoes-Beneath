using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource footstepAudioSource; // Ссылка на AudioSource для звука шагов
    [SerializeField] private AudioClip footstepClip;          // Step sound file
    [SerializeField] private float footstepDelay;      // Задержка между шагами

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isMoving;
    private float footstepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footstepTimer = footstepDelay;
    }

    void Update()
    {
        HandleInput();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        Move();
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
    void HandleFootsteps()
    {
        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstepSound();
                footstepTimer = footstepDelay;
            }
        }
        else
        {
            footstepTimer = 0f; // Reset Timer if character isn't moving
        }
    }
    void PlayFootstepSound()
    {
        if (footstepClip != null && footstepAudioSource != null)
        {
#if UNITY_EDITOR
            Debug.Log("Playing footstep sound");
#endif
            footstepAudioSource.PlayOneShot(footstepClip);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("AudioClip or AudioSource is missing!");
#endif
        }
    }
}

