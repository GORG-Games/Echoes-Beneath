using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [field:SerializeField] public int MaxHealth { get; private set; }
    private int _currentHealth;

    [Header("Medkit Settings")]
    [SerializeField] private FirstAidKitUIManager firstAidKitUIManager;
    [SerializeField] private int _healAmount = 25; // Количество восстанавливаемого здоровья
    public int MedkitCount = 0; // Количество аптечек
    [SerializeField] private KeyCode _useMedkitKey = KeyCode.Q; // Клавиша для использования аптечки

    [Header("UI Elements")]
    [SerializeField] private Text _healthDisplay;
    [SerializeField] private Text _medkitDisplay;

    [Header("Audio Settings")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _takingDamageSound;
    [SerializeField] private AudioClip _usingMedSound;

    [SerializeField] private AudioMixerGroup _environmentGroup;

    [Header("Pulse")]
    [SerializeField] private EKGMonitor _ekgMonitor;
    private PulseController _pulseController;

    void Start()
    {
        _currentHealth = MaxHealth;
        _pulseController = gameObject.GetComponent<PulseController>();
        UpdateHealthUI();
        _ekgMonitor.UpdateEKGState(_currentHealth, MaxHealth);
        firstAidKitUIManager.UpdateMedkitUI(MedkitCount);
    }
    void Update()
    {
        if (Input.GetKeyDown(_useMedkitKey))
        {
            UseMedkit();
        }
    }

    public void TakeDamage(int damage)
    {
#if UNITY_EDITOR
            Debug.Log("Playing hurt sound");
#endif
        _audioManager.PlaySound(_audioSource, _takingDamageSound, _environmentGroup);
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
#if UNITY_EDITOR
        Debug.Log($"Player Health: {_currentHealth}");
#endif

        CameraController.cameraShake(damage / 2.5f, 0.1f, 0.1f);
        _pulseController.IncreasePulse(damage);
        UpdateHealthUI();
        _ekgMonitor.UpdateEKGState(_currentHealth, MaxHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    // Метод для увеличения количества аптечек
    public void AddMedkit()
    {
        MedkitCount++;
        firstAidKitUIManager.UpdateMedkitUI(MedkitCount);
    }

    // Метод для использования аптечки
    public void UseMedkit()
    {
        if (MedkitCount > 0 && _currentHealth < MaxHealth)
        {
            if (_usingMedSound != null)
                _audioManager.PlaySound(_audioSource, _usingMedSound, _environmentGroup);
            _currentHealth += _healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); // Ограничиваем здоровье максимальным значением
            MedkitCount--;
            UpdateHealthUI();
            _ekgMonitor.UpdateEKGState(_currentHealth, MaxHealth);
            firstAidKitUIManager.UpdateMedkitUI(MedkitCount);
        }
    }
    private void UpdateHealthUI()
    {
        _healthDisplay.text = $"{_currentHealth}";
    }
    private void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Player has died!");
#endif
    }
}
