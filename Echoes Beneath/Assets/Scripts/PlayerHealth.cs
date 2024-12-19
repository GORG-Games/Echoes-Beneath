using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Pulse")]
    private PulseController _pulseController;

    void Start()
    {
        _currentHealth = MaxHealth;
        _pulseController = gameObject.GetComponent<PulseController>();
        UpdateHealthUI();
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
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
#if UNITY_EDITOR
        Debug.Log($"Player Health: {_currentHealth}");
#endif
        _pulseController.IncreasePulse(damage);
        UpdateHealthUI();

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
            _currentHealth += _healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); // Ограничиваем здоровье максимальным значением
            MedkitCount--;
            UpdateHealthUI();
            firstAidKitUIManager.UpdateMedkitUI(MedkitCount);
        }
    }
    private void UpdateHealthUI()
    {
        _healthDisplay.text = $"HP: {_currentHealth}";
    }
    private void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Player has died!");
#endif
    }
}
