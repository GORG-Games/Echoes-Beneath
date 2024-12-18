using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;


public class PulseController : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float currentPulse;
    [field: SerializeField] public float minPulse {get ; private set;}
    [SerializeField] private float maxPulse = 210f;
    [SerializeField] private float pulseIncreaseRate = 10f;
    [SerializeField] private float pulseDecreaseRate = 1f;
    public bool IsFlickering = false;

    [Header("Audio Settings: General")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource audioSource;
    [Header("Audio Settings: Heartbeat")]
    [SerializeField] private AudioMixerGroup heartbeatGroup;
    [SerializeField] private AudioClip heartbeatClip; // Heartbeat sound
    private Coroutine heartbeatCoroutine;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float _maxAttenuation; // Should be negative
    [Header("Audio Settings: EarRing")]
    [SerializeField] private AudioSource earRingAudioSource;
    [SerializeField] private AudioMixerGroup earRingGroup;
    [SerializeField] private AudioClip earRingClip; // Ear Ring sound
    [SerializeField] private float maxEarRingVolume = -10f;

    //[SerializeField] private AudioReverbFilter reverbFilter;
    //[SerializeField] private float reverbPulseThreshold = 120f;
    public float _heartBeatDelay;

    [Header("Post-Processing Settings")]
    [SerializeField] private Volume blurVolume;
    [SerializeField] private Volume bloomVolume;

    [Header("UI Settings")]
    [SerializeField] private Text pulseText; // Show pulse value
    [SerializeField] private EKGMonitor ekgMonitor;

    void Start()
    {
        currentPulse = minPulse;
        if (heartbeatCoroutine == null)
        {
            heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
        }
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
        AdjustEnvironmentVolume();
        AdjustEarRingVolume();
        UpdateVisualEffects();
        UpdatePulseUI();
    }
    public void IncreasePulseFromFlashlight(float pulseIncreaseAmount)
    {
        currentPulse += pulseIncreaseAmount * Time.deltaTime;
        currentPulse = Mathf.Clamp(currentPulse, minPulse, maxPulse);
        AdjustEnvironmentVolume();
        AdjustEarRingVolume();
        UpdateVisualEffects();
        UpdatePulseUI();
    }
    public void DecreasePulse()
    {
        if (currentPulse > minPulse)
        {
            if(!IsFlickering)
            {
                currentPulse -= pulseDecreaseRate * Time.deltaTime; // Плавное снижение пульса
                AdjustEnvironmentVolume();
                AdjustEarRingVolume();
                UpdateVisualEffects();
                UpdatePulseUI();
            }
        }
    }
    void AdjustEnvironmentVolume()
    {
        float volume = Mathf.Lerp(0f, _maxAttenuation, (currentPulse - minPulse) / (maxPulse - minPulse));
#if UNITY_EDITOR
        Debug.Log($"Setting Environment Volume to: {volume}");
#endif
        audioMixer.SetFloat("EnvironmentVolume", volume);
    }
    void AdjustEarRingVolume()
    {
        float volume = Mathf.Lerp(-80f, maxEarRingVolume, (currentPulse - minPulse) / (maxPulse - minPulse));
        audioMixer.SetFloat("EarRingVolume", volume);
    }

    void UpdateVisualEffects()
    {
        // Управляем интенсивностью размытия и блум-эффекта
        /*if (blurVolume.profile.TryGet(out MotionBlur blur))
        {
            blur.intensity.value = Mathf.Lerp(0f, 1f, (currentPulse - minPulse) / (float)(maxPulse - minPulse));
        }

        if (bloomVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = Mathf.Lerp(0f, 2f, (currentPulse - minPulse) / (float)(maxPulse - minPulse));
        }*/
    }

    void UpdatePulseUI()
    {
        //pulseText.text = $"Pulse: {currentPulse} bpm";
    }
    IEnumerator HeartbeatRoutine()
    {
        while (true)
        {
            
            audioManager.PlaySound( audioSource, heartbeatClip, heartbeatGroup); // Play heartbeat clip
            
            ekgMonitor.StartPulseGeneration(); // Generate impulse on EKG
            
            _heartBeatDelay = (currentPulse > 0) ? 60f / currentPulse : 1f; // Counting delay between heartbeats
            yield return new WaitForSeconds(_heartBeatDelay);
        }
    }
}