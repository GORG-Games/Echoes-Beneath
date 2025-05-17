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
    private bool _isAlive = true;

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
    [SerializeField] private float earRingTweenDuration = 0.5f; // Duration of ear ring volume animation
    [SerializeField] private string _earRingVolumeParameter = "EarRingVolume"; // Имя параметра в AudioMixer

    //[SerializeField] private AudioReverbFilter reverbFilter;
    //[SerializeField] private float reverbPulseThreshold = 120f;
    public float _heartBeatDelay;

    [Header("Post-Processing Vignette Settings")]
    [SerializeField] private Image _vignetteImage;
    [SerializeField] private float _vignetteMaxAlpha = 0.5f;
    [SerializeField] private float _vignetteFadeSpeed = 2f;
    [SerializeField] private float _pulseThresholdForVignette = 90f; // примерный пульс, после которого начинается эффект

    [Header("PostProcessing Panic FX")]
    [SerializeField] private Volume _postProcessVolume;

    private Bloom _bloom;
    private ChromaticAberration _chromatic;
    private MotionBlur _motionBlur;
    private LensDistortion _lensDistortion;

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

        if (_postProcessVolume.profile.TryGet(out Bloom bloom))
            _bloom = bloom;
        if (_postProcessVolume.profile.TryGet(out ChromaticAberration chromatic))
            _chromatic = chromatic;
        if (_postProcessVolume.profile.TryGet(out MotionBlur blur))
            _motionBlur = blur;
        if (_postProcessVolume.profile.TryGet(out LensDistortion distortion))
            _lensDistortion = distortion;

        UpdatePulseUI();
        AdjustEarRingVolume();
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
/*#if UNITY_EDITOR
        Debug.Log($"Setting Environment Volume to: {volume}");
#endif*/
        _audioMixer.SetFloat("EnvironmentVolume", volume);
    }
    public void AdjustEarRingVolume()
    {
        float targetVolume = Mathf.Lerp(-80f, -15f, (CurrentPulse - MinPulse) / (_maxPulse - MinPulse));

        // Анимируем громкость параметра в AudioMixer через DoTween
        float currentVolume;
        _audioMixer.GetFloat(_earRingVolumeParameter, out currentVolume);
        DOTween.To(() => currentVolume, x => _audioMixer.SetFloat(_earRingVolumeParameter, x), targetVolume, earRingTweenDuration);
    }

    void UpdateVisualEffects()
    {
        float targetAlpha = 0f;

        if (CurrentPulse >= _pulseThresholdForVignette)
        {
            float intensity = Mathf.InverseLerp(_pulseThresholdForVignette, _maxPulse, CurrentPulse);
            targetAlpha = intensity * _vignetteMaxAlpha;
        }

        // Плавное обновление альфы виньетки
        Color currentColor = _vignetteImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * _vignetteFadeSpeed);
        _vignetteImage.color = currentColor;

        float panicLevel = Mathf.InverseLerp(_pulseThresholdForVignette, _maxPulse, CurrentPulse);

        if (_bloom != null)
            _bloom.intensity.value = Mathf.Lerp(1f, 4f, panicLevel);

        if (_chromatic != null)
            _chromatic.intensity.value = Mathf.Lerp(0f, 0.6f, panicLevel);

        if (_motionBlur != null)
            _motionBlur.intensity.value = Mathf.Lerp(0f, 1f, panicLevel);

        if (_lensDistortion != null)
            _lensDistortion.intensity.value = Mathf.Lerp(0f, -0.4f, panicLevel);
    }

    void UpdatePulseUI()
    {
        //pulseText.text = $"Pulse: {currentPulse} bpm";
    }
    IEnumerator HeartbeatRoutine()
    {
        while (_isAlive)
        {
            
            _audioManager.PlaySound( _audioSource, _heartbeatClip, _heartbeatGroup); // Play heartbeat clip
            
            _ekgMonitor.StartPulseGeneration(); // Generate impulse on EKG
            
            _heartBeatDelay = (CurrentPulse > 0) ? 60f / CurrentPulse : 1f; // Counting delay between heartbeats
            yield return new WaitForSeconds(_heartBeatDelay);
        }
    }
    public void StopHeartbeat()
    {
        _isAlive = false;
        if (_heartbeatCoroutine != null)
            StopCoroutine(_heartbeatCoroutine);
    }

    public void ResetAudioMixers()
    {
        _audioMixer.SetFloat("EnvironmentVolume", -10f);
        _audioMixer.SetFloat(_earRingVolumeParameter, -80f);
    }
}