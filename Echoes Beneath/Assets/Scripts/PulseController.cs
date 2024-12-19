using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;



public class PulseController : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float CurrentPulse;
    [field: SerializeField] public float MinPulse {get ; private set;}
    [SerializeField] private float _maxPulse;
    [SerializeField] private float _pulseIncreaseRate;
    [SerializeField] private float _pulseDecreaseRate;
    public bool IsFlickering = false;

    [Header("Audio Settings: General")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;
    
    [Header("Audio Settings: Heartbeat")]
    [SerializeField] private AudioMixerGroup _heartbeatGroup;
    [SerializeField] private AudioClip _heartbeatClip; // Heartbeat sound
    private Coroutine _heartbeatCoroutine;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _maxAttenuation; // Should be negative
    
    [Header("Audio Settings: EarRing")]
    [SerializeField] private AudioSource _earRingAudioSource;
    [SerializeField] private AudioMixerGroup _earRingGroup;
    [SerializeField] private AudioClip _earRingClip; // Ear Ring sound
    [SerializeField] private float _maxEarRingVolume = -10f;
    [SerializeField] private float earRingTweenDuration = 0.5f; // Duration of ear ring volume animation
    [SerializeField] private string _earRingVolumeParameter = "EarRingVolume"; // Имя параметра в AudioMixer

    //[SerializeField] private AudioReverbFilter reverbFilter;
    //[SerializeField] private float reverbPulseThreshold = 120f;
    public float _heartBeatDelay;

    [Header("Post-Processing Settings")]
    [SerializeField] private Volume _blurVolume;
    [SerializeField] private Volume _bloomVolume;

    [Header("UI Settings")]
    [SerializeField] private Text _pulseText; // Show pulse value
    [SerializeField] private EKGMonitor _ekgMonitor;

    void Start()
    {
        CurrentPulse = MinPulse;
        if (_heartbeatCoroutine == null)
        {
            _heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
        }
        UpdatePulseUI();
    }
    private void Update()
    {
        DecreasePulse();
    }

    public void IncreasePulse(int damageAmount)
    {
        CurrentPulse += damageAmount * _pulseIncreaseRate;
        CurrentPulse = Mathf.Clamp(CurrentPulse, MinPulse, _maxPulse);
        AdjustEnvironmentVolume();
        AdjustEarRingVolume();
        UpdateVisualEffects();
        UpdatePulseUI();
    }
    public void IncreasePulseFromFlashlight(float pulseIncreaseAmount)
    {
        CurrentPulse += pulseIncreaseAmount * Time.deltaTime;
        CurrentPulse = Mathf.Clamp(CurrentPulse, MinPulse, _maxPulse);
        AdjustEnvironmentVolume();
        AdjustEarRingVolume();
        UpdateVisualEffects();
        UpdatePulseUI();
    }
    public void DecreasePulse()
    {
        if (CurrentPulse > MinPulse)
        {
            if(!IsFlickering)
            {
                CurrentPulse -= _pulseDecreaseRate * Time.deltaTime; // Плавное снижение пульса
                AdjustEnvironmentVolume();
                AdjustEarRingVolume();
                UpdateVisualEffects();
                UpdatePulseUI();
            }
        }
    }
    void AdjustEnvironmentVolume()
    {
        float volume = Mathf.Lerp(-10f, _maxAttenuation, (CurrentPulse - MinPulse) / (_maxPulse - MinPulse));
#if UNITY_EDITOR
        Debug.Log($"Setting Environment Volume to: {volume}");
#endif
        _audioMixer.SetFloat("EnvironmentVolume", volume);
    }
    void AdjustEarRingVolume()
    {
        float targetVolume = Mathf.Lerp(-80f, -15f, (CurrentPulse - MinPulse) / (_maxPulse - MinPulse));

        // Анимируем громкость параметра в AudioMixer через DoTween
        float currentVolume;
        _audioMixer.GetFloat(_earRingVolumeParameter, out currentVolume);
        DOTween.To(() => currentVolume, x => _audioMixer.SetFloat(_earRingVolumeParameter, x), targetVolume, earRingTweenDuration).SetEase(Ease.InOutCubic);
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
            
            _audioManager.PlaySound( _audioSource, _heartbeatClip, _heartbeatGroup); // Play heartbeat clip
            
            _ekgMonitor.StartPulseGeneration(); // Generate impulse on EKG
            
            _heartBeatDelay = (CurrentPulse > 0) ? 60f / CurrentPulse : 1f; // Counting delay between heartbeats
            yield return new WaitForSeconds(_heartBeatDelay);
        }
    }
}