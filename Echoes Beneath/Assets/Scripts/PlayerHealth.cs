using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [field:SerializeField] public int MaxHealth { get; private set; }
    private int _currentHealth;

    [Header("Medkit Settings")]
    [SerializeField] private int _healAmount = 25; // ���������� ������������������ ��������
    public int MedkitCount = 0; // ���������� �������
    [SerializeField] private KeyCode _useMedkitKey = KeyCode.Q; // ������� ��� ������������� �������

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
        UpdateMedkitUI();
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
    // ����� ��� ���������� ���������� �������
    public void AddMedkit()
    {
        MedkitCount++;
        UpdateMedkitUI();
    }

    // ����� ��� ������������� �������
    public void UseMedkit()
    {
        if (MedkitCount > 0 && _currentHealth < MaxHealth)
        {
            _currentHealth += _healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); // ������������ �������� ������������ ���������
            MedkitCount--;
            UpdateHealthUI();
            UpdateMedkitUI();
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
    void UpdateMedkitUI()
    {
        //_medkitDisplay.text = $"Medkits: {MedkitCount}";
    }
}
