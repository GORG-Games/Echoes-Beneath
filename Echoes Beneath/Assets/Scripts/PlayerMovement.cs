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
#if UNITY_EDITOR
            Debug.Log("Playing footstep sound");
#endif
            audioManager.PlaySound(audioSource, footstepClip, _environmentGroup);
    }
}

