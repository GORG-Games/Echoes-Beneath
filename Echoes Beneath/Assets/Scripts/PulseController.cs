using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;


public class PulseController : MonoBehaviour
{
    [Header("Pulse Settings")]
    private int currentPulse;
    [SerializeField] private int minPulse = 60;
    [SerializeField] private int maxPulse = 210;
    [SerializeField] private int pulseIncreaseRate = 10;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource heartbeatAudioSource;
    [SerializeField] private AudioClip heartbeatClip; // Звук сердцебиения
    private Coroutine heartbeatCoroutine;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float _maxAttenuation; // Should be negative

    [Header("Post-Processing Settings")]
    [SerializeField] private Volume blurVolume;
    [SerializeField] private Volume bloomVolume;

    [Header("UI Settings")]
    [SerializeField] private Text pulseText; // Show pulse value


    void Start()
    {
        if (heartbeatCoroutine == null)
        {
            heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
        }
        currentPulse = minPulse;
        UpdatePulseUI();
    }
    private void Update()
    {
        DecreasePulse();
    }

    public void IncreasePulse(int damageAmount)
    {
        currentPulse += damageAmount * pulseIncreaseRate;
        currentPulse = Mathf.Clamp(currentPulse, minPulse, maxPulse);
        AdjustAudioMix();
        UpdateVisualEffects();
        UpdatePulseUI();
    }

    public void DecreasePulse()
    {
        if (currentPulse > minPulse)
        {
            currentPulse -= 1; // Плавное снижение пульса
            AdjustAudioMix();
            UpdateVisualEffects();
            UpdatePulseUI();
        }
    }

    void AdjustAudioMix()
    {
        float attenuation = Mathf.Lerp(0f, _maxAttenuation, (currentPulse - minPulse) / (float)(maxPulse - minPulse));
        audioMixer.SetFloat("MasterVolume", attenuation);
    }

    void UpdateVisualEffects()
    {
        // Управляем интенсивностью размытия и блум-эффекта
        if (blurVolume.profile.TryGet(out MotionBlur blur))
        {
            blur.intensity.value = Mathf.Lerp(0f, 1f, (currentPulse - minPulse) / (float)(maxPulse - minPulse));
        }

        if (bloomVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = Mathf.Lerp(0f, 2f, (currentPulse - minPulse) / (float)(maxPulse - minPulse));
        }
    }

    void UpdatePulseUI()
    {
        pulseText.text = $"Pulse: {currentPulse} bpm";
    }
    IEnumerator HeartbeatRoutine()
    {
        while (true)
        {
            // Playing heartbeat sound
            heartbeatAudioSource.PlayOneShot(heartbeatClip);

            // Counting delay between heartbeats
            float delay = 60f / currentPulse; // Example: when 60 bpm => 1 second delay
            yield return new WaitForSeconds(delay);
        }
    }
}