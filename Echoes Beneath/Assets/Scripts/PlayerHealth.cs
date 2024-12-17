using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [field:SerializeField] public int MaxHealth { get; private set; }
    private int _currentHealth;

    [Header("Health UI")]
    [SerializeField] private Text _healthDisplay;

    [Header("Pulse")]
    private PulseController pulseController;

    void Start()
    {
        _currentHealth = MaxHealth;
        pulseController = gameObject.GetComponent<PulseController>();
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
#if UNITY_EDITOR
        Debug.Log($"Player Health: {_currentHealth}");
#endif
        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            Die();
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
