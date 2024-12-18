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

    [Header("Audio Settings")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup heartbeatGroup;
    [SerializeField] private AudioClip heartbeatClip; // Heartbeat sound
    private Coroutine heartbeatCoroutine;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float _maxAttenuation; // Should be negative
    //[SerializeField] private AudioReverbFilter reverbFilter;
    //[SerializeField] private float reverbPulseThreshold = 120f;
    public float delay;

    [Header("Post-Processing Settings")]
    [SerializeField] private Volume blurVolume;
    [SerializeField] private Volume bloomVolume;

    [Header("UI Settings")]
    [SerializeField] private Text pulseText; // Show pulse value


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
        UpdateVisualEffects();
        UpdatePulseUI();
    }
    public void IncreasePulseFromFlashlight(float pulseIncreaseAmount)
    {
        currentPulse += pulseIncreaseAmount * Time.deltaTime;
        currentPulse = Mathf.Clamp(currentPulse, minPulse, maxPulse);
        AdjustEnvironmentVolume();
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

            audioManager.PlaySound( audioSource, heartbeatClip, heartbeatGroup);
            // Counting delay between heartbeats
            delay = (currentPulse > 0) ? 60f / currentPulse : 1f;
/*#if UNITY_EDITOR
            Debug.Log($"Next heartbeat in {delay} seconds");
#endif*/
            yield return new WaitForSeconds(delay);
        }
    }
}