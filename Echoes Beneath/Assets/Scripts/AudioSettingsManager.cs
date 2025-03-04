using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private float divider = 100f;

    // ƒиапазон в децибелах, например от -80 до 0
    private float minVolume = -80f;
    private float maxVolume = 0f;

    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        // ѕреобразуем значение слайдера (0-1) в децибелы
        float volume = Mathf.Lerp(minVolume, maxVolume, sliderValue / divider);
        audioMixer.SetFloat("MasterVolume", volume);
    }
}